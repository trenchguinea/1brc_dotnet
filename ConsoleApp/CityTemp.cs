using System.Globalization;
using System.Text;

namespace ConsoleApp;

public readonly struct CityTemp(ReadOnlyMemory<byte> city, ReadOnlyMemory<byte> temp)
{
    public ReadOnlyMemory<byte> City { get; } = city;

    public int Temperature { get; } = ParseTemp(temp.Span);

    public override string ToString() => $"{Encoding.UTF8.GetString(City.Span)};{Temperature}";

    private static int ParseTemp(ReadOnlySpan<byte> temp)
    {
        // The char encoding starts char 0 at code 48
        const int start = 48;
        
        // Code 45 is a -
        const int neg = 45;
        
        var i = temp.Length - 1;

        var tenths = temp[i--] - start;
        i--; // skip over decimal
        var ones = temp[i--] - start;

        var tens = 0;
        
        // If i is negative then it means we're done parsing, else parse tens position (or negative sign)
        if (i >= 0)
            tens = temp[i] - start;

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
}
