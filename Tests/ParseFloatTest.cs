using System.Text;

namespace Tests;

public class ParseFloatTest
{
    [Theory]
    [InlineData("-20.9", -209)]
    [InlineData("-2.9", -29)]
    [InlineData("20.9", 209)]
    [InlineData("2.9", 29)]
    [InlineData("0.0", 0)]
    [InlineData("-0.0", 0)]
    public void TestCustomParsing_NegativeFullLength(string numAsStr, int expected)
    {
        var parsed = CustomParse(new ReadOnlySpan<byte>(Encoding.UTF8.GetBytes(numAsStr)));
        Assert.Equal(expected, parsed);
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
}