using ParallelAndDistributedSystems_SectionA;

Console.WriteLine("Hello, World!");

// Basic generator for random int values in an array, added as a separate class to keep the main clean
var generator = new RandomGenerator(0, 200);

const int elements = 100000;

var unsortedArray = generator.Generate(elements);

