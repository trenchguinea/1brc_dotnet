using System.Diagnostics;

namespace ConsoleApp;

public class Block
{
    public static readonly Block Empty = new();

    public readonly ReadOnlyMemory<byte> Bytes;
    
    private Block()
    {
        Bytes = ReadOnlyMemory<byte>.Empty;
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

            Bytes = totalBlock;
        }
        else
        {
            var length = initialBuffer.Length + supplementalBuffer.Length + 1;
        
            var totalBlock = new Memory<byte>(new byte[length]);
            initialBuffer.CopyTo(totalBlock[..initialBuffer.Length].Span);
            supplementalBuffer.CopyTo(totalBlock.Slice(initialBuffer.Length, supplementalBuffer.Length).Span);
            totalBlock.Span[^1] = Constants.NewLine;

            Bytes = totalBlock;
        }
    }

    public bool IsEmpty => Bytes.IsEmpty;
}