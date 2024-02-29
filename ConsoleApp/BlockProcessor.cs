using System.Buffers;

namespace ConsoleApp;

public static class BlockProcessor
{
    public static Task<long> ProcessBlock(object? block)
    {
        if (block is null)
            return Task.FromResult(0L);

        var asBlock = (Block) block;
        if (asBlock.IsEmpty)
            return Task.FromResult(0L);

        // var lineTasks = new List<Task<int>>(10000);
        var charsAsSpan = asBlock.Chars.Span;

        var startOfNewLine = 0;
        var numLines = 0;
        for (var i = 0; i < charsAsSpan.Length; ++i)
        {
            if (charsAsSpan[i] == '\n' || i == charsAsSpan.Length - 1)
            {
                var line = asBlock.Chars.Slice(startOfNewLine, i - startOfNewLine);
                numLines += LineProcessor.ProcessLine(line);
                // lineTasks.Add(Task.Factory.StartNew(LineProcessor.ProcessLine, line));
                startOfNewLine = i + 1;
            }
        }

        return Task.FromResult((long) numLines);

        // return Task<long>.Factory.ContinueWhenAll(lineTasks.ToArray(), completedTasks =>
        // {
        //     return completedTasks.Sum(t => t.GetAwaiter().GetResult());
        // });
    }
}