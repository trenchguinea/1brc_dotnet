namespace ConsoleApp;

public sealed class BlockReader(StreamReader reader, int bufferSize)
{
    private readonly Memory<char> _buffer = new char[bufferSize];
    private readonly Memory<char> _supplementalBuffer = new char[105];
    
    public Block ReadNextBlock()
    {
        var numRead = reader.ReadBlock(_buffer.Span);
        if (numRead == 0)
        {
            return Block.Empty;
        }
        
        if (numRead < bufferSize)
        {
            return new Block(_buffer[..numRead].Span, ReadOnlySpan<char>.Empty, true);
        }

        var supplementalBufferPos = -1;
        var supplementalSpan = _supplementalBuffer.Span;

        var nextChar = reader.Read();
        while (nextChar != -1)
        {
            supplementalSpan[++supplementalBufferPos] = (char) nextChar;

            if (nextChar == '\n')
                break;

            nextChar = reader.Read();
        }

        return new Block(_buffer.Span, supplementalSpan[..(supplementalBufferPos+1)], false);
    }
}