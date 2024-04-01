using System.Text;
using ConsoleApp;

namespace Tests;

public class MemoryMappedFilePartitionerTest
{
    private const int BufferSize = 64 * 1024;
    
    [Fact]
    public void TestEmptyReturnsNoPartitions()
    {
        var partitioner = new MemoryMappedFilePartitioner(BufferSize);
        var partitions = partitioner.PartitionFile("resources/Empty.txt");

        Assert.Equal(0, partitions.Length);
    }

    [Fact]
    public void TestSmallReturnsAllChars()
    {
        using var file = File.OpenRead("resources/Small.txt");
        var contents = new MemoryStream();
        file.CopyTo(contents);
        var contentsAsStr = Encoding.UTF8.GetString(contents.ToArray());

        var partitioner = new MemoryMappedFilePartitioner(BufferSize);
        var partitions = partitioner.PartitionFile("resources/Small.txt");
        
        Assert.Equal(1, partitions.Length);
        Assert.Equal(file.Length, partitions[0].Size);

        var blockBytes = new byte[partitions[0].Size];
        partitions[0].Accessor.ReadArray(0, blockBytes, 0, blockBytes.Length);
        var blockAsStr = Encoding.UTF8.GetString(blockBytes.ToArray());
        Assert.Equal(contentsAsStr, blockAsStr);
    }
    
    [Fact]
    public void TestExactly64K_ReturnsAllChars()
    {
        using var file = File.OpenRead("resources/Exactly64k.txt");
        var contents = new MemoryStream();
        file.CopyTo(contents);
        var contentsAsStr = Encoding.UTF8.GetString(contents.ToArray());
    
        var partitioner = new MemoryMappedFilePartitioner(BufferSize);
        var partitions = partitioner.PartitionFile("resources/Exactly64k.txt");

        Assert.Equal(1, partitions.Length);
        Assert.Equal(file.Length, partitions[0].Size);

        var blockBytes = new byte[partitions[0].Size];
        partitions[0].Accessor.ReadArray(0, blockBytes, 0, blockBytes.Length);
        var blockAsStr = Encoding.UTF8.GetString(blockBytes.ToArray());
        Assert.Equal(contentsAsStr, blockAsStr);
    }

    [Fact]
    public void TestExactly64KWhenEndsInNewLine_ReturnsAllChars()
    {
        using var file = File.OpenRead("resources/Exactly64k_EndsInNewLine.txt");
        var contents = new MemoryStream();
        file.CopyTo(contents);
        var contentsAsStr = Encoding.UTF8.GetString(contents.ToArray());
    
        var partitioner = new MemoryMappedFilePartitioner(BufferSize);
        var partitions = partitioner.PartitionFile("resources/Exactly64k_EndsInNewLine.txt");

        Assert.Equal(1, partitions.Length);
        Assert.Equal(file.Length, partitions[0].Size);

        var blockBytes = new byte[partitions[0].Size];
        partitions[0].Accessor.ReadArray(0, blockBytes, 0, blockBytes.Length);
        var blockAsStr = Encoding.UTF8.GetString(blockBytes.ToArray());
        Assert.Equal(contentsAsStr, blockAsStr);
    }

    [Fact]
    public void TestGreaterThan64KReturnsAllChars()
    {
        var partitioner = new MemoryMappedFilePartitioner(BufferSize);
        var partitions = partitioner.PartitionFile("resources/BarelyGreaterThan64k.txt");

        const int sizeOfFirstBlock = 65535;
        const int sizeOfSecondBlock = 14;
        const byte firstByteOfFirstBlock = 75; // K
        const byte firstByteOfSecondBlock = 77; // M
        
        Assert.Equal(2, partitions.Length);
        Assert.Equal(sizeOfFirstBlock, partitions[0].Size);
        Assert.Equal(sizeOfSecondBlock, partitions[1].Size);
        
        Assert.Equal(firstByteOfFirstBlock, partitions[0].Accessor.ReadByte(0));
        Assert.Equal(Constants.NewLine, partitions[0].Accessor.ReadByte(sizeOfFirstBlock-1));

        Assert.Equal(firstByteOfSecondBlock, partitions[1].Accessor.ReadByte(0));
        Assert.Equal(Constants.NewLine, partitions[1].Accessor.ReadByte(sizeOfSecondBlock-1));
    }

    [Fact]
    public void TestGreaterThan128KReturnsAllChars()
    {
        var partitioner = new MemoryMappedFilePartitioner(BufferSize);
        var partitions = partitioner.PartitionFile("resources/BarelyGreaterThan128k.txt");

        const int sizeOfFirstBlock = 65534;
        const int sizeOfSecondBlock = 65531;
        const int sizeOfThirdBlock = 14;
        const byte firstByteOfFirstBlock = 75; // K
        const byte firstByteOfSecondBlock = 87; // W
        const byte firstByteOfThirdBlock = 65; // A
        
        Assert.Equal(3, partitions.Length);

        Assert.Equal(sizeOfFirstBlock, partitions[0].Size);
        Assert.Equal(sizeOfSecondBlock, partitions[1].Size);
        Assert.Equal(sizeOfThirdBlock, partitions[2].Size);
        
        Assert.Equal(firstByteOfFirstBlock, partitions[0].Accessor.ReadByte(0));
        Assert.Equal(Constants.NewLine, partitions[0].Accessor.ReadByte(sizeOfFirstBlock-1));
        
        Assert.Equal(firstByteOfSecondBlock, partitions[1].Accessor.ReadByte(0));
        Assert.Equal(Constants.NewLine, partitions[1].Accessor.ReadByte(sizeOfSecondBlock-1));

        Assert.Equal(firstByteOfThirdBlock, partitions[2].Accessor.ReadByte(0));
        Assert.Equal(Constants.NewLine, partitions[2].Accessor.ReadByte(sizeOfThirdBlock-1)); 
    }
}