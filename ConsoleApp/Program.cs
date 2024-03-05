using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using ConsoleApp;

var sw = Stopwatch.StartNew();
await using var reader = File.Open("resources/measurements.txt", FileMode.Open);

var blockReader = new BlockReader(reader, 64 * 1024);
var processorTasks = new List<Task<CityTemperatureStatCalc>>(210503);

var block = blockReader.ReadNextBlock();
var totalBlockCnt = 1;

while (!block.IsEmpty)
{
    processorTasks.Add(Task<CityTemperatureStatCalc>.Factory.StartNew(BlockProcessor.ProcessBlock, block));

    block = blockReader.ReadNextBlock();
    totalBlockCnt++;
}

var allCityTemps = new CityTemperatureStatCalc(500);
await Task.Factory.ContinueWhenAll(processorTasks.ToArray(), tasks =>
{
    foreach (var task in tasks)
    {
        allCityTemps.Merge(task.GetAwaiter().GetResult());
    }
});

Console.Write("{");
Console.Write(string.Join(", ",
    allCityTemps.FinalizeStats().Select(kv =>
        $"{kv.Key}={kv.Value.Min}/{float.Round(kv.Value.RunningAvg.Item1 / kv.Value.RunningAvg.Item2, 1)}/{kv.Value.Max}")));
Console.WriteLine("}");

sw.Stop();
Console.WriteLine($"Num cities: {allCityTemps.NumCities}");
Console.WriteLine($"Total time: {sw.ElapsedMilliseconds}ms");
// Console.WriteLine($"Total block cnt: {totalBlockCnt}");