using System.Diagnostics;

namespace ConsoleApp;

public class Block
{
    public static readonly Block Empty = new();

    private ReadOnlyMemory<byte> _bytes;
    
    private Block()
    {
        _bytes = ReadOnlyMemory<byte>.Empty;
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
            
            var totalBlock = new Memory<byte>(new byte[length]);
            initialBuffer.CopyTo(totalBlock[..initialBuffer.Length].Span);
            supplementalBuffer.CopyTo(totalBlock.Slice(initialBuffer.Length, supplementalBuffer.Length).Span);

            _bytes = totalBlock;
        }
        else
        {
            var length = initialBuffer.Length + supplementalBuffer.Length + 1;
        
            var totalBlock = new Memory<byte>(new byte[length]);
            initialBuffer.CopyTo(totalBlock[..initialBuffer.Length].Span);
            supplementalBuffer.CopyTo(totalBlock.Slice(initialBuffer.Length, supplementalBuffer.Length).Span);
            totalBlock.Span[^1] = Constants.NewLine;

            _bytes = totalBlock;
        }
    }

    public ReadOnlyMemory<byte> Bytes => _bytes;

    public bool IsEmpty => _bytes.IsEmpty;

    public void Clear()
    {
        _bytes = ReadOnlyMemory<byte>.Empty;
    }
}