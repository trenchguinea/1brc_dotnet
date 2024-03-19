namespace ConsoleApp;

public class SpanEqualityComparator : IEqualityComparer<ReadOnlyMemory<byte>>
{
    public static readonly SpanEqualityComparator Instance = new SpanEqualityComparator();

    private SpanEqualityComparator()
    {
    }
    
    public bool Equals(ReadOnlyMemory<byte> x, ReadOnlyMemory<byte> y) => x.Span.SequenceEqual(y.Span);

    public int GetHashCode(ReadOnlyMemory<byte> obj)
    {
        var span = obj.Span;
        var div = span.Length / 4;

        var hash = 0;
        for (var i = 0; i < div; ++i)
        {
            hash ^= BitConverter.ToInt32(span.Slice(i * 4, 4));
        }

        return HashCode.Combine(hash, span.Length);
    }
}