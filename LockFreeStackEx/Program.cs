class LockFreeStackExample
{
    static async Task Main(string[] args)
    {
        var stack = new LockFreeStack<int>();
        int numTasks = 10;
        int operationsPerTask = 1000;

        var pushTasks = new Task[numTasks];
        for (int i = 0; i < numTasks; i++)
        {
            int taskId = i;
            pushTasks[i] = Task.Run(() =>
            {
                for (int j = 0; j < operationsPerTask; j++)
                {
                    stack.Push(taskId * operationsPerTask + j);
                }
            });
        }

        await Task.WhenAll(pushTasks);
        Console.WriteLine($"Total items after push: {stack.Count}");

        var popTasks = new Task[numTasks];
        int totalPopped = 0;

        for (int i = 0; i < numTasks; i++)
        {
            popTasks[i] = Task.Run(() =>
            {
                int localPopped = 0;
                while (stack.TryPop(out int item))
                {
                    localPopped++;
                }
                Interlocked.Add(ref totalPopped, localPopped);
            });
        }

        await Task.WhenAll(popTasks);
        Console.WriteLine($"Total items popped: {totalPopped}");
        Console.WriteLine($"Final stack count: {stack.Count}");
    }
}
