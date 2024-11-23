using ParallelAndDistributedSystems_SectionA;
using System.Diagnostics;

// Basic generator for random int values in an array, added as a separate class to keep the main clean
var validTasks = new HashSet<int>() { 1, 2 };

while (true)
{
    WriteSeparator();
    Console.WriteLine("Task 2 has additional messages to the console that are commented out to keep it visually easier to follow.");
    WriteSeparator();

    Console.WriteLine("Please input which task you want to test.");
    Console.WriteLine("Task 1: Parallel bubble sort");
    Console.WriteLine("Task 2: Finding tools in parallel");
    Console.Write("Selected: ");
    
    var taskSelectionStr = Console.ReadLine();

    if (!int.TryParse(taskSelectionStr, out var taskSelected) || !validTasks.Contains(taskSelected))
    {
        Console.Clear();
        WriteSeparator();
        Console.WriteLine("Please selected a valid task to test.");
        WriteSeparator();
        continue;
    }

    Console.Write("Input number of threads you would like to use: ");

    var threadsStr = Console.ReadLine();

    if (!int.TryParse(threadsStr, out var threads) || threads <= 0)
    {
        Console.Clear();
        Console.WriteLine("Please input a valid number of threads > 0");
        continue;
    }

    Console.Write("Input number of elements you would like to use: ");

    var elementsStr = Console.ReadLine();

    if (!int.TryParse(elementsStr, out var elements) || elements <= 0)
    {
        Console.Clear();
        WriteSeparator();
        Console.WriteLine("Please input a valid number of elements > 0");
        WriteSeparator();
        continue;
    }

    switch (taskSelected)
    {
        case 1:
            var generator = new RandomGenerator(0, 100_000);
            var unsortedArray = generator.Generate(elements);
            await ParallelBubbleSort(unsortedArray, threads);
            WriteSeparator();
            break;

        case 2:
            var toolGenerator = new RandomToolGenerator();
            var toolsList = toolGenerator.Generate(elements);
            await FindTools(toolsList, threads);
            WriteSeparator();
            break;
    }

}

void WriteSeparator()
{
    Console.WriteLine("===============================================================");
}

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
    int chunkSize = (int)Math.Ceiling((double)tools.Count / maxThreads);

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

        var currentLimit = Math.Min(currentStart + chunkSize - 1, tools.Count - 1);

        tasks.Add(Task.Run(() => SearchChunk(tools, currentStart, currentLimit)));
    }

    await Task.WhenAll(tasks);

    watch.Stop();
    Console.WriteLine($"Finished execution with time '{watch.Elapsed.TotalSeconds}' seconds");

    foreach (var (type, barcodes) in resultDict)
    {
        Console.WriteLine(
            (barcodes.Count == neededTools[type] ? "SUCCESS. " : "INSUFFICIENT TOOLS FOUND. ") 
            + $"Found '{barcodes.Count}' tools with type '{type}' - barcodes csv '{string.Join(',', barcodes)}'");
    }

    void SearchChunk(IList<Tool> tools, int start, int end)
    {
        for (int i = start; i < end; i++)
        {
            // First lock before grabbing a tool
            if (AreAllToolsFound())
                break;

            //Console.WriteLine($"Worker {Thread.CurrentThread.ManagedThreadId} is searching for a tool");

            // Add a sleep to show that the execution is parallel
            Thread.Sleep(TimeSpan.FromMilliseconds(200));

            var currentTool = tools[i];
            //Console.WriteLine($"Worker {Thread.CurrentThread.ManagedThreadId} found a tool");

            if (neededTools.TryGetValue(currentTool.Type, out var neededCount))
            {
                // Lock the results dict when adding the current tool
                lock (resultDict)
                {
                    var currentBarcodes = resultDict[currentTool.Type];

                    // Check the count of tools found in case it was reached between the previous and current lock
                    if (currentBarcodes.Count != neededCount)
                    {
                        //Console.WriteLine($"Worker {Thread.CurrentThread.ManagedThreadId} added a tool of type '{currentTool.Type}' and barcode '{currentTool.Barcode}'");
                        currentBarcodes.Add(currentTool.Barcode);
                    }
                }
            }
            else
            {
                //Console.WriteLine($"Worker {Thread.CurrentThread.ManagedThreadId} found a tool of type '{currentTool.Type}' that is not needed....");
            }
        }
    }

    bool AreAllToolsFound()
    {
        // Lock the results dict when checking
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