using Microsoft.Win32.SafeHandles;

namespace ConsoleApp;

public record struct ProcessingState(int ExpectedCityCount, Block Block);

public record struct ProcessingState2(int ExpectedCityCount, SafeFileHandle FileHandle, Partition Partition);