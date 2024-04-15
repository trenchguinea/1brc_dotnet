namespace ConsoleApp;

public sealed class SpanEqualityComparator : IEqualityComparer<ReadOnlyMemory<byte>>, IComparer<ReadOnlyMemory<byte>>
{
    public static readonly SpanEqualityComparator Instance = new();

    private SpanEqualityComparator()
    {
    }

    public bool Equals(ReadOnlyMemory<byte> x, ReadOnlyMemory<byte> y) => SpanEqualityUtil.Equals(x.Span, y.Span);

    public int Compare(ReadOnlyMemory<byte> x, ReadOnlyMemory<byte> y) => SpanEqualityUtil.Compare(x.Span, y.Span);

    public int GetHashCode(ReadOnlyMemory<byte> obj) => SpanEqualityUtil.GetHashCode(obj.Span);
}