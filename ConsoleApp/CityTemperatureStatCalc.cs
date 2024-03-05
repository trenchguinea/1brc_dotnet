using System.Reflection.Metadata;

namespace ConsoleApp;

public class RunningStats(float temperature)
{
    public float Min { get; private set; } = temperature;

    public float Max { get; private set; } = temperature;
    
    public (float, int) RunningAvg { get; private set; } = (temperature, 1);
    
    public void AddTemperature(float temperature)
    {
        if (temperature < Min) Min = temperature;
        if (temperature > Max) Max = temperature;

        var currRunningAvg = RunningAvg;
        RunningAvg = (currRunningAvg.Item1 + temperature, currRunningAvg.Item2 + 1);
    }

    public void AddRunningStats(RunningStats otherStats)
    {
        if (otherStats.Min < Min) Min = otherStats.Min;
        if (otherStats.Max > Max) Max = otherStats.Max;

        var thisRunningAvg = RunningAvg;
        var otherRunningAvg = otherStats.RunningAvg;
        RunningAvg = (thisRunningAvg.Item1 + otherRunningAvg.Item1, thisRunningAvg.Item2 + otherRunningAvg.Item2);
    }
}

public class CityTemperatureStatCalc(int capacity)
{
    private readonly Dictionary<string, RunningStats> _stats = new(capacity);

    public int NumCities => _stats.Count;

    public void AddCityTemp(CityTemp cityTemp)
    {
        if (_stats.TryGetValue(cityTemp.City, out var runningStats))
        {
            runningStats.AddTemperature(cityTemp.Temperature);
        }
        else
        {
            _stats.Add(cityTemp.City, new RunningStats(cityTemp.Temperature));
        }
    }
    
    public void Merge(CityTemperatureStatCalc other)
    {
        foreach (var (otherCity, otherStats) in other._stats)
        {
            if (_stats.TryGetValue(otherCity, out var thisStats))
            {
                thisStats.AddRunningStats(otherStats);
            }
            else
            {
                _stats.Add(otherCity, otherStats);
            }
        }
    }

    public SortedDictionary<string, RunningStats> FinalizeStats() => new(_stats);
}