using System.Buffers;
using System.Diagnostics;

namespace ConsoleApp;

public sealed class Block : IDisposable
{
    public static readonly Block Empty = new();

    private readonly byte[] _backingBuffer;
    private ReadOnlyMemory<byte> _bytes;
    
    private Block()
    {
        _bytes = ReadOnlyMemory<byte>.Empty;
        _backingBuffer = Array.Empty<byte>();
    }

    public Block(ReadOnlySpan<byte> initialBuffer, ReadOnlySpan<byte> supplementalBuffer)
    {
        Debug.Assert(initialBuffer.Length > 0);

        var endsInNewLine = !supplementalBuffer.IsEmpty ?
            supplementalBuffer[^1] == Constants.NewLine :
            initialBuffer[^1] == Constants.NewLine;
        
        // Ensure blocks always end in a new line to simplify later logic elsewhere
        if (endsInNewLine)
        {
            var length = initialBuffer.Length + supplementalBuffer.Length;
            _backingBuffer = ArrayPool<byte>.Shared.Rent(length);

            var totalBlock = new Memory<byte>(_backingBuffer);
            initialBuffer.CopyTo(totalBlock[..initialBuffer.Length].Span);
            supplementalBuffer.CopyTo(totalBlock.Slice(initialBuffer.Length, supplementalBuffer.Length).Span);

            _bytes = totalBlock[..length];
        }
        else
        {
            var length = initialBuffer.Length + supplementalBuffer.Length + 1;
            _backingBuffer = ArrayPool<byte>.Shared.Rent(length);

            var totalBlock = new Memory<byte>(_backingBuffer);
            initialBuffer.CopyTo(totalBlock[..initialBuffer.Length].Span);
            supplementalBuffer.CopyTo(totalBlock.Slice(initialBuffer.Length, supplementalBuffer.Length).Span);
            totalBlock.Span[^1] = Constants.NewLine;

            _bytes = totalBlock[..length];
        }
    }

    public ReadOnlyMemory<byte> Bytes => _bytes;

    public bool IsEmpty => _bytes.IsEmpty;
    
    public void Dispose()
    {
        ArrayPool<byte>.Shared.Return(_backingBuffer);
        _bytes = ReadOnlyMemory<byte>.Empty;
    }
}