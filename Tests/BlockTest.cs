using System.Text;
using ConsoleApp;

namespace Tests;

public class BlockTest
{
    [Fact]
    public void EmptyBlock()
    {
        var b = Block.Empty;
        Assert.True(b.IsEmpty);
        Assert.Equal(0, b.Bytes.Length);
    }

    [Fact]
    public void EmptySupplemental_EndsInNewLine()
    {
        var initial = "initial\n"u8.ToArray();
        
        var b = new Block(initial.AsSpan(), ReadOnlySpan<byte>.Empty);
        Assert.Equal("initial\n", Encoding.UTF8.GetString(b.Bytes.ToArray()));
    }

    [Fact]
    public void EmptySupplemental_DoesNotEndInNewLine()
    {
        var initial = "initial"u8.ToArray();
        
        var b = new Block(initial.AsSpan(), ReadOnlySpan<byte>.Empty);
        Assert.Equal("initial\n", Encoding.UTF8.GetString(b.Bytes.ToArray()));
    }

    [Fact]
    public void NonEmptySupplemental_EndsInNewLine()
    {
        var initial = "initial"u8.ToArray();
        var supplemental = "supplemental\n"u8.ToArray();

        var b = new Block(initial.AsSpan(), supplemental.AsSpan());
        Assert.Equal("initialsupplemental\n", Encoding.UTF8.GetString(b.Bytes.ToArray()));
    }
    
    [Fact]
    public void NonEmptySupplemental_DoesNotEndInNewLine()
    {
        var initial = "initial"u8.ToArray();
        var supplemental = "supplemental"u8.ToArray();

        var b = new Block(initial.AsSpan(), supplemental.AsSpan());
        Assert.Equal("initialsupplemental\n", Encoding.UTF8.GetString(b.Bytes.ToArray()));
    }
}