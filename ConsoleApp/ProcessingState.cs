using Microsoft.Win32.SafeHandles;

namespace ConsoleApp;

public record struct ProcessingState(int ExpectedCityCount, SafeFileHandle FileHandle, Partition Partition);