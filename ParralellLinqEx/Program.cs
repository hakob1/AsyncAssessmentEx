class PLINQExample
{
    static void Main()
    {
        var numbers = Enumerable.Range(2, 500_000); // half a million numbers

        // A CPU-bound prime-checking function
        bool IsPrime(int n)
        {
            if (n < 2) return false;
            int limit = (int)Math.Sqrt(n);
            for (int i = 2; i <= limit; i++)
            {
                if (n % i == 0) return false;
            }
            return true;
        }

        // Run the prime check in parallel
        var primeCount = numbers.AsParallel()
                                .WithDegreeOfParallelism(Environment.ProcessorCount)
                                .Count(IsPrime);

        Console.WriteLine($"Found {primeCount} prime numbers.");
    }
}
