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
            var newlinePos = remainingBlockBytes.IndexOf(Constants.NewLine);
            var line = remainingBlockBytes[..newlinePos];
            
            var semicolonPos = line.IndexOf(Constants.Semicolon);
            var city = line[..semicolonPos];
            var temperature = line[(semicolonPos+1)..];
            
            statCalc.AddCityTemp(new CityTemp(city, temperature));
            
            remainingBlockBytes = remainingBlockBytes[(newlinePos+1)..];
        }

        Marshal.FreeHGlobal(buffer);
        return statCalc;
    }
}