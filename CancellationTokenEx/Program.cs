class CancellationExample
{
    static async Task Main()
    {
        //Tasktodo
        using var cancellationTokenSource = new CancellationTokenSource();

        var longTask = DoLongRunningCalculationAsync(cancellationTokenSource.Token);

        Console.WriteLine("Press 'c' to cancel...");
        Task cancelTask = Task.Run(() =>
        {
            if (Console.ReadKey(true).Key == ConsoleKey.C)
            {
                cancellationTokenSource.Cancel();
            }
        });

        try
        {
            await longTask;
            Console.WriteLine("Calculation completed successfully.");
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Calculation was canceled.");
        }
    }
    //Tasktodo
    static async Task DoLongRunningCalculationAsync(CancellationToken token)
    {
        for (int i = 0; i < 10; i++)
        {
            token.ThrowIfCancellationRequested();
            await Task.Delay(1000, token);
            Console.WriteLine($"Step {i + 1} completed.");
        }
    }
}
