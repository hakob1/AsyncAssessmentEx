using System.Threading.Channels;

class DeepChannelExample
{
    static async Task Main()
    {
        //Tasktodo
        // Create a bounded channel with a capacity of 20 items.
        var channel = Channel.CreateBounded<int>(new BoundedChannelOptions(20)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleWriter = false,
            SingleReader = false
        });

        var cts = new CancellationTokenSource();

        // Start multiple producers
        var producerCount = 3;
        var producers = new Task[producerCount];
        for (int i = 0; i < producerCount; i++)
        {
            int producerId = i;
            producers[i] = Task.Run(() => SomeProducerFunction(channel.Writer, producerId, cts.Token));
        }

        // Start multiple consumers
        var consumerCount = 2;
        var consumers = new Task[consumerCount];
        for (int c = 0; c < consumerCount; c++)
        {
            int consumerId = c;
            consumers[c] = Task.Run(() => SomeConsumerFunction(channel.Reader, consumerId, cts));
        }

        try
        {
            await Task.WhenAll(producers);
            // Once all producers are done writing, complete the writer
            channel.Writer.TryComplete();
        }
        catch (Exception ex)
        {
            // If a producer fails, mark the channel as completed with an error
            channel.Writer.TryComplete(ex);
        }

        // Wait for consumers to drain the channel
        await Task.WhenAll(consumers);

        Console.WriteLine("All done. Press any key to exit.");
        Console.ReadKey();
    }

    //Tasktodo
    static async Task SomeProducerFunction(ChannelWriter<int> writer, int producerId, CancellationToken token)
    {
        try
        {
            var rand = new Random(producerId + Environment.TickCount);
            for (int i = 0; i < 50; i++)
            {
                token.ThrowIfCancellationRequested();
                int value = rand.Next(1, 100);

                await writer.WriteAsync(value, token);

                Console.WriteLine($"Producer {producerId} produced {value}");
                // Simulate
                await Task.Delay(rand.Next(50, 200), token);
            }

            Console.WriteLine($"Producer {producerId} completed.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Producer {producerId} encountered an error: {ex.Message}");
            writer.TryComplete(ex);
        }
    }

    //Tasktodo
    static async Task SomeConsumerFunction(ChannelReader<int> reader, int consumerId, CancellationTokenSource cts)
    {
        try
        {
            await foreach (var item in reader.ReadAllAsync())
            {
                // Process
                Console.WriteLine($"Consumer {consumerId} processing {item}");

                // Simulate
                await Task.Delay(100);

                // Special condition
                if (item > 95)
                {
                    Console.WriteLine($"Consumer {consumerId} saw 42 - signaling cancellation!");
                    cts.Cancel();
                }

                if (cts.Token.IsCancellationRequested)
                    break;
            }

            Console.WriteLine($"Consumer {consumerId} completed reading.");
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine($"Consumer {consumerId} canceled.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Consumer {consumerId} error: {ex.Message}");
        }
    }
}
