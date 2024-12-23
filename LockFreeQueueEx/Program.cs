class LockFreeQueueExample
{
    static async Task Main(string[] args)
    {
        var queue = new LockFreeQueue<int>();
        int numTasks = 10;
        int operationsPerTask = 1000;

        var enqueueTasks = new Task[numTasks];
        for (int i = 0; i < numTasks; i++)
        {
            int taskId = i;
            enqueueTasks[i] = Task.Run(() =>
            {
                for (int j = 0; j < operationsPerTask; j++)
                {
                    queue.Enqueue(taskId * operationsPerTask + j);
                }
            });
        }

        await Task.WhenAll(enqueueTasks);
        Console.WriteLine($"Total items after enqueue: {queue.Count}");

        var dequeueTasks = new Task[numTasks];
        int totalDequeued = 0;

        for (int i = 0; i < numTasks; i++)
        {
            dequeueTasks[i] = Task.Run(() =>
            {
                int localDequeued = 0;
                while (queue.TryDequeue(out int item))
                {
                    localDequeued++;
                }
                Interlocked.Add(ref totalDequeued, localDequeued);
            });
        }

        await Task.WhenAll(dequeueTasks);
        Console.WriteLine($"Total items dequeued: {totalDequeued}");
        Console.WriteLine($"Final queue count: {queue.Count}");
    }
}
