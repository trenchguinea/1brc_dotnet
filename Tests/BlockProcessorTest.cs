using ConsoleApp;

namespace Tests;

// public class BlockProcessorTest
// {
//     [Fact]
//     public void TestNullBlock()
//     {
//         Assert.Equal(0, BlockProcessor.ProcessBlock(null));
//     }
//     
//     [Fact]
//     public void TestEmptyBlock()
//     {
//         var b = new Block();
//         Assert.Equal(0, BlockProcessor.ProcessBlock(b));
//     }
//
//     [Fact]
//     public void TestOneLineBlock_EndsInNewLine()
//     {
//         var b = new Block("block\n", ReadOnlySpan<char>.Empty);
//         Assert.Equal(1, BlockProcessor.ProcessBlock(b));
//     }
//
//     [Fact]
//     public void TestOneLineBlock_DoesNotEndInNewLine()
//     {
//         var b = new Block("block", ReadOnlySpan<char>.Empty);
//         Assert.Equal(1, BlockProcessor.ProcessBlock(b));
//     }
//
//     [Fact]
//     public void TestBlock_10Lines_NoSupplemental()
//     {
//         var s = string.Join('\n', Enumerable.Range(1, 10).Select(i => $"line{i}")) + "\n";
//         var b = new Block(s, ReadOnlySpan<char>.Empty);
//
//         Assert.Equal(10, BlockProcessor.ProcessBlock(b));
//     }
//     
//     [Fact]
//     public void TestBlock_15Lines_WithSupplemental()
//     {
//         var s = string.Join('\n', Enumerable.Range(1, 10).Select(i => $"line{i}")) + "\n";
//         var s2 = string.Join('\n', Enumerable.Range(11, 5).Select(i => $"line{i}")) + "\n";
//         var b = new Block(s, s2);
//
//         Assert.Equal(15, BlockProcessor.ProcessBlock(b));
//     }
//
// }