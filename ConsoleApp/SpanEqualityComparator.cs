namespace ConsoleApp;

public class SpanEqualityComparator : IEqualityComparer<ReadOnlyMemory<byte>>
{
    public bool Equals(ReadOnlyMemory<byte> x, ReadOnlyMemory<byte> y) => x.Span.SequenceEqual(y.Span);
    public int GetHashCode(ReadOnlyMemory<byte> obj) => obj.Span[0].GetHashCode();
}