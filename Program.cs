using ParallelAndDistributedSystems_SectionA;
using System.Diagnostics;

Console.WriteLine("Hello, World!");

// Basic generator for random int values in an array, added as a separate class to keep the main clean
var generator = new RandomGenerator(0, 600);

const int elements = 100000;
const int maxThreads = 2;


var unsortedArray = generator.Generate(elements);

await ParallelBubbleSort(unsortedArray, maxThreads);

Console.ReadLine();

// TODO: redo tests after adding merging
// THREADS - STOPWATCH TIME
// 2 - 5,4604041
// 3 - 2,3955435
// 4 - 1,3065858
// 6 - 0,5995377

// 16 (PC max) -  0,1688517
async Task ParallelBubbleSort(int[] arr, int maxThreads)
{

    // PLAN:
    // - merge all chunks when all tasks are done

    // TODO: discuss if splitting the array still counts as bubble sort and not bucket sort
    var watch = new Stopwatch();
    int chunkSize = arr.Length / maxThreads;

    var tasks = new List<Task>();

    watch.Start();
    Console.WriteLine("Starting...");
    for (int i = 0; i < maxThreads; i++)
    {
        int currentStart = i * chunkSize;

        int currentLimit = (i == maxThreads - 1) ? arr.Length - 1 : (currentStart + chunkSize - 1);

        tasks.Add(Task.Run(() => BubbleSort(arr, currentStart, currentLimit)));
    }

    await Task.WhenAll(tasks);
    watch.Stop();

    Console.WriteLine($"Finished execution with time '{watch.Elapsed.TotalSeconds}' seconds");


}

void BubbleSort(int[] arr, int start, int end)
{
    for (int i = start; i < end; i++)
    {
        for (int j = start; j < end - (i - start); j++)
        {
            if (arr[j] > arr[j + 1])
            {
                int temp = arr[j];
                arr[j] = arr[j + 1];
                arr[j + 1] = temp;
            }
        }
    }
}