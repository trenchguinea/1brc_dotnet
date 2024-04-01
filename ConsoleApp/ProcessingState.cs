namespace ConsoleApp;

public record struct ProcessingState(int ExpectedCityCount, Block Block);

public record struct ProcessingState2(int ExpectedCityCount, MemoryMappedBlock Block);