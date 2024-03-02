using BenchmarkDotNet.Attributes;
using ConsoleApp;

namespace Benchmark;

[MemoryDiagnoser]
public class BlockReaderBenchmark
{
    [Params(32*1024, 64*1024, 128*1024)]
    public int BufferSize;

    private Stream? _stream;
    private BlockReader? _blockReader;
    
    [IterationSetup]
    public void Setup()
    {
        _stream = File.Open("resources/measurements_mediumlarge.txt", FileMode.Open);
        _blockReader = new BlockReader(_stream, BufferSize);
    }

    [IterationCleanup]
    public void Cleanup()
    {
        _stream!.Dispose();
    }

    [Benchmark]
    public void ReadBlock()
    {
        Block block;
        do
        {
            block = _blockReader!.ReadNextBlock();
        } while (!block.IsEmpty);
    }
}