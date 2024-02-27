using System.Diagnostics;
using ConsoleApp;

var sw = Stopwatch.StartNew();

using var reader = File.OpenText("resources/measurements.txt");

var blockReader = new BlockReader(reader, 64 * 1024);
var processorTasks = new List<Task<int>>(10480);

var block = blockReader.ReadNextBlock();
while (!block.IsEmpty)
{
    processorTasks.Add(Task<int>.Factory.StartNew(BlockProcessor.ProcessBlock, block));
    block = blockReader.ReadNextBlock();
}

var totalLineCnt = await Task<int>.Factory.ContinueWhenAll(processorTasks.ToArray(), completedTasks =>
{
    return completedTasks.Sum(t => t.GetAwaiter().GetResult());
});

sw.Stop();
Console.WriteLine($"Elapsed: {sw.ElapsedMilliseconds}ms");
Console.WriteLine($"Total line cnt: {totalLineCnt}");