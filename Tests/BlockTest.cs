using ConsoleApp;

namespace Tests;

public class BlockTest
{
    [Fact]
    public void EmptyBlock()
    {
        var b = Block.Empty;
        Assert.True(b.IsEmpty);
        Assert.Equal(0, b.Chars.Length);
    }

    [Fact]
    public void EmptySupplemental()
    {
        const string initial = "initial";
        
        var b = new Block(initial.AsSpan(), ReadOnlySpan<char>.Empty);
        Assert.Equal(initial, b.Chars.ToString());
    }

    [Fact]
    public void NonEmptySupplemental()
    {
        const string initial = "initial";
        const string supplemental = "supplemental";

        var b = new Block(initial.AsSpan(), supplemental.AsSpan());
        Assert.Equal(initial + supplemental, b.Chars.ToString());
    }
}