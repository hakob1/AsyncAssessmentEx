class Program
{
    private const string MutexName = "Global\\MyAsyncFileMutex_filingId";
    private const string FilePath = "shared_config.json";

    static async Task Main()
    {
        // Ensure the file exists before starting
        if (!File.Exists(FilePath))
        {
            string initialContent = "{ \"LastUpdated\": \"2024-12-18T12:00:00Z\", \"SettingA\": \"Default\", \"SettingB\": 0 }";
            await File.WriteAllTextAsync(FilePath, initialContent);
        }

        using (var mutex = new Mutex(false, MutexName))
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Attempting to acquire the mutex asynchronously...");

            // Acquire the mutex asynchronously by using Task.Run to avoid blocking the main thread.
            await Task.Run(mutex.WaitOne);

            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Mutex acquired by current process/thread asynchronously!");

            try
            {
                string content = await File.ReadAllTextAsync(FilePath);
                Console.WriteLine("Current file content:");
                Console.WriteLine(content);

                // Simulate
                string updatedContent = UpdateContent(content);

                // Simulate
                await Task.Delay(2000);

                // Write the updated content back to the file
                await File.WriteAllTextAsync(FilePath, updatedContent);

                Console.WriteLine("File updated successfully. New content:");
                Console.WriteLine(updatedContent);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during file update: {ex.Message}");
            }
            finally
            {
                // Release the mutex so another process can proceed
                mutex.ReleaseMutex();
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Mutex released.");
            }
        }

        Console.WriteLine("Press Enter to exit.");
        Console.ReadLine();
    }

    private static string UpdateContent(string json)
    {
        var now = DateTime.UtcNow.ToString("o");

        string settingBKey = "\"SettingB\": ";
        int startIndex = json.IndexOf(settingBKey);
        if (startIndex == -1) return json;

        startIndex += settingBKey.Length;
        int endIndex = json.IndexOfAny(new char[] { ',', '}' }, startIndex);
        if (endIndex == -1) return json;

        string oldValueStr = json.Substring(startIndex, endIndex - startIndex).Trim();
        if (int.TryParse(oldValueStr, out int oldValue))
        {
            int newValue = oldValue + 1;
            json = json.Remove(startIndex, oldValueStr.Length).Insert(startIndex, newValue.ToString());
        }

        // Update LastUpdated with the current UTC time
        string lastUpdatedKey = "\"LastUpdated\": \"";
        int luStart = json.IndexOf(lastUpdatedKey);
        if (luStart != -1)
        {
            luStart += lastUpdatedKey.Length;
            int luEnd = json.IndexOf('"', luStart);
            if (luEnd != -1)
            {
                json = json.Remove(luStart, luEnd - luStart).Insert(luStart, now);
            }
        }

        return json;
    }
}
