namespace ParallelAndDistributedSystems_SectionA;

public class RandomGenerator
{
    private readonly int _minValue;
    private readonly int _maxValue;

    public RandomGenerator(int minValue, int maxValue)
    {
        _minValue = minValue;
        _maxValue = maxValue;
    }

    public int[] Generate(int size)
    {
        var result = new int[size];

        var random = new Random();
        for(int i = 0; i < size; i++)
        {
            result[i] = random.Next(_minValue, _maxValue);
        }

        return result;
    }
}
