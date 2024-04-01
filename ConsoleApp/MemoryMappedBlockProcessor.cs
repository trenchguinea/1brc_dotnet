using System.Buffers;

namespace ConsoleApp;

public class MemoryMappedBlockProcessor
{
    public static CityTemperatureStatCalc ProcessBlock(object? processingState)
    {
        var (expectedCityCount, block) = (ProcessingState2) processingState!;

        var statCalc = new CityTemperatureStatCalc(expectedCityCount);

        var blockBytes = ArrayPool<byte>.Shared.Rent(block.Size);
        block.Accessor.ReadArray(0, blockBytes, 0, blockBytes.Length);
        var remainingBlockBytes = blockBytes.AsMemory()[..block.Size];

        while (!remainingBlockBytes.IsEmpty)
        {
            // Get city
            var semicolonPos = remainingBlockBytes.Span.IndexOf(Constants.Semicolon);
            var city = remainingBlockBytes[..semicolonPos];
        
            // Skip past semicolon
            remainingBlockBytes = remainingBlockBytes[(semicolonPos+1)..];

            // Get temperature
            var newlinePos = remainingBlockBytes.Span.IndexOf(Constants.NewLine);
            var temperature = remainingBlockBytes[..newlinePos];

            var cityTemp = new CityTemp(city, temperature);
            statCalc.AddCityTemp(cityTemp);

            // Skip past newline
            remainingBlockBytes = remainingBlockBytes[(newlinePos+1)..];
        }

        ArrayPool<byte>.Shared.Return(blockBytes);

        // We're done with the block so free up the underlying buffer
        block.Dispose();
        
        return statCalc;
    }
}