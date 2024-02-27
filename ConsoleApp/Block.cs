using System.Diagnostics;

namespace ConsoleApp;

public readonly struct Block
{
    public Block()
    {
        Length = 0;
        Chars = ReadOnlyMemory<char>.Empty;
    }

    public Block(ReadOnlySpan<char> initialBuffer, ReadOnlySpan<char> supplementalBuffer)
    {
        Debug.Assert(initialBuffer.Length > 0);

        Length = initialBuffer.Length + supplementalBuffer.Length;

        var totalBlock = new Memory<char>(new char[Length]);
        initialBuffer.CopyTo(totalBlock[..initialBuffer.Length].Span);
        supplementalBuffer.CopyTo(totalBlock.Slice(initialBuffer.Length, supplementalBuffer.Length).Span);

        Chars = totalBlock;
    }

    public bool IsEmpty => Length == 0;

    public ReadOnlyMemory<char> Chars { get; }
    
    public int Length { get; }
}
