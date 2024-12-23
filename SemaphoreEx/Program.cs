class ForeachSemaphoreAsyncExample
{
    static async Task Main()
    {
        var urls = new List<string>
        {
            "https://www.microsoft.com",
            "https://www.github.com",
            "https://www.google.com",
            "https://www.stackoverflow.com",
            "https://www.wikipedia.org"
        };

        using var semaphore = new SemaphoreSlim(3);

        var tasks = new List<Task>();

        using var httpClient = new HttpClient();

        foreach (var url in urls)
        {
            await semaphore.WaitAsync();

            var task = ProcessUrlAsync(httpClient, url)
                .ContinueWith(t =>
                {
                    semaphore.Release();
                });

            tasks.Add(task);
        }

        await Task.WhenAll(tasks);

        Console.WriteLine("All URLs processed.");
    }

    static async Task ProcessUrlAsync(HttpClient httpClient, string url)
    {
        try
        {
            Console.WriteLine($"Fetching {url}...");
            string content = await httpClient.GetStringAsync(url);
            Console.WriteLine($"{url} fetched, length: {content.Length}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching {url}: {ex.Message}");
        }
    }
}
