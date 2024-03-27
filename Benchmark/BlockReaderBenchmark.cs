using BenchmarkDotNet.Attributes;
using ConsoleApp;

namespace Benchmark;

[MemoryDiagnoser]
public class BlockReaderBenchmark
{
    [Params(512*1024, 1024 * 1024, 2 * 1024 * 1024)]
    public int BufferSize;

    private Stream? _stream;

    [GlobalSetup]
    public void Setup()
    {
        _stream = File.Open("resources/measurements_large.txt", FileMode.Open);
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _stream!.Dispose();
    }

    [Benchmark(Baseline = true)]
    public void ReadBlock()
    {
        _stream!.Position = 0;

        var blockReader = new BlockReader(_stream!, BufferSize);
        var block = blockReader.ReadNextBlock();
        while (!block.IsEmpty)
        {
            block.Dispose();
            blockReader.ReadNextBlock();
        }
    }
    
    [Benchmark]
    public void ReadBlock2()
    {
        _stream!.Position = 0;

        var blockReader = new BlockReader(_stream!, BufferSize);
        var block = blockReader.ReadNextBlock();
        while (!block.IsEmpty)
        {
            block.Dispose();
            blockReader.ReadNextBlock();
        }
    }

}