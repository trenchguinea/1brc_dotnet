using System.Buffers;
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
    
    // [Benchmark(Baseline = true)]
    // public void ParseDefault()
    // {
    //     float.Parse(_first.Span);
    //     float.Parse(_second.Span);
    //     float.Parse(_third.Span);
    // }
    
    [Benchmark(Baseline = true)]
    public void ParseCustom()
    {
        CustomParse(_first.Span);
        CustomParse(_second.Span);
        CustomParse(_third.Span);
    }

    [Benchmark]
    public void ParseCustom2()
    {
        CustomParse2(_first.Span);
        CustomParse2(_second.Span);
        CustomParse2(_third.Span);
    }

    private static int CustomParse(ReadOnlySpan<byte> num)
    {
        // The char encoding starts char 0 at code 48
        const int start = 48;
        
        // Code 45 is a -
        const int neg = 45;
        
        var i = num.Length - 1;

        var tenths = num[i--] - start;
        i--; // skip over decimal
        var ones = num[i--] - start;

        var tens = 0;
        
        // If i is negative then it means we're done parsing, else parse tens position (or negative sign)
        if (i >= 0)
            tens = num[i] - start;

        // It's a negative number if we either just parsed a -
        // or we have one more character remaining, which must be a -
        // because we'll never have a number > 99 or < 99
        var isNeg = tens == (neg - start) || i == 1;

        var asInt = 0;
        
        // Neg is a lower code than 0, so if tens is > 0 it means it's an actual number
        if (tens > 0)
            asInt += tens * 100;

        asInt += ones * 10;
        asInt += tenths;
        return isNeg ? asInt * -1 : asInt;
    }
    
    private static int CustomParse2(ReadOnlySpan<byte> num)
    {
        // The char encoding starts char 0 at code 48
        const int start = 48;
        
        // Code 45 is a -
        const int neg = 45;
        
        var i = num.Length - 1;

        var tenths = num[i] - start;
        i -= 2; // skip over decimal
        var ones = num[i--] - start;

        var tens = 0;
        
        // If i is negative then it means we're done parsing, else parse tens position (or negative sign)
        if (i >= 0)
            tens = num[i] - start;

        // It's a negative number if we either just parsed a -
        // or we have one more character remaining, which must be a -
        // because we'll never have a number > 99 or < 99
        var isNeg = tens == (neg - start) | i == 1;

        // Neg is a lower code than 0, so if tens is > 0 it means it's an actual number
        var asInt = (tens > 0 ? tens * 100 : 0) + (ones * 10) + tenths;
        return isNeg ? asInt * -1 : asInt;
    }

}