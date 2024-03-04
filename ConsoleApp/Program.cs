using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using ConsoleApp;

var sw = Stopwatch.StartNew();
await using var reader = File.Open("resources/measurements.txt", FileMode.Open);

var blockReader = new BlockReader(reader, 64 * 1024);
var processorTasks = new List<Task<CityTemperatureStats>>(210503);

var block = blockReader.ReadNextBlock();
var totalBlockCnt = 1;

while (!block.IsEmpty)
{
 //   await BlockProcessor.ProcessBlock(block);
    processorTasks.Add(Task<CityTemperatureStats>.Factory.StartNew(BlockProcessor.ProcessBlock, block));

    block = blockReader.ReadNextBlock();
    totalBlockCnt++;
}

var allCityTemps = new CityTemperatureStats(500);
await Task.Factory.ContinueWhenAll(processorTasks.ToArray(), tasks =>
{
    foreach (var task in tasks)
    {
        allCityTemps.Merge(task.GetAwaiter().GetResult());
    }
});

sw.Stop();

allCityTemps.Dump();
Console.WriteLine($"Num cities: {allCityTemps.NumCities}");
Console.WriteLine($"Total time: {sw.ElapsedMilliseconds}ms");
// Console.WriteLine($"Total block cnt: {totalBlockCnt}");