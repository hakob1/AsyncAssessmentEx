using System.Collections.Concurrent;

class ParallelSumWithMerging
{
    static async Task Main()
    {
        int size = 100_000_000;
        var numbers = new int[size];
        for (int i = 0; i < size; i++) numbers[i] = i + 1;

        // Number of partitions
        int partitions = Environment.ProcessorCount;

        // Partition the data
        var rangePartitions = Partitioner.Create(0, numbers.Length).GetPartitions(partitions);

        long[] partialSums = new long[partitions];

        var tasks = new Task[partitions];
        for (int i = 0; i < partitions; i++)
        {
            int partitionIndex = i;
            var enumerator = rangePartitions.ElementAt(partitionIndex);

            tasks[i] = Task.Run(() =>
            {
                long localSum = 0;
                while (enumerator.MoveNext())
                {
                    var range = enumerator.Current;
                    for (int j = range.Item1; j < range.Item2; j++)
                    {
                        localSum += numbers[j];
                    }
                }
                partialSums[partitionIndex] = localSum;
            });
        }

        await Task.WhenAll(tasks);

        // Merge partial sums
        long totalSum = partialSums.Sum();

        Console.WriteLine($"Total Sum: {totalSum}");

        long expectedSum = (long)size * (size + 1) / 2;
        Console.WriteLine($"Expected Sum: {expectedSum}");
        Console.WriteLine($"Sum Correct: {totalSum == expectedSum}");
    }
}
