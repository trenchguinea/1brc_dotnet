using System.Diagnostics;
using System.Text;

namespace ConsoleApp;

public class Program
{
    private static readonly int MaxProcessors = 32;
    private static readonly int ExpectedCityCnt = 413;
    private static readonly int BufferSize = 4 * 1024 * 1024;
    private static readonly string InputFile = "/Users/seangriffin/Coding/1brc_dotnet/ConsoleApp/resources/measurements_large.txt";

    public static async Task Main(string[] args)
    {
        var sw = Stopwatch.StartNew();
        
        var processorTasks = new Task<CityTemperatureStatCalc>[MaxProcessors];
        var totalCalc = new CityTemperatureStatCalc(ExpectedCityCnt);
        
        var partitioner = new MemoryMappedFilePartitioner(BufferSize);
        var partitions = partitioner.PartitionFile(InputFile);
        
        var processorCnt = -1;

        foreach (var partition in partitions)
        {
            processorCnt++;

            var state = new ProcessingState2(ExpectedCityCnt, partition);
            processorTasks[processorCnt] = Task<CityTemperatureStatCalc>.Factory.StartNew(MemoryMappedBlockProcessor.ProcessBlock, state);

            if (processorCnt == MaxProcessors - 1)
            {
                foreach (var calcTask in Interleaved(processorTasks))
                {
                    totalCalc.Merge(await calcTask.Unwrap());
                }
                processorCnt = -1;
            }
        }
        
        Array.Resize(ref processorTasks, processorCnt+1);
        foreach (var calcTask in Interleaved(processorTasks))
        {
            totalCalc.Merge(await calcTask.Unwrap());
        }

        var finalBuffer = new StringBuilder(16 * 1024);
        finalBuffer.Append('{');
        finalBuffer.AppendJoin(", ",
            totalCalc.FinalizeStats().Select(kv =>
                $"{kv.Key}={kv.Value.Min:F1}/{kv.Value.TemperatureAvg:F1}/{kv.Value.Max:F1}"));
        finalBuffer.Append('}');
        Console.WriteLine(finalBuffer);

        sw.Stop();
        Console.WriteLine($"Num blocks: {partitions.Length}");
        Console.WriteLine($"Num cities: {totalCalc.NumCities}");
        Console.WriteLine($"Total time: {sw.ElapsedMilliseconds}ms");

        // var sw = Stopwatch.StartNew();
        //
        // await using var reader = File.OpenRead(InputFile);
        //
        // var blockReader = new BlockReader(reader, BufferSize);
        // var processorTasks = new Task<CityTemperatureStatCalc>[MaxProcessors];
        //
        // var totalCalc = new CityTemperatureStatCalc(ExpectedCityCnt);
        //
        // var totalBlockCnt = 0;
        // var processorCnt = -1;
        // var block = blockReader.ReadNextBlock();
        // while (!block.IsEmpty)
        // {
        //     processorCnt++;
        //     totalBlockCnt++;
        //     
        //     var state = new ProcessingState(ExpectedCityCnt, block);
        //     processorTasks[processorCnt] = Task<CityTemperatureStatCalc>.Factory.StartNew(BlockProcessor.ProcessBlock, state);
        //     
        //     if (processorCnt == MaxProcessors - 1)
        //     {
        //         foreach (var calcTask in Interleaved(processorTasks))
        //         {
        //             totalCalc.Merge(await calcTask.Unwrap());
        //         }
        //         processorCnt = -1;
        //     }
        //     
        //     block = blockReader.ReadNextBlock();
        // }
        //
        // Array.Resize(ref processorTasks, processorCnt+1);
        // foreach (var calcTask in Interleaved(processorTasks))
        // {
        //     totalCalc.Merge(await calcTask.Unwrap());
        // }
        //
        // // Time.Me("Dump output", () =>
        // // {
        // var finalBuffer = new StringBuilder(16 * 1024);
        // finalBuffer.Append('{');
        // finalBuffer.AppendJoin(", ",
        //     totalCalc.FinalizeStats().Select(kv =>
        //         $"{kv.Key}={kv.Value.Min:F1}/{kv.Value.TemperatureAvg:F1}/{kv.Value.Max:F1}"));
        // finalBuffer.Append('}');
        // Console.WriteLine(finalBuffer);
        // // });
        //
        // sw.Stop();
        // Console.WriteLine($"Num blocks: {totalBlockCnt}");
        // Console.WriteLine($"Num cities: {totalCalc.NumCities}");
        // Console.WriteLine($"Total time: {sw.ElapsedMilliseconds}ms");
    }
    
    // This uses the approach described in https://devblogs.microsoft.com/pfxteam/processing-tasks-as-they-complete/
    private static Task<Task<T>>[] Interleaved<T>(IReadOnlyCollection<Task<T>> tasks)
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
}