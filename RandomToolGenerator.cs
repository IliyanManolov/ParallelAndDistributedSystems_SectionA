namespace ParallelAndDistributedSystems_SectionA;

public class RandomToolGenerator
{
    private readonly int _minValue = 1;
    private readonly int _maxValue = 100;

    public IList<Tool> Generate(int size)
    {
        var result = new List<Tool>(size);

        var random = new Random();
        for (int i = 0; i < size; i++)
        {
            result.Add(new Tool()
            {
                Type = random.Next(_minValue, _maxValue),
                Barcode = i
            });
        }

        return result;
    }
}