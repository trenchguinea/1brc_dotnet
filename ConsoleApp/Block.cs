using System.Buffers;
using System.Diagnostics;

namespace ConsoleApp;

public sealed class Block : IDisposable
{
    public static readonly Block Empty = new();

    private readonly byte[] _initialBuffer;
    private readonly ArrayPool<byte> _bufferPool;
    private ReadOnlyMemory<byte> _bytes;

    private Block()
    {
        _initialBuffer = Array.Empty<byte>();
        _bufferPool = ArrayPool<byte>.Shared;
        _bytes = ReadOnlyMemory<byte>.Empty;
    }

    public Block(byte[] initialBuffer, int bufferLen, ArrayPool<byte> bufferPool)
    {
        Debug.Assert(initialBuffer.Length > 0);

        _initialBuffer = initialBuffer;
        _bufferPool = bufferPool;
        _bytes = new ReadOnlyMemory<byte>(initialBuffer, 0, bufferLen); 
    }

    public ReadOnlyMemory<byte> Bytes => _bytes;

    public bool IsEmpty => _bytes.IsEmpty;
    
    public void Dispose()
    {
        _bufferPool.Return(_initialBuffer);
        _bytes = ReadOnlyMemory<byte>.Empty;
    }
}