using System.Diagnostics;
using System.Text;
using ConsoleApp;

const int MaxProcessors = 50;

var sw = Stopwatch.StartNew();
await using var reader = File.Open("/Users/seangriffin/Coding/1brc_dotnet/ConsoleApp/resources/measurements_large.txt", FileMode.Open);

var blockReader = new BlockReader(reader, 64 * 1024);
var processorTasks = new Task[MaxProcessors];
var statsCalc = new CityTemperatureStatCalc(413);

var processorCnt = -1;
var block = blockReader.ReadNextBlock();
while (!block.IsEmpty)
{
    processorCnt++;
    var state = new ProcessingState(statsCalc, block);
    processorTasks[processorCnt] = Task.Factory.StartNew(BlockProcessor.ProcessBlock, state);
    
    if (processorCnt == MaxProcessors - 1)
    {
        Task.WaitAll(processorTasks);
        processorCnt = -1;
    }

    block = blockReader.ReadNextBlock();
}

Array.Resize(ref processorTasks, processorCnt+1);
Task.WaitAll(processorTasks);

var finalBuffer = new StringBuilder(16 * 1024);
finalBuffer.Append('{');
finalBuffer.AppendJoin(", ",
    statsCalc.FinalizeStats().Select(kv =>
        $"{kv.Key}={kv.Value.MinAsFloat}/{kv.Value.TemperatureAvg}/{kv.Value.MaxAsFloat}"));
finalBuffer.Append('}');
Console.WriteLine(finalBuffer);

sw.Stop();
Console.WriteLine($"Num cities: {statsCalc.NumCities}");
Console.WriteLine($"Total time: {sw.ElapsedMilliseconds}ms");