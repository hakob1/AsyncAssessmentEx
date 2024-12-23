class ParallelExample
{
    static void Main()
    {
        int n = 10_000_000;
        int[] data = new int[n];

        for (int i = 0; i < n; i++)
            data[i] = i;

        long sum = 0;

        Parallel.For(0, n,
            localInit: () => 0L,
            body: (i, state, localSum) =>
            {
                // Simulate
                localSum += Process(data[i]);
                return localSum;
            },
            localFinally: localSum =>
            {
                Interlocked.Add(ref sum, localSum);
            });

        Console.WriteLine($"Sum of processed data: {sum}");
    }

    static int Process(int value)
    {
        return value * value;
    }
}
