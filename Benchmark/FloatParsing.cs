using System.Globalization;
using System.Text;
using BenchmarkDotNet.Attributes;

namespace Benchmark;

[MemoryDiagnoser]
public class FloatParsing
{
    private ReadOnlyMemory<byte> _first;
    private ReadOnlyMemory<byte> _second;
    private ReadOnlyMemory<byte> _third;
    
    [GlobalSetup]
    public void Setup()
    {
        _first = new ReadOnlyMemory<byte>("17.3"u8.ToArray());
        _second = new ReadOnlyMemory<byte>("-9.2"u8.ToArray());
        _third = new ReadOnlyMemory<byte>("-20.9"u8.ToArray());
    }
    
    [Benchmark]
    public void ParseDefault()
    {
        float.Parse(_first.Span);
        float.Parse(_second.Span);
        float.Parse(_third.Span);
    }
    
    [Benchmark]
    public void ParseSpecificStyles()
    {
        float.Parse(_first.Span, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign);
        float.Parse(_second.Span, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign);
        float.Parse(_third.Span, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign);
    }

    [Benchmark]
    public void ParseSpecificStylesAndInvariantFormat()
    {
        float.Parse(_first.Span, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, NumberFormatInfo.InvariantInfo);
        float.Parse(_second.Span, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, NumberFormatInfo.InvariantInfo);
        float.Parse(_third.Span, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, NumberFormatInfo.InvariantInfo);
    }

}