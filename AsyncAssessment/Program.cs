using System.Collections.Concurrent;

class Program
{
    static async Task Main(string[] args)
    {
        int[] data = GenerateRandomArray(100000);

        //Tasktodo
        using var cts = new CancellationTokenSource();

        Task cancelTask = Task.Run(() =>
        {
            Console.WriteLine("Press 'c' to cancel sorting...");
            if (Console.ReadKey(true).Key == ConsoleKey.C)
            {
                cts.Cancel();
            }
        });

        try
        {
            int[] sorted = await SortArrayAsync(data, cts.Token);
            Console.WriteLine("Sorting completed successfully.");
            Console.WriteLine("First 50 elements of the sorted array:");
            Console.WriteLine(string.Join(", ", sorted.Take(50)));
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Sorting was canceled.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred during sorting: " + ex.Message);
        }
    }

    private static int[] GenerateRandomArray(int size)
    {
        var rand = new Random();
        int[] arr = new int[size];
        for (int i = 0; i < size; i++)
        {
            arr[i] = rand.Next(1, 1_000_000);
        }
        return arr;
    }

    //Tasktodo
    private static async Task<int[]> SortArrayAsync(int[] input, CancellationToken cancellationToken)
    {
        int numberOfSegments = 8;
        int segmentSize = input.Length / numberOfSegments;
        var segments = new (int start, int length)[numberOfSegments];

        for (int i = 0; i < numberOfSegments; i++)
        {
            int start = i * segmentSize;
            int length = (i == numberOfSegments - 1) ? input.Length - start : segmentSize;
            segments[i] = (start, length);
        }

        var concurrentBag = new ConcurrentBag<int[]>();

        // Create and run tasks to sort each segment in parallel
        var sortTasks = segments.Select(segment => Task.Run(() =>
        {
            // Check for cancellation
            cancellationToken.ThrowIfCancellationRequested();

            // Extract the subarray
            int[] subArray = new int[segment.length];
            Array.Copy(input, segment.start, subArray, 0, segment.length);

            // Sort (can be synchronous, but this is running in a parallel task)
            Array.Sort(subArray);

            // Add to the concurrent collection
            concurrentBag.Add(subArray);
        }, cancellationToken)).ToArray();

        try
        {
            await Task.WhenAll(sortTasks);
        }
        catch (Exception)
        {
            // In case of an error, propagate it
            throw;
        }

        cancellationToken.ThrowIfCancellationRequested();

        var sortedSegments = concurrentBag.ToArray();

        Array.Sort(sortedSegments, (a, b) => a[0].CompareTo(b[0]));

        int[] result = sortedSegments[0];
        for (int i = 1; i < sortedSegments.Length; i++)
        {
            result = MergeTwoSortedArrays(result, sortedSegments[i], cancellationToken);
        }

        return result;
    }

    //Tasktodo
    private static int[] MergeTwoSortedArrays(int[] arr1, int[] arr2, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        int[] merged = new int[arr1.Length + arr2.Length];
        int i = 0, j = 0, k = 0;

        while (i < arr1.Length && j < arr2.Length)
        {
            token.ThrowIfCancellationRequested();
            if (arr1[i] <= arr2[j])
            {
                merged[k++] = arr1[i++];
            }
            else
            {
                merged[k++] = arr2[j++];
            }
        }

        while (i < arr1.Length)
        {
            token.ThrowIfCancellationRequested();
            merged[k++] = arr1[i++];
        }

        while (j < arr2.Length)
        {
            token.ThrowIfCancellationRequested();
            merged[k++] = arr2[j++];
        }

        return merged;
    }
}
