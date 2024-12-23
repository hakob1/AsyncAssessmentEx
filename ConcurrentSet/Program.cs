using System.Collections.Concurrent;

//Tasktodo
class ConcurrentSet
{
    private readonly ConcurrentDictionary<int, bool> _set = new();

    public bool Add(int item)
    {
        return _set.TryAdd(item, true);
    }

    public bool Remove(int item)
    {
        return _set.TryRemove(item, out _);
    }

    public bool Contains(int item)
    {
        return _set.ContainsKey(item);
    }
}

class Program
{
    static void Main()
    {
        var concurrentSet = new ConcurrentSet();

        Console.WriteLine(concurrentSet.Add(10)); // true, newly added
        Console.WriteLine(concurrentSet.Add(10)); // false, already exists
        Console.WriteLine(concurrentSet.Contains(10)); // true
        Console.WriteLine(concurrentSet.Remove(10)); // true, now removed
        Console.WriteLine(concurrentSet.Remove(10)); // false, no longer exists
    }
}
