using System.Diagnostics;
using System.Runtime.InteropServices.JavaScript;
using System.Text;

namespace ConsoleApp;

public readonly struct Block
{
    public static readonly Block Empty = new();
    
    public readonly ReadOnlyMemory<char> Chars;
    public readonly bool IsDefinitelyLast;
    
    public Block()
    {
        Chars = ReadOnlyMemory<char>.Empty;
        IsDefinitelyLast = true;
    }

    public Block(ReadOnlySpan<char> initialBuffer, ReadOnlySpan<char> supplementalBuffer, bool isDefinitelyLast)
    {
        Debug.Assert(initialBuffer.Length > 0);

        var length = initialBuffer.Length + supplementalBuffer.Length;

        var totalBlock = new Memory<char>(new char[length]);
        initialBuffer.CopyTo(totalBlock[..initialBuffer.Length].Span);
        supplementalBuffer.CopyTo(totalBlock.Slice(initialBuffer.Length, supplementalBuffer.Length).Span);

        Chars = totalBlock;
        IsDefinitelyLast = isDefinitelyLast;
    }

    public bool IsEmpty => Chars.IsEmpty;
}
