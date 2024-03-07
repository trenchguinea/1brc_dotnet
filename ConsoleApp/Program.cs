using System.Diagnostics;
using ConsoleApp;

var sw = Stopwatch.StartNew();
await using var reader = File.Open("resources/measurements_medium.txt", FileMode.Open);

var blockReader = new BlockReader(reader, 64 * 1024);
var processorTasks = new List<Task>(210503);
var statsCalc = new CityTemperatureStatCalc(413);

var block = blockReader.ReadNextBlock();
while (!block.IsEmpty)
{
    var state = new ProcessingState(statsCalc, block);
    processorTasks.Add(Task.Factory.StartNew(BlockProcessor.ProcessBlock, state));
    block = blockReader.ReadNextBlock();
}

await Task.WhenAll(processorTasks);

// var allCityTemps = new CityTemperatureStatCalc(413);
// await Task.Factory.ContinueWhenAll(processorTasks.ToArray(), tasks =>
// {
//     foreach (var task in tasks)
//     {
//         allCityTemps.Merge(task.GetAwaiter().GetResult());
//     }
// });

Console.Write("{");
Console.Write(string.Join(", ",
    statsCalc.FinalizeStats().Select(kv =>
        $"{kv.Key}={kv.Value.Min}/{float.Round(kv.Value.RunningAvg.Item1 / kv.Value.RunningAvg.Item2, 1)}/{kv.Value.Max}")));
Console.WriteLine("}");

sw.Stop();
Console.WriteLine($"Num cities: {statsCalc.NumCities}");
Console.WriteLine($"Total time: {sw.ElapsedMilliseconds}ms");