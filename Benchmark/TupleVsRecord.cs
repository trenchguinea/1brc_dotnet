using BenchmarkDotNet.Attributes;

namespace Benchmark;

public class TupleVsRecord
{
    record struct Item(long First, int Second);
    
    [Benchmark(Baseline = true)]
    public void CreateAndUseRecord()
    {
        var i = new Item(1L, 1);
        var first = i.First;
        var second = i.Second;
    }

    [Benchmark]
    public void CreateAndUseTuple()
    {
        var i = Tuple.Create(1L, 1);
        var first = i.Item1;
        var second = i.Item2;
    }
    
}