using System.Buffers;

namespace ConsoleApp;

public static class BlockProcessor
{
    public static CityTemperatureStats ProcessBlock(object? block)
    {
        var asBlock = (Block) block!;

        var calculator = new CityTemperatureStatCalc(500);

        var remainingBlockBytes = asBlock.Bytes;
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

            calculator.AddCityTemp(new CityTemp(city, temperature));
            
            // Skip past newline
            remainingBlockBytes = remainingBlockBytes[(newlinePos+1)..];
        }

        return calculator.CalculateFinalStats();
    }
}