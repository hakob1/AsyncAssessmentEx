class AsyncFileReadExample
{
    private static readonly SemaphoreSlim _writeLock = new(1, 1);

    static async Task Main()
    {
        // Multiple files to read
        var filePaths = new string[]
        {
            "C:\\Users\\user\\source\\repos\\AsyncAssessment\\AsyncFileRead\\matchresults_1.txt",
            "C:\\Users\\user\\source\\repos\\AsyncAssessment\\AsyncFileRead\\matchresults_2.txt"
        };

        // Combined file path
        string combineFilePath = "C:\\Users\\user\\source\\repos\\AsyncAssessment\\AsyncFileRead\\combinefile.txt";

        try
        {
            if (!File.Exists(combineFilePath))
            {
                using var fs = File.Create(combineFilePath);
            }

            File.WriteAllText(combineFilePath, "");

            //Tasktodo
            var tasks = filePaths.Select(async filePath =>
            {
                string[] lines = await File.ReadAllLinesAsync(filePath);
                await WriteLinesAsync(combineFilePath, lines);
            });

            await Task.WhenAll(tasks);

            string[] combinedLines = await File.ReadAllLinesAsync(combineFilePath);
            foreach (var line in combinedLines)
            {
                Console.WriteLine(line);
            }

            Console.WriteLine("All files processed successfully.");
            Console.ReadKey();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    //Tasktodo
    static async Task WriteLinesAsync(string path, string[] lines)
    {
        await _writeLock.WaitAsync();
        try
        {
            using (var sw = new StreamWriter(path, append: true)) 
            {
                foreach (var line in lines)
                {
                    await sw.WriteLineAsync(line);
                }
            };
        }
        finally
        {
            _writeLock.Release();
        }
    }
}
