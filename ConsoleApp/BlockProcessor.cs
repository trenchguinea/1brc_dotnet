namespace ConsoleApp;

public static class BlockProcessor
{
    public static int ProcessBlock(object? block)
    {
        if (block is null)
            return 0;

        var asBlock = (Block) block;
        if (asBlock.IsEmpty)
            return 0;

        var asSpan = asBlock.Chars.Span;

        var lineCnt = 0;
        for (var i = 0; i < asBlock.Length; ++i)
        {
            if (asSpan[i] == '\n' || i == asBlock.Length - 1)
            {
                lineCnt++;
            }
        }

        return lineCnt;
    }
}