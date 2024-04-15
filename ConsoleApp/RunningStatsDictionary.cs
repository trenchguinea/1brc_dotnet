using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace ConsoleApp;

public sealed class RunningStatsDictionary(int capacity) : IEnumerable<KeyValuePair<ReadOnlyMemory<byte>, RunningStats>>
{
    private readonly Dictionary<int, List<KeyValuePair<ReadOnlyMemory<byte>, RunningStats>>> _dict = new(capacity);

    public int Count => _dict.Values.SelectMany(valueList => valueList).Count();

    public bool TryGetValue(ReadOnlySpan<byte> key, out RunningStats? stats)
    {
        var hashCode = SpanEqualityUtil.GetHashCode(key);
        if (_dict.TryGetValue(hashCode, out var matches))
        {
            stats = FindMatch(key, matches);
            return stats is not null;
        }

        stats = null;
        return false;
    }
    
    public void Add(ReadOnlySpan<byte> key, RunningStats value)
    {
        var keyBuffer = new byte[key.Length];
        key.CopyTo(keyBuffer);
        var keyAsMemory = new ReadOnlyMemory<byte>(keyBuffer);

        var hashCode = SpanEqualityUtil.GetHashCode(key);
        if (_dict.TryGetValue(hashCode, out var matches))
        {
            Debug.Assert(FindMatch(key, matches) is null);
            matches.Add(KeyValuePair.Create(keyAsMemory, value));
        }
        else
        {
            matches = new List<KeyValuePair<ReadOnlyMemory<byte>, RunningStats>>(5)
                { KeyValuePair.Create(keyAsMemory, value) };
            _dict.Add(hashCode, matches);
        }
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static RunningStats? FindMatch(ReadOnlySpan<byte> key, List<KeyValuePair<ReadOnlyMemory<byte>, RunningStats>> potentialMatches)
    {
        foreach (var match in potentialMatches)
        {
            if (SpanEqualityUtil.Equals(key, match.Key.Span))
            {
                return match.Value;
            }
        }

        return null;
    }

    public SortedDictionary<string, RunningStats> ToFinalDictionary()
    {
        var finalDict = new SortedDictionary<string, RunningStats>();
        foreach (var valueList in _dict.Values)
        {
            foreach (var kv in valueList)
            {
                finalDict.Add(Encoding.UTF8.GetString(kv.Key.Span), kv.Value.FinalizeStats());
            }
        }

        return finalDict;
    }

    public IEnumerator<KeyValuePair<ReadOnlyMemory<byte>, RunningStats>> GetEnumerator() =>
        _dict.Values.SelectMany(valueList => valueList).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}