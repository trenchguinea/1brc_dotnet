using System.Collections.Concurrent;
using System.Text;

namespace ConsoleApp;

public sealed class RunningStats
{
    private int _min;
    private int _max;
    private int _temperatureSum;
    private int _numTemperatures;

    public float Min { get; private set; }
    public float Max { get; private set; }
    public float TemperatureAvg { get; private set; }
    
    public void AddTemperature(int temperature)
    {
        if (temperature < _min) _min = temperature;
        if (temperature > _max) _max = temperature;

        _temperatureSum += temperature;
        _numTemperatures++;
    }

    public void Merge(RunningStats other)
    {
        if (other._min < _min) _min = other._min;
        if (other._max > _max) _max = other._max;

        _temperatureSum += other._temperatureSum;
        _numTemperatures += other._numTemperatures;
    }
    
    public RunningStats FinalizeStats()
    {
        Min = _min / 10.0f;
        Max = _max / 10.0f;
        TemperatureAvg = _temperatureSum / 10.0f / _numTemperatures;
        return this;
    }
}

public sealed class CityTemperatureStatCalc(int capacity)
{
    private static readonly ConcurrentDictionary<ReadOnlyMemory<byte>, ReadOnlyMemory<byte>> CachedCityNames
        = new(Environment.ProcessorCount * 2, 413, SpanEqualityComparator.Instance);
    
    private readonly Dictionary<ReadOnlyMemory<byte>, RunningStats> _stats =
        new(capacity, SpanEqualityComparator.Instance);
    
    public int NumCities => _stats.Count;

    public void AddCityTemp(CityTemp cityTemp)
    {
        if (!_stats.TryGetValue(cityTemp.City, out var runningStats))
        {
            if (!CachedCityNames.TryGetValue(cityTemp.City, out var cachedName))
            {
                cachedName = new ReadOnlyMemory<byte>(cityTemp.City.ToArray());
                CachedCityNames[cachedName] = cachedName;
            }

            runningStats = new RunningStats();
            _stats.Add(cachedName, runningStats);
        }
        runningStats.AddTemperature(cityTemp.Temperature);
    }
    
    public void Merge(CityTemperatureStatCalc other)
    {
        using var otherKvs = other._stats.GetEnumerator();
        while (otherKvs.MoveNext())
        {
            var otherKv = otherKvs.Current;
            if (this._stats.TryGetValue(otherKv.Key, out var thisRunningStats))
            {
                thisRunningStats.Merge(otherKv.Value);
            }
            else
            {
                this._stats.Add(otherKv.Key, otherKv.Value);
            }
        }
    }

    public SortedDictionary<string, RunningStats> FinalizeStats()
    {
        return new SortedDictionary<string, RunningStats>(_stats.ToDictionary(
            kv => Encoding.UTF8.GetString(kv.Key.Span),
            kv => kv.Value.FinalizeStats()));
    }
}