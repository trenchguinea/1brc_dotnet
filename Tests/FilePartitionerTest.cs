using ConsoleApp;

namespace Tests;

public class FilePartitionerTest
{
    private const int BufferSize = 64 * 1024;
    
    [Fact]
    public void TestEmptyReturnsNoPartitions()
    {
        using var file = File.OpenRead("resources/Empty.txt");
        var partitions = FilePartitioner.PartitionFile(file, BufferSize);
        Assert.Empty(partitions);
    }

    [Fact]
    public void TestSmall_ReturnsSinglePartition()
    {
        using var file = File.OpenRead("resources/Small.txt");

        var partitions = FilePartitioner.PartitionFile(file, BufferSize);
        var partition = Assert.Single(partitions);
        
        Assert.Equal(0, partition.Pos);
        Assert.Equal(187, partition.Length);
    }
    
    [Fact]
    public void TestExactly64K_ReturnsSinglePartition()
    {
        using var file = File.OpenRead("resources/Exactly64k.txt");

        var partitions = FilePartitioner.PartitionFile(file, BufferSize);
        var partition = Assert.Single(partitions);
        
        Assert.Equal(0, partition.Pos);
        Assert.Equal(65536, partition.Length);
    }

    [Fact]
    public void TestExactly64KWhenEndsInNewLine_ReturnsSinglePartition()
    {
        using var file = File.OpenRead("resources/Exactly64k_EndsInNewLine.txt");

        var partitions = FilePartitioner.PartitionFile(file, BufferSize);
        var partition = Assert.Single(partitions);
        
        Assert.Equal(0, partition.Pos);
        Assert.Equal(65536, partition.Length);
    }

    [Fact]
    public void TestGreaterThan64KReturnsAllChars()
    {
        using var file = File.OpenRead("resources/BarelyGreaterThan64k.txt");

        var partitions = FilePartitioner.PartitionFile(file, BufferSize);
        var partition = Assert.Single(partitions);
        
        Assert.Equal(0, partition.Pos);
        Assert.Equal(65549, partition.Length);
    }

    [Fact]
    public void TestGreaterThan128KReturnsAllChars()
    {
        using var file = File.OpenRead("resources/BarelyGreaterThan128k.txt");
    
        var partitions = FilePartitioner.PartitionFile(file, BufferSize);
        Assert.Equal(2, partitions.Count);

        var partition1 = partitions[0];
        Assert.Equal(0, partition1.Pos);
        Assert.Equal(65550, partition1.Length);

        var partition2 = partitions[1];
        Assert.Equal(65550, partition2.Pos);
        Assert.Equal(65529, partition2.Length);
    }

}