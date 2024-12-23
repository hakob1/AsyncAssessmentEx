class AsyncEnumerationExample
{
    static async Task Main()
    {
        //Tasktodo
        await foreach (var number in GenerateNumbersAsync())
        {
            Console.WriteLine(number);
        }

        Console.WriteLine("All numbers received.");
    }

    //Tasktodo
    static async IAsyncEnumerable<int> GenerateNumbersAsync()
    {
        for (int i = 1; i <= 10; i++)
        {
            await Task.Delay(500);
            yield return i;
        }
    }
}
