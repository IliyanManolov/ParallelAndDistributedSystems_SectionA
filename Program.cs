using ParallelAndDistributedSystems_SectionA;
using System.Diagnostics;

// Basic generator for random int values in an array, added as a separate class to keep the main clean
var generator = new RandomGenerator(0, 100_000);

const int elements = 10_000;
const int maxThreads = 16;


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
    

    // Use the max threads since each thread creates 1 chunk
    for (int i = 1; i < maxThreads; i++)
    {
        var start = 0;
        var middle = i * chunkSize;
        var end = middle + chunkSize < arr.Length ? middle + chunkSize : arr.Length;

        InPlaceMerge(arr, start, middle, end);
    }

    watch.Stop();
    Console.WriteLine($"Finished execution with time '{watch.Elapsed.TotalSeconds}' seconds");

    // For basic debugging
    //for (int i = 0; i < arr.Length - 1; i++)
    //{
    //    if (arr[i] > arr[i + 1])
    //    {
    //        Console.WriteLine($"UNSORTED AT INDEX {i} - value {arr[i]} > {arr[i + 1]}");
    //    }
    //}

    // Basic in-place merge
    // Intentionally not using temporary array and cloning due to slightly better performance because of smaller sizes
    // In case of a bigger array (e.g.
    void InPlaceMerge(int[] array, int start, int middle, int end)
    {
        var i = start;
        var j = middle;
        while (i < j && j < end)
        {
            if (array[i] <= array[j])
            {
                i++;
            }
            else
            {
                int value = array[j];
                int index = j;

                while (index != i)
                {
                    array[index] = array[index - 1];
                    index--;
                }
                array[i] = value;

                i++;
                j++;
                middle++;
            }
        }
    }

    // Basic bubble sort
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
}