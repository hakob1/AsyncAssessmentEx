class LockExample
{
    private static int _counter = 0;
    private static readonly Lock _lockObj = new();

    static async Task Main()
    {
        //increment the counter 1000 times in 10 tasks

        Task[] tasks = new Task[10];
        for (int i = 0; i < tasks.Length; i++)
        {
            tasks[i] = Task.Run(() =>
            {
                //Tasktodo
                for (int x = 0; x < 1000; x++)
                {
                    lock (_lockObj)
                    {
                        _counter++;
                    }
                }
            });
        }

        await Task.WhenAll(tasks);
        Console.WriteLine($"Final counter value: {_counter}");
    }
}
