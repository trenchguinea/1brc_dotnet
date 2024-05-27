using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ConsoleApp;

public static class PartitionProcessor
{
    public static unsafe CityTemperatureStatCalc ProcessPartition(object? processingState)
    {
        var state = (ProcessingState)processingState!;

        var statCalc = new CityTemperatureStatCalc(state.ExpectedCityCount);
        var partition = state.Partition;

        var buffer = Marshal.AllocHGlobal(partition.Length);
        var bufferPtr = (byte*)buffer.ToPointer();
        var remainingBlockBytes = new Span<byte>(bufferPtr, partition.Length);

        var numRead = RandomAccess.Read(state.FileHandle, remainingBlockBytes, partition.Pos);

        Debug.Assert(numRead == partition.Length);

        while (!remainingBlockBytes.IsEmpty)
        {
            var semicolonPos = remainingBlockBytes.IndexOf(Constants.Semicolon);
            var newlinePos = remainingBlockBytes.IndexOf(Constants.NewLine);

            var city = remainingBlockBytes[..semicolonPos];
            var temperature = remainingBlockBytes.Slice(semicolonPos + 1, newlinePos - semicolonPos - 1);

            statCalc.AddCityTemp(new CityTemp(city, temperature));

            remainingBlockBytes = remainingBlockBytes[(newlinePos + 1)..];
        }

        Marshal.FreeHGlobal(buffer);
        return statCalc;
    }
}
