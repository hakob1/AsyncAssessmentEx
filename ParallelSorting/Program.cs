class ParallelSortingExample
{
    static async Task Main()
    {
        int[] data = GenerateRandomArray(50000);

        //Tasktodo
        int midpoint = data.Length / 2;
        int[] firstHalf = data[..midpoint];
        int[] secondHalf = data[midpoint..];

        Task sortFirst = Task.Run(() => Array.Sort(firstHalf));
        Task sortSecond = Task.Run(() => Array.Sort(secondHalf));

        await Task.WhenAll(sortFirst, sortSecond);

        int[] merged = MergeTwoSortedArrays(firstHalf, secondHalf);

        Console.WriteLine("First 20 elements of the sorted array:");
        Console.WriteLine(string.Join(", ", merged.Take(20)));
    }

    static int[] GenerateRandomArray(int size)
    {
        var rand = new Random();
        int[] arr = new int[size];
        for (int i = 0; i < size; i++)
            arr[i] = rand.Next(1, 1_000_000);
        return arr;
    }

    //Tasktodo
    static int[] MergeTwoSortedArrays(int[] arr1, int[] arr2)
    {
        int[] merged = new int[arr1.Length + arr2.Length];
        int i = 0, j = 0, k = 0;

        while (i < arr1.Length && j < arr2.Length)
        {
            if (arr1[i] <= arr2[j])
                merged[k++] = arr1[i++];
            else
                merged[k++] = arr2[j++];
        }
        while (i < arr1.Length) merged[k++] = arr1[i++];
        while (j < arr2.Length) merged[k++] = arr2[j++];
        return merged;
    }
}
