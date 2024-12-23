class LockFreeCounter
{
    private int _count = 0;

    public void Increment()
    {
        Interlocked.Increment(ref _count);
    }

    public void Decrement()
    {
        Interlocked.Decrement(ref _count);
    }

    public int GetValue()
    {
        return Volatile.Read(ref _count);
    }

    static async Task Main(string[] args)
    {
        var counter = new LockFreeCounter();
        int numTasks = 100;
        int incrementsPerTask = 1000;

        var tasks = new Task[numTasks * 2];
        for (int i = 0; i < numTasks; i++)
        {
            tasks[i] = Task.Run(() =>
            {
                for (int j = 0; j < incrementsPerTask; j++)
                {
                    counter.Increment();
                }
            });

            tasks[numTasks + i] = Task.Run(() =>
            {
                for (int j = 0; j < incrementsPerTask; j++)
                {
                    counter.Decrement();
                }
            });
        }

        await Task.WhenAll(tasks);

        Console.WriteLine($"Final Counter Value: {counter.GetValue()}");
    }
}
