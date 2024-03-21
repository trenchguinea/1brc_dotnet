using System.Diagnostics;
using System.Text;
using ConsoleApp;

var MaxProcessors = 32;
const int ExpectedCityCnt = 413;

var sw = Stopwatch.StartNew();
await using var reader = File.Open("/Users/seangriffin/Coding/1brc_dotnet/ConsoleApp/resources/measurements_large.txt", FileMode.Open);

var blockReader = new BlockReader(reader, 1024 * 1024);
var processorTasks = new Task<CityTemperatureStatCalc>[MaxProcessors];

var totalCalc = new CityTemperatureStatCalc(ExpectedCityCnt);

var processorCnt = -1;
var block = blockReader.ReadNextBlock();
while (!block.IsEmpty)
{
    processorCnt++;
    var state = new ProcessingState(ExpectedCityCnt, block);
    processorTasks[processorCnt] = Task<CityTemperatureStatCalc>.Factory.StartNew(BlockProcessor.ProcessBlock, state);
    
    if (processorCnt == MaxProcessors - 1)
    {
        foreach (var bucket in Interleaved(processorTasks))
        {
            var calcTask = await bucket;
            totalCalc.Merge(await calcTask);
        }
        processorCnt = -1;
    }

    block = blockReader.ReadNextBlock();
}

Array.Resize(ref processorTasks, processorCnt+1);
foreach (var bucket in Interleaved(processorTasks))
{
    var calcTask = await bucket;
    totalCalc.Merge(await calcTask);
}

// This uses the approach described in https://devblogs.microsoft.com/pfxteam/processing-tasks-as-they-complete/
static Task<Task<T>>[] Interleaved<T>(IReadOnlyCollection<Task<T>> tasks)
{
    var buckets = new TaskCompletionSource<Task<T>>[tasks.Count];
    var results = new Task<Task<T>>[buckets.Length];
    for (var i = 0; i < buckets.Length; i++) 
    {
        buckets[i] = new TaskCompletionSource<Task<T>>();
        results[i] = buckets[i].Task;
    }

    var nextTaskIndex = -1;
    Action<Task<T>> continuation = completed =>
    {
        var bucket = buckets[Interlocked.Increment(ref nextTaskIndex)];
        bucket.TrySetResult(completed);
    };

    foreach (var inputTask in tasks)
        inputTask.ContinueWith(continuation, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);

    return results;
}

var finalBuffer = new StringBuilder(16 * 1024);
finalBuffer.Append('{');
finalBuffer.AppendJoin(", ",
    totalCalc.FinalizeStats().Select(kv =>
        $"{kv.Key}={kv.Value.Min:F1}/{kv.Value.TemperatureAvg:F1}/{kv.Value.Max:F1}"));
finalBuffer.Append('}');
Console.WriteLine(finalBuffer);

sw.Stop();
Console.WriteLine($"Num cities: {totalCalc.NumCities}");
Console.WriteLine($"Total time: {sw.ElapsedMilliseconds}ms");