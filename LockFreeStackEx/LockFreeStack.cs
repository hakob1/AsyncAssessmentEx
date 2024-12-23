public class LockFreeStack<T>
{
    private Node _head;

    private class Node
    {
        public T Value;
        public Node Next;
    }

    public void Push(T item)
    {
        var newNode = new Node { Value = item };
        do
        {
            newNode.Next = _head;
        } while (Interlocked.CompareExchange(ref _head, newNode, newNode.Next) != newNode.Next);
    }

    public bool TryPop(out T result)
    {
        result = default(T);
        Node oldHead;

        do
        {
            oldHead = _head;
            if (oldHead == null)
                return false;
        } while (Interlocked.CompareExchange(ref _head, oldHead.Next, oldHead) != oldHead);

        result = oldHead.Value;
        return true;
    }

    public int Count
    {
        get
        {
            int count = 0;
            var current = _head;
            while (current != null)
            {
                count++;
                current = current.Next;
            }
            return count;
        }
    }
}