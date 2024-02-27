using ConsoleApp;

namespace Tests;

public class BlockTest
{
    [Fact]
    public void EmptyBlock()
    {
        var b = new Block();
        Assert.True(b.IsEmpty);
        Assert.Equal(0, b.Length);
        Assert.Equal(0, b.Chars.Length);
    }

    [Fact]
    public void EmptySupplemental()
    {
        const string initial = "initial";
        
        var b = new Block(initial, ReadOnlySpan<char>.Empty);
        Assert.False(b.IsEmpty);
        Assert.Equal(initial.Length, b.Length);
        Assert.Equal(initial, b.Chars.ToString());
    }

    [Fact]
    public void NonEmptySupplemental()
    {
        const string initial = "initial";
        const string supplemental = "supplemental";

        var b = new Block(initial, supplemental);
        Assert.False(b.IsEmpty);
        Assert.Equal(initial.Length + supplemental.Length, b.Length);
        Assert.Equal(initial + supplemental, b.Chars.ToString());
    }
}