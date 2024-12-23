using System.Threading.Channels;

class ChannelProducerConsumerExample
{
    static async Task Main()
    {
        //Tasktodo
        var channel = Channel.CreateUnbounded<int>();

        var producer = Task.Run(async () =>
        {
            for (int i = 1; i <= 20; i++)
            {
                await channel.Writer.WriteAsync(i);
                await Task.Delay(100);
            }
            channel.Writer.Complete();
        });

        var consumers = new Task[3];
        for (int c = 0; c < consumers.Length; c++)
        {
            int consumerId = c;
            consumers[c] = Task.Run(async () =>
            {
                await foreach (var item in channel.Reader.ReadAllAsync())
                {
                    await Task.Delay(50);
                    Console.WriteLine($"Consumer {consumerId}: Processed {item}");
                }
            });
        }

        await producer;
        await Task.WhenAll(consumers);

        Console.WriteLine("All items processed.");
    }
}
