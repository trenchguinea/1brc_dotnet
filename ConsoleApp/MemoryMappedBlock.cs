using System.IO.MemoryMappedFiles;

namespace ConsoleApp;

public sealed class MemoryMappedBlock(MemoryMappedViewAccessor accessor, int size) : IDisposable
{
    public readonly MemoryMappedViewAccessor Accessor = accessor;
    public readonly int Size = size;

    public void Dispose()
    {
        Accessor.Dispose();
    }
}
