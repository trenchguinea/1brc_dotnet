using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.JavaScript;

namespace ConsoleApp;

public sealed class BlockReader(Stream reader, int bufferSize)
{
    private const int MaxLineLength = 107; // 100 in city name + ; + 5 in temperature + newline
    
    public Block ReadNextBlock()
    {
        var buffer = ArrayPool<byte>.Shared.Rent(bufferSize + MaxLineLength);

        var numRead = reader.Read(buffer, 0, bufferSize);
        if (numRead == 0)
            return Block.Empty;

        if (numRead < bufferSize)
        {
            EnsureBlockEndsInNewLine(ref buffer, ref numRead);
            return new Block(buffer, numRead, ArrayPool<byte>.Shared);
        }

        // If here, it means entire bufferSize was read
        // Walk forward until we either reach the end of the file or the new line to complete the block

        var pos = numRead;
        var nextByte = reader.ReadByte();
        while (nextByte != -1)
        {
            buffer[pos++] = (byte) nextByte;
            if (nextByte == Constants.NewLine)
                break;
            nextByte = reader.ReadByte();
        }

        EnsureBlockEndsInNewLine(ref buffer, ref pos);
        return new Block(buffer, pos, ArrayPool<byte>.Shared);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void EnsureBlockEndsInNewLine(ref byte[] buffer, ref int endPos)
    {
        if (buffer[endPos - 1] != Constants.NewLine)
        {
            buffer[endPos++] = Constants.NewLine;
        }
    }
}