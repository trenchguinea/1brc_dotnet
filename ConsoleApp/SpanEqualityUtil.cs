using System.Collections.Concurrent;
using System.Text;

namespace ConsoleApp;

public static class SpanEqualityUtil
{
    private static readonly ConcurrentDictionary<int, int> HashCodes = [];

    public static bool Equals(ReadOnlySpan<byte> x, ReadOnlySpan<byte> y) => x.SequenceEqual(y);

    public static int Compare(ReadOnlySpan<byte> x, ReadOnlySpan<byte> y) => x.SequenceCompareTo(y);

    public static int GetHashCode(ReadOnlySpan<byte> span)
    {
        var len = span.Length;
        var hash = 0;

        switch (len)
        {
            case 1:
                hash = span[0];
                break;

            case 2:
            case 3:
                hash = BitConverter.ToInt16(span[..2]);
                break;
            
            default:
            {
                var div = span.Length / 4;
                for (var i = 0; i < div; ++i)
                {
                    hash ^= BitConverter.ToInt32(span.Slice(i * 4, 4));
                }

                hash ^= span.Length;
                break;
            }
        }

        //HashCodes[Encoding.UTF8.GetString(span.ToArray())] = hash;
        HashCodes[hash] = hash;
        return hash;
    }

    public static IDictionary<int, int> GetHashCodes => HashCodes;
}
