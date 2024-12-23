using System.Reactive.Concurrency;
using System.Reactive.Linq;

class RxExample
{
    static async Task Main()
    {
        // Simulate a sensor generating a new reading every 100ms
        // Observable.Interval creates a sequence of longs: 0,1,2,... every specified interval.
        var sensorReadings = Observable.Interval(TimeSpan.FromMilliseconds(100))
                                       .Select(_ => GenerateSensorReading());

        // We'll apply several Rx operators:
        // 1. Filter: take only readings above a certain threshold.
        // 2. Debounce: if readings come too fast, ignore some (only the last within the debounce window).
        // 3. ObserveOn: move processing to a background thread for CPU-bound work.
        // 4. SelectMany with Task.Run to process in parallel.
        // 5. Finally, ObserveOn a main/UI thread for final action (simulated here).

        var threshold = 50;

        // Debounce: Wait 300ms of quiet before considering the value stable.
        var processedStream = sensorReadings
            .Where(value => value > threshold)
            .Debounce(TimeSpan.FromMilliseconds(300)) // only take the last value after a pause in activity
            .SelectMany(value =>
                Observable.FromAsync(() => ProcessReadingAsync(value))
            )
            // After processing is done, ensure final results are observed on the main thread.
            // In a real UI app, you'd use DispatcherScheduler or SynchronizationContext.Current.
            .ObserveOn(Scheduler.Immediate);

        // Subscribe to the observable
        var cts = new CancellationTokenSource();
        var subscription = processedStream.Subscribe(
            onNext: result => Console.WriteLine($"[Main Thread] Processed result: {result}"),
            onError: ex => Console.WriteLine($"Error: {ex.Message}"),
            onCompleted: () => Console.WriteLine("Sequence completed.")
        );

        // Let the simulation run for a few seconds
        await Task.Delay(5000);

        // Cancel the subscription
        subscription.Dispose();
        Console.WriteLine("Subscription disposed, no more events will be processed.");
    }

    static Random rand = new Random();

    // Simulate a random sensor reading
    static int GenerateSensorReading()
    {
        // Values range from 0 to 100
        return rand.Next(0, 101);
    }

    // Simulate CPU-bound processing of the reading
    static async Task<string> ProcessReadingAsync(int value)
    {
        // Simulate CPU-bound work by doing a Task.Delay (to mimic some complex computation)
        await Task.Delay(rand.Next(100, 500));
        return $"Value {value} processed at {DateTime.Now:HH:mm:ss.fff}";
    }
}
