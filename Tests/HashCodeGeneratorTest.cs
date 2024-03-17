using System.Buffers;

namespace Tests;

public class HashCodeGeneratorTest
{
    [Fact]
    public void GenerateHashCode_1Char()
    {
        var obj = new ReadOnlyMemory<byte>("a"u8.ToArray());
        GenerateHashCode(obj);
    }

    [Fact]
    public void GenerateHashCode_4Char()
    {
        var obj = new ReadOnlyMemory<byte>("abcd"u8.ToArray());
        GenerateHashCode(obj);
    }

    [Fact]
    public void GenerateHashCode_5Char()
    {
        var obj = new ReadOnlyMemory<byte>("abcde"u8.ToArray());
        GenerateHashCode(obj);
    }

    [Fact]
    public void GenerateHashCode_10Char()
    {
        var obj = new ReadOnlyMemory<byte>("abcdefghij"u8.ToArray());
        GenerateHashCode(obj);
    }

    private int GenerateHashCode(ReadOnlyMemory<byte> obj)
    {
        var span = obj.Span;
        uint length = (uint) obj.Length;
        uint initval = 0x32193759; // Arbitrary

        uint a, b, c, len;

        len = length;
        a = b = 0x9e3779b9; // Arbitrary
        c = initval;

        while (len >= 12)
        {
            a += span[0] + ((uint)span[1] << 8) + ((uint)span[2] << 16) + ((uint)span[3] << 24);
            b += span[4] + ((uint)span[5] << 8) + ((uint)span[6] << 16) + ((uint)span[7] << 24);
            c += span[8] + ((uint)span[9] << 8) + ((uint)span[10] << 16) + ((uint)span[11] << 24);
            Mix(ref a, ref b, ref c);

            span = span[12..];
            len -= 12;
        }

        c += length;
        switch (len)
        {
            case 11: c += (uint)span[10] << 24;
                goto case 10;
            case 10: c += (uint)span[9] << 16;
                goto case 9;
            case 9: c += (uint)span[8] << 8;
                // first byte of c is reserved for the length
                goto case 8;
            case 8: b += (uint)span[7] << 24;
                goto case 7;
            case 7: b += (uint)span[6] << 16;
                goto case 6;
            case 6: b += (uint)span[5] << 8;
                goto case 5;
            case 5: b += span[4];
                goto case 4;
            case 4: a += (uint)span[3] << 24;
                goto case 3;
            case 3: a += (uint)span[2] << 16;
                goto case 2;
            case 2: a += (uint)span[1] << 8;
                goto case 1;
            case 1: a += span[0];
                break;
        }

        Mix(ref a, ref b, ref c);
        return (int) c;
    }
    
    private static void Mix(ref uint a, ref uint b, ref uint c)
    {
        a -= b;
        a -= c;
        a ^= c >> 13;
        b -= c;
        b -= a;
        b ^= a << 8;
        c -= a;
        c -= b;
        c ^= b >> 13;
        a -= b;
        a -= c;
        a ^= c >> 12;
        b -= c;
        b -= a;
        b ^= a << 16;
        c -= a;
        c -= b;
        c ^= b >> 5;
        a -= b;
        a -= c;
        a ^= c >> 3;
        b -= c;
        b -= a;
        b ^= a << 10;
        c -= a;
        c -= b;
        c ^= b >> 15;
    }
}