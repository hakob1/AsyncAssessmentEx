using System.Collections.Concurrent;

class ConcurrentDictionaryExample
{
    static async Task Main()
    {
        //Tasktodo
        var dict = new ConcurrentDictionary<string, int>();

        var tasks = new Task[10];
        for (int i = 0; i < 10; i++)
        {
            int workerId = i;
            tasks[i] = Task.Run(() =>
            {
                for (int j = 0; j < 100; j++)
                {
                    dict.AddOrUpdate("counter", 1, (key, oldVal) => oldVal + 1);
                }
                Console.WriteLine($"Worker {workerId} done.");
            });
        }

        await Task.WhenAll(tasks);
        Console.WriteLine($"Final counter value: {dict["counter"]}");
    }
}
