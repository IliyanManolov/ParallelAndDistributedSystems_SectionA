using ParallelAndDistributedSystems_SectionA;

Console.WriteLine("Hello, World!");

// Basic generator for random int values in an array, added as a separate class to keep the main clean
var generator = new RandomGenerator(0, 200);

const int elements = 100000;

var unsortedArray = generator.Generate(elements);


void ParallelBubbleSort(int[] arr, int maxThreads)
{
    // PLAN:
    // - split array into different groups/chunks (number of threads !)
    //      - wont need communication
    // - bubble sort each chunk
    // - merge all chunks when all tasks are done


    // TODO: discuss if splitting the array still counts as bubble sort and not bucket sort
}