using ParallelAndDistributedSystems_SectionA;
using System.Diagnostics;

// Basic generator for random int values in an array, added as a separate class to keep the main clean

const int elements = 100_000;
const int maxThreads = 6;

// Uncomment to test Task 1 (Parralel bubble sort)
var generator = new RandomGenerator(0, 100_000);
var unsortedArray = generator.Generate(elements);
await ParallelBubbleSort(unsortedArray, maxThreads);

//// Uncomment to test Task 2 (Finding tools)
//var toolGenerator = new RandomToolGenerator();
//var toolsList = toolGenerator.Generate(elements);
//await FindTools(toolsList, maxThreads);

Console.ReadLine();

async Task ParallelBubbleSort(int[] arr, int maxThreads)
{
    var watch = new Stopwatch();
    int chunkSize = (int)Math.Ceiling((double)arr.Length / maxThreads);

    var tasks = new List<Task>();

    watch.Start();
    Console.WriteLine($"Starting 'Parallel bubble sort' with threads - {maxThreads}...");
    for (int i = 0; i < maxThreads; i++)
    {
        int currentStart = i * chunkSize;

        var currentLimit = Math.Min(currentStart + chunkSize - 1, arr.Length - 1);

        tasks.Add(Task.Run(() => BubbleSort(arr, currentStart, currentLimit)));
    }

    await Task.WhenAll(tasks);

    // Use the max threads since each thread creates 1 chunk
    for (int i = 0; i < maxThreads; i++)
    {
        var start = 0;
        var middle = i * chunkSize;
        var end = middle + chunkSize < arr.Length ? middle + chunkSize : arr.Length;

        InPlaceMerge(arr, start, middle, end);
    }

    watch.Stop();
    Console.WriteLine($"Finished execution with time '{watch.Elapsed.TotalSeconds}' seconds");

    // For basic debugging
    for (int i = 0; i < arr.Length - 1; i++)
    {
        if (arr[i] > arr[i + 1])
        {
            Console.WriteLine($"UNSORTED AT INDEX {i} - value {arr[i]} > {arr[i + 1]}");
        }
    }

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
                var currentValue = array[j];
                var currentIndex = j;

                while (currentIndex != i)
                {
                    array[currentIndex] = array[currentIndex - 1];
                    currentIndex--;
                }
                array[i] = currentValue;

                i++;
                j++;
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


async Task FindTools(IList<Tool> tools, int maxThreads)
{
    var watch = new Stopwatch();
    int chunkSize = tools.Count / maxThreads;

    var tasks = new List<Task>();
    bool allToolsFound = false;

    // Type - Needed
    var neededTools = new Dictionary<int, int>
    {
        { 1, 30 },
        { 7, 15 },
        { 10, 8 }
    };

    // Type - Barcodes
    var resultDict = new Dictionary<int, IList<int>>
    {
        { 1, new List<int>() },
        { 7, new List<int>() },
        { 10, new List<int>() }
    };

    watch.Start();
    Console.WriteLine($"Starting 'Find tools' with threads - '{maxThreads}'....");
    for (int i = 0; i < maxThreads; i++)
    {
        int currentStart = i * chunkSize;

        int currentLimit = (i == maxThreads - 1) ? tools.Count - 1 : (currentStart + chunkSize - 1);

        tasks.Add(Task.Run(() => SearchChunk(tools, currentStart, currentLimit)));
    }

    await Task.WhenAll(tasks);
    watch.Stop();
    Console.WriteLine($"Finished execution with time '{watch.Elapsed.TotalSeconds}' seconds");

    foreach (var (type, barcodes) in resultDict)
    {
        Console.WriteLine($"Found '{barcodes.Count}' tools with type {type} - barcodes csv {string.Join(',', barcodes)}");
    }

    void SearchChunk(IList<Tool> tools, int start, int end)
    {
        for (int i = start; i < end; i++)
        {
            if (AreAllToolsFound())
                break;

            var currentTool = tools[i];
            
            if (neededTools.TryGetValue(currentTool.Type, out var neededCount))
            {
                lock (resultDict)
                {
                    var currentBarcodes = resultDict[currentTool.Type];

                    if (currentBarcodes.Count != neededCount)
                        currentBarcodes.Add(currentTool.Barcode);
                }
            }
        }
    }

    bool AreAllToolsFound()
    {
        lock(resultDict)
        {
            if (allToolsFound)
                return true;

            var result = true;
            foreach (var (id, nededCount) in neededTools)
            {
                if (resultDict[id].Count != nededCount)
                    result = false;
            }

            if (result)
                allToolsFound = true;

            return result;   
        }
    }
}