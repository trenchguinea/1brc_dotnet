using System.Buffers;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace ConsoleApp;

public static class PartitionProcessor
{
    public static unsafe CityTemperatureStatCalc ProcessPartition(object? processingState)
    {
        var state = (ProcessingState2) processingState!;

        var statCalc = new CityTemperatureStatCalc(state.ExpectedCityCount);
        var fileHandle = state.FileHandle;
        var partition = state.Partition;

        var buffer = Marshal.AllocHGlobal(partition.Length);
        var bufferPtr = (byte*) buffer.ToPointer();
        var remainingBlockBytes = new Span<byte>(bufferPtr, partition.Length);
        
        var numRead = RandomAccess.Read(fileHandle, remainingBlockBytes, partition.Pos);
        
        Debug.Assert(numRead == partition.Length);

        while (!remainingBlockBytes.IsEmpty)
        {
            // Get city
            var semicolonPos = remainingBlockBytes.IndexOf(Constants.Semicolon);
            var city = remainingBlockBytes[..semicolonPos];

            // Skip past semicolon
            remainingBlockBytes = remainingBlockBytes[(semicolonPos+1)..];
            
            // Get temperature
            var newlinePos = remainingBlockBytes.IndexOf(Constants.NewLine);
            var temperature = remainingBlockBytes[..newlinePos];

            statCalc.AddCityTemp(new CityTemp(city, temperature));

            // Skip past newline
            remainingBlockBytes = remainingBlockBytes[(newlinePos+1)..];
        }

        Marshal.FreeHGlobal(buffer);
        //memoryOwner.Dispose();
        return statCalc;
    }
}