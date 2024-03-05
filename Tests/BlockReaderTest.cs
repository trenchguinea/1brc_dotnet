using System.Text;
using ConsoleApp;

namespace Tests;

public class BlockReaderTest
{
    private const int BufferSize = 64 * 1024;
    
    [Fact]
    public void TestEmptyReturnsEmptyBlock()
    {
        using var file = File.Open("resources/Empty.txt", FileMode.Open);
        var reader = new BlockReader(file, BufferSize);
        var block = reader.ReadNextBlock();
        
        Assert.True(block.IsEmpty);
    }

    [Fact]
    public void TestSmallReturnsAllChars()
    {
        using var file = File.Open("resources/Small.txt", FileMode.Open);
        var contents = new MemoryStream();
        file.CopyTo(contents);
        var contentsAsStr = Encoding.UTF8.GetString(contents.ToArray());

        file.Position = 0;

        var reader = new BlockReader(file, BufferSize);
        var block = reader.ReadNextBlock();
        var blockAsStr = Encoding.UTF8.GetString(block.Bytes.ToArray());

        Assert.Equal(contentsAsStr, blockAsStr);
    }
    
    [Fact]
    public void TestExactly64K_ReturnsAllChars()
    {
        using var file = File.Open("resources/Exactly64k.txt", FileMode.Open);
        var contents = new MemoryStream();
        file.CopyTo(contents);
        var contentsAsStr = Encoding.UTF8.GetString(contents.ToArray()) + "\n";

        file.Position = 0;

        var reader = new BlockReader(file, BufferSize);
        var block1 = reader.ReadNextBlock();
        var block2 = reader.ReadNextBlock();
        var block1AsStr = Encoding.UTF8.GetString(block1.Bytes.ToArray());

        Assert.Equal(contentsAsStr, block1AsStr);
        Assert.True(block2.IsEmpty);
    }

    [Fact]
    public void TestExactly64KWhenEndsInNewLine_ReturnsAllChars()
    {
        using var file = File.Open("resources/Exactly64k_EndsInNewLine.txt", FileMode.Open);
        var contents = new MemoryStream();
        file.CopyTo(contents);
        var contentsAsStr = Encoding.UTF8.GetString(contents.ToArray());

        file.Position = 0;

        var reader = new BlockReader(file, BufferSize);
        var block1 = reader.ReadNextBlock();
        var block2 = reader.ReadNextBlock();
        var block1AsStr = Encoding.UTF8.GetString(block1.Bytes.ToArray());

        Assert.Equal(contentsAsStr, block1AsStr);
        Assert.True(block2.IsEmpty);
    }

    [Fact]
    public void TestGreaterThan64KReturnsAllChars()
    {
        using var file = File.Open("resources/BarelyGreaterThan64k.txt", FileMode.Open);
        var contents = new MemoryStream();
        file.CopyTo(contents);
        var contentsAsStr = Encoding.UTF8.GetString(contents.ToArray());

        file.Position = 0;

        var reader = new BlockReader(file, BufferSize);
        var block1 = reader.ReadNextBlock();
        var block2 = reader.ReadNextBlock();
        var block1AsStr = Encoding.UTF8.GetString(block1.Bytes.ToArray());

        Assert.Equal(contentsAsStr, block1AsStr);
        Assert.True(block2.IsEmpty);
    }

    [Fact]
    public void TestGreaterThan128KReturnsAllChars()
    {
        using var file = File.Open("resources/BarelyGreaterThan128k.txt", FileMode.Open);
        var contents = new MemoryStream();
        file.CopyTo(contents);
        var contentsAsStr = Encoding.UTF8.GetString(contents.ToArray());

        file.Position = 0;

        var reader = new BlockReader(file, BufferSize);
        var block1 = reader.ReadNextBlock();
        var block2 = reader.ReadNextBlock();
        var block3 = reader.ReadNextBlock();

        var block1AsStr = Encoding.UTF8.GetString(block1.Bytes.ToArray());
        var block2AsStr = Encoding.UTF8.GetString(block2.Bytes.ToArray());

        Assert.Equal(contentsAsStr, block1AsStr + block2AsStr);
        Assert.True(block3.IsEmpty);
    }
}