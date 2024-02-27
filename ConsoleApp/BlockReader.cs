namespace ConsoleApp;

public sealed class BlockReader(TextReader reader, int bufferSize)
{
    private readonly char[] _buffer = new char[bufferSize];
    private readonly char[] _supplementalBuffer = new char[105];
    
    public Block ReadNextBlock()
    {
        var bufferSpan = _buffer.AsSpan();
        var numRead = reader.ReadBlock(bufferSpan);
        if (numRead == 0)
        {
            return new Block();
        }
        
        if (numRead < bufferSize)
        {
            return new Block(bufferSpan[..numRead], ReadOnlySpan<char>.Empty);
        }

        var supplementalBufferPos = -1;
        var nextChar = reader.Read();
        
        while (nextChar != -1)
        {
            _supplementalBuffer[++supplementalBufferPos] = (char) nextChar;

            if (nextChar == '\n')
                break;

            nextChar = reader.Read();
        }

        return new Block(bufferSpan, new ReadOnlySpan<char>(_supplementalBuffer, 0, supplementalBufferPos+1));
    }
}