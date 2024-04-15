using System.Buffers;
using System.Collections;
using System.Diagnostics;

namespace ConsoleApp;

public static class BlockProcessor
{
    public static CityTemperatureStatCalc ProcessBlock(object? processingState)
    {
        var state = (ProcessingState) processingState!;

        var statCalc = new CityTemperatureStatCalc(state.ExpectedCityCount);
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

            statCalc.AddCityTemp(new CityTemp(city.Span, temperature.Span));
            
            // Skip past newline
            remainingBlockBytes = remainingBlockBytes[(newlinePos+1)..];
        }
        
        // We're done with the block so free up the underlying buffer
        state.Block.Dispose();
        
        return statCalc;
    }
}