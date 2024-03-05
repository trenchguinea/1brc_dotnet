using System.Text;
using BenchmarkDotNet.Attributes;

namespace Benchmark;

[MemoryDiagnoser]
public class CityTempBenchmark
{
    private ReadOnlyMemory<byte> _city;
    private ReadOnlyMemory<byte> _temp;
    
    [GlobalSetup]
    public void Setup()
    {
        _city = new ReadOnlyMemory<byte>("Montreal"u8.ToArray());
        _temp = new ReadOnlyMemory<byte>("10.9"u8.ToArray());
    }
    
    [Benchmark]
    public void ConvertToString()
    {
        Encoding.UTF8.GetString(_city.Span);
        Encoding.UTF8.GetString(_temp.Span);
    }

    [Benchmark]
    public void ConvertToMemory()
    {
        var numCityChars = Encoding.UTF8.GetCharCount(_city.Span);
        var numTempChars = Encoding.UTF8.GetCharCount(_temp.Span);
        
        var cityChars = new Memory<char>(new char[numCityChars]);
        var tempChars = new Memory<char>(new char[numTempChars]);
        
        Encoding.UTF8.GetChars(_city.Span, cityChars.Span);
        Encoding.UTF8.GetChars(_temp.Span, tempChars.Span);
    }
}