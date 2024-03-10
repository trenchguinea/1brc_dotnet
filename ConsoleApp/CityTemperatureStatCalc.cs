using System.Collections.Concurrent;
using System.Text;

namespace ConsoleApp;

public sealed class RunningStats
{
    public float Min { get; private set; }

    public float Max { get; private set; }
    
    public float TemperatureSum { get; private set; }
    
    public int NumTemperatures { get; private set; }
    
    public void AddTemperature(float temperature)
    {
        if (temperature < Min) Min = temperature;
        if (temperature > Max) Max = temperature;

        TemperatureSum += temperature;
        NumTemperatures++;
    }
}

public sealed class CityTemperatureStatCalc(int capacity)
{
    private readonly ConcurrentDictionary<string, RunningStats> _stats =
        new(Environment.ProcessorCount, capacity);

    public int NumCities => _stats.Count;

    public void AddCityTemp(CityTemp cityTemp)
    {
        var runningStats = _stats.GetOrAdd(cityTemp.City, _ => new RunningStats());
        runningStats.AddTemperature(cityTemp.Temperature);
    }

    public SortedDictionary<string, RunningStats> FinalizeStats() => new(_stats);
}