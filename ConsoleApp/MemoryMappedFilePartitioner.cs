using System.Buffers;
using System.IO.MemoryMappedFiles;
using System.Runtime.CompilerServices;

namespace ConsoleApp;

public sealed class MemoryMappedFilePartitioner(int maxPartitionSize)
{
    public MemoryMappedBlock[] PartitionFile(string filePath)
    {
        var fileSize = new FileInfo(filePath).Length;
        if (fileSize == 0)
            return Array.Empty<MemoryMappedBlock>();

        using var file = MemoryMappedFile.CreateFromFile(filePath);

        long offset = 0;
        var numPartitions = fileSize / maxPartitionSize;

        // If there's a remainder of bytes, make sure we add another partition for them
        if (fileSize % maxPartitionSize > 0)
            numPartitions++;
        
        var partitions = new MemoryMappedBlock[numPartitions];

        for (var i = 0; i < numPartitions; ++i)
        {
            var blockSize = fileSize - offset > maxPartitionSize ? maxPartitionSize : fileSize - offset;
            var accessor = file.CreateViewAccessor(offset, blockSize);
            offset += blockSize;

            if (offset < fileSize)
            {
                while (accessor.ReadByte(blockSize - 1) != Constants.NewLine)
                {
                    offset--;
                    blockSize--;
                }
            }

            partitions[i] = new MemoryMappedBlock(accessor, (int) blockSize);
        }

        return partitions;
    }
}