using System.Diagnostics;
using ConsoleApp;

var sw = Stopwatch.StartNew();
await using var reader = File.Open("resources/measurements_medium.txt", FileMode.Open);

var blockReader = new BlockReader(reader, 64 * 1024);
var processorTasks = new List<Task<long>>(210503);

// var block = Time.Me("ReadNextBlock", blockReader.ReadNextBlock);
var block = blockReader.ReadNextBlock();
// var totalLineCnt = 0L;

while (!block.IsEmpty)
{
    // totalLineCnt += await Time.Me("ProcessBlock", BlockProcessor.ProcessBlock, block);
    // totalLineCnt += await BlockProcessor.ProcessBlock(block);
    processorTasks.Add(Task<Task<long>>.Factory.StartNew(BlockProcessor.ProcessBlock, block).Unwrap());

//    block = Time.Me("ReadNextBlock", blockReader.ReadNextBlock);
    block = blockReader.ReadNextBlock();
}

var totalLineCnt = await Task<long>.Factory.ContinueWhenAll(processorTasks.ToArray(), tasks =>
{
    return tasks.Sum(t => t.GetAwaiter().GetResult());
});

sw.Stop();
Console.WriteLine($"Total time: {sw.ElapsedMilliseconds}ms");
Console.WriteLine($"Total line cnt: {totalLineCnt}");