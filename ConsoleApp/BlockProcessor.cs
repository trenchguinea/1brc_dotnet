using System.Buffers;

namespace ConsoleApp;

public static class BlockProcessor
{
    public static void ProcessBlock(object? processingState)
    {
        var state = (ProcessingState) processingState!;

        var remainingBlockBytes = state.Block.Bytes;
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

            state.StatCalc.AddCityTemp(new CityTemp(city, temperature));
            
            // Skip past newline
            remainingBlockBytes = remainingBlockBytes[(newlinePos+1)..];
        }
        
        // We're done with the block so free up the underlying buffer
        state.Block.Dispose();
    }
}