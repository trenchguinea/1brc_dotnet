namespace ConsoleApp;

public sealed class BlockReader(Stream reader, int bufferSize)
{
    private readonly Memory<byte> _buffer = new byte[bufferSize];
    private readonly Memory<byte> _supplementalBuffer = new byte[105];
    
    public Block ReadNextBlock()
    {
        var numRead = reader.ReadAtLeast(_buffer.Span, bufferSize, false);
        if (numRead == 0)
            return Block.Empty;

        if (numRead < bufferSize)
            return new Block(_buffer[..numRead].Span, ReadOnlySpan<byte>.Empty);

        var supplementalBufferPos = -1;
        var supplementalSpan = _supplementalBuffer.Span;

        var nextByte = reader.ReadByte();
        while (nextByte != -1)
        {
            supplementalSpan[++supplementalBufferPos] = (byte) nextByte;

            if (nextByte == Constants.NewLine)
                break;

            nextByte = reader.ReadByte();
        }

        return new Block(_buffer.Span, supplementalSpan[..(supplementalBufferPos+1)]);
    }
}