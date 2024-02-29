using System.Diagnostics;
using ConsoleApp;

using var reader = File.OpenText("resources/measurements_medium.txt");

var blockReader = new BlockReader(reader, 32 * 1024);
//var processorTasks = new List<Task<long>>(10480);

var block = Time.Me("ReadNextBlock", blockReader.ReadNextBlock);
var totalLineCnt = 0L;
while (!block.IsEmpty)
{
    totalLineCnt += await Time.Me("ProcessBlock", BlockProcessor.ProcessBlock, block);
    // processorTasks.Add(Task<Task<long>>.Factory.StartNew(BlockProcessor.ProcessBlock, block).Unwrap());

    if (block.IsDefinitelyLast)
        break;

    block = Time.Me("ReadNextBlock", blockReader.ReadNextBlock);
}

// var totalLineCnt = await Task<long>.Factory.ContinueWhenAll(processorTasks.ToArray(), tasks =>
// {
//     return tasks.Sum(t => t.GetAwaiter().GetResult());
// });

Console.WriteLine($"Total line cnt: {totalLineCnt}");