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
        var initial = Encoding.Default.GetBytes("initial\n");
        
        var b = new Block(initial.AsSpan(), ReadOnlySpan<byte>.Empty);
        Assert.Equal("initial\n", Encoding.Default.GetString(b.Bytes.ToArray()));
    }

    [Fact]
    public void EmptySupplemental_DoesNotEndInNewLine()
    {
        var initial = Encoding.Default.GetBytes("initial");
        
        var b = new Block(initial.AsSpan(), ReadOnlySpan<byte>.Empty);
        Assert.Equal("initial\n", Encoding.Default.GetString(b.Bytes.ToArray()));
    }

    [Fact]
    public void NonEmptySupplemental_EndsInNewLine()
    {
        var initial = Encoding.Default.GetBytes("initial");
        var supplemental = Encoding.Default.GetBytes("supplemental\n");

        var b = new Block(initial.AsSpan(), supplemental.AsSpan());
        Assert.Equal("initialsupplemental\n", Encoding.Default.GetString(b.Bytes.ToArray()));
    }
    
    [Fact]
    public void NonEmptySupplemental_DoesNotEndInNewLine()
    {
        var initial = Encoding.Default.GetBytes("initial");
        var supplemental = Encoding.Default.GetBytes("supplemental");

        var b = new Block(initial.AsSpan(), supplemental.AsSpan());
        Assert.Equal("initialsupplemental\n", Encoding.Default.GetString(b.Bytes.ToArray()));
    }
}