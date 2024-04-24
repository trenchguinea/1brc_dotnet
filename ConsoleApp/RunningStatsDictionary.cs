using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace ConsoleApp;

public sealed class RunningStatsDictionary(int capacity) : IEnumerable<KeyValuePair<ReadOnlyMemory<byte>, RunningStats>>
{
    private readonly Dictionary<int, List<KeyValuePair<ReadOnlyMemory<byte>, RunningStats>>> _dict = new(capacity);

    public int Count => _dict.Values.SelectMany(valueList => valueList).Count();

    public bool TryGetValue(int keyHashCode, ReadOnlySpan<byte> key, [MaybeNullWhen(false)] out RunningStats stats)
    {
        Debug.Assert(keyHashCode == SpanEqualityUtil.GetHashCode(key));
        
        if (_dict.TryGetValue(keyHashCode, out var matches))
        {
            stats = FindMatch(key, matches);
            return stats is not null;
        }

        stats = null;
        return false;
    }
    
    public void Add(int keyHashCode, ReadOnlySpan<byte> key, RunningStats value)
    {
        Debug.Assert(keyHashCode == SpanEqualityUtil.GetHashCode(key));

        var keyBuffer = new byte[key.Length];
        key.CopyTo(keyBuffer);
        var keyAsMemory = new ReadOnlyMemory<byte>(keyBuffer);

        if (_dict.TryGetValue(keyHashCode, out var matches))
        {
            Debug.Assert(FindMatch(key, matches) is null);
            matches.Add(KeyValuePair.Create(keyAsMemory, value));
        }
        else
        {
            matches = new List<KeyValuePair<ReadOnlyMemory<byte>, RunningStats>>(5)
                { KeyValuePair.Create(keyAsMemory, value) };
            _dict.Add(keyHashCode, matches);
        }
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static RunningStats? FindMatch(ReadOnlySpan<byte> key, List<KeyValuePair<ReadOnlyMemory<byte>, RunningStats>> potentialMatches)
    {
        foreach (var match in CollectionsMarshal.AsSpan(potentialMatches))
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
        foreach (var kv in this)
        {
            finalDict.Add(Encoding.UTF8.GetString(kv.Key.Span), kv.Value.FinalizeStats());
        }

        return finalDict;
    }

    public IEnumerator<KeyValuePair<ReadOnlyMemory<byte>, RunningStats>> GetEnumerator() =>
        _dict.Values.SelectMany(valueList => valueList).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}