using ConsoleApp;

namespace Tests;

public class BlockReaderTest
{
    private const int BufferSize = 64 * 1024;
    
    [Fact]
    public void TestEmptyReturnsEmptyBlock()
    {
        using var file = File.OpenText("resources/Empty.txt");
        var reader = new BlockReader(file, BufferSize);
        var block = reader.ReadNextBlock();
        
        Assert.True(block.IsEmpty);
    }

    [Fact]
    public void TestSmallReturnsAllChars()
    {
        using var file = File.OpenText("resources/Small.txt");
        var contents = file.ReadToEnd();

        using var file2 = File.OpenText("resources/Small.txt");
        var reader = new BlockReader(file2, BufferSize);
        var block = reader.ReadNextBlock();

        Assert.Equal(contents.Length, block.Length);
        Assert.Equal(contents, block.Chars.ToString());
    }
    
    [Fact]
    public void TestExactly64K_ReturnsAllChars()
    {
        using var file = File.OpenText("resources/Exactly64k.txt");
        var contents = file.ReadToEnd();

        using var file2 = File.OpenText("resources/Exactly64k.txt");
        var reader = new BlockReader(file2, BufferSize);
        var block1 = reader.ReadNextBlock();
        var block2 = reader.ReadNextBlock();

        Assert.Equal(contents.Length, block1.Length);
        Assert.Equal(contents, block1.Chars.ToString());
        Assert.True(block2.IsEmpty);
    }

    [Fact]
    public void TestExactly64KWhenEndsInNewLine_ReturnsAllChars()
    {
        using var file = File.OpenText("resources/Exactly64k_EndsInNewLine.txt");
        var contents = file.ReadToEnd();

        using var file2 = File.OpenText("resources/Exactly64k_EndsInNewLine.txt");
        var reader = new BlockReader(file2, BufferSize);
        var block1 = reader.ReadNextBlock();
        var block2 = reader.ReadNextBlock();

        Assert.Equal(contents.Length, block1.Length);
        Assert.Equal(contents, block1.Chars.ToString());
        Assert.True(block2.IsEmpty);
    }

    [Fact]
    public void TestGreaterThan64KReturnsAllChars()
    {
        using var file = File.OpenText("resources/BarelyGreaterThan64k.txt");
        var contents = file.ReadToEnd();

        using var file2 = File.OpenText("resources/BarelyGreaterThan64k.txt");
        var reader = new BlockReader(file2, BufferSize);
        var block1 = reader.ReadNextBlock();
        var block2 = reader.ReadNextBlock();

        Assert.Equal(contents.Length, block1.Length);
        Assert.Equal(contents, block1.Chars.ToString());
        Assert.True(block2.IsEmpty);
    }

    [Fact]
    public void TestGreaterThan128KReturnsAllChars()
    {
        using var file = File.OpenText("resources/BarelyGreaterThan128k.txt");
        var contents = file.ReadToEnd();

        using var file2 = File.OpenText("resources/BarelyGreaterThan128k.txt");
        var reader = new BlockReader(file2, BufferSize);
        var block1 = reader.ReadNextBlock();
        var block2 = reader.ReadNextBlock();
        var block3 = reader.ReadNextBlock();

        var blockStr = block1.Chars.ToString() + block2.Chars;

        Assert.Equal(contents.Length, block1.Length + block2.Length);
        Assert.Equal(contents, blockStr);
        Assert.True(block3.IsEmpty);
    }

}