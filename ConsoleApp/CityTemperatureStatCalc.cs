using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Data.SqlTypes;
using System.Text;

namespace ConsoleApp;

public sealed class RunningStats
{
    public int Min { get; private set; }
    
    public float MinAsFloat { get; private set; }

    public int Max { get; private set; }
    
    public float MaxAsFloat { get; private set; }
    
    public int TemperatureSum { get; private set; }
    
    public int NumTemperatures { get; private set; }
    
    public float TemperatureAvg { get; private set; }
    
    public void AddTemperature(int temperature)
    {
        if (temperature < Min) Min = temperature;
        if (temperature > Max) Max = temperature;

        TemperatureSum += temperature;
        NumTemperatures++;
    }

    public RunningStats FinalizeStats()
    {
        MinAsFloat = Min / 10.0f;
        MaxAsFloat = Max / 10.0f;
        TemperatureAvg = float.Round(TemperatureSum / 10.0f / NumTemperatures, 1);
        return this;
    }
}

public sealed class CityTemperatureStatCalc(int capacity)
{
    private readonly ConcurrentDictionary<ReadOnlyMemory<byte>, string> _cityNameCache =
        new(Environment.ProcessorCount, capacity, new SpanEqualityComparator());

    private readonly ConcurrentDictionary<string, RunningStats> _stats =
        new(Environment.ProcessorCount, capacity);

    public int NumCities => _stats.Count;

    public void AddCityTemp(CityTemp cityTemp)
    {
        if (!_cityNameCache.TryGetValue(cityTemp.City, out var cachedCityName))
        {
            cachedCityName = Encoding.UTF8.GetString(cityTemp.City.Span); 
            _cityNameCache.TryAdd(new ReadOnlyMemory<byte>(cityTemp.City.ToArray()), cachedCityName);
        }
        
        var runningStats = _stats.GetOrAdd(cachedCityName, _ => new RunningStats());
        runningStats.AddTemperature(cityTemp.Temperature);
    }

    public SortedDictionary<string, RunningStats> FinalizeStats()
    {
        return new SortedDictionary<string, RunningStats>(_stats.ToDictionary(
            kv => kv.Key,
            kv => kv.Value.FinalizeStats()));
    }
}