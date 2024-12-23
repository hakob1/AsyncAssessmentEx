public class LockFreeQueue<T>
{
    private class Node
    {
        public T Value;
        public Node Next;
    }

    private Node _head;
    private Node _tail;

    public LockFreeQueue()
    {
        _head = _tail = new Node();
    }

    public void Enqueue(T item)
    {
        var newNode = new Node { Value = item };
        while (true)
        {
            Node tail = _tail;
            Node next = tail.Next;

            if (tail == _tail)
            {
                if (next == null)
                {
                    if (Interlocked.CompareExchange(ref tail.Next, newNode, null) == null)
                    {
                        Interlocked.CompareExchange(ref _tail, newNode, tail);
                        return;
                    }
                }
                else
                {
                    Interlocked.CompareExchange(ref _tail, next, tail);
                }
            }
        }
    }

    public bool TryDequeue(out T result)
    {
        while (true)
        {
            Node head = _head;
            Node tail = _tail;
            Node next = head.Next;

            if (head == _head)
            {
                if (head == tail)
                {
                    if (next == null)
                    {
                        result = default(T);
                        return false;
                    }
                    Interlocked.CompareExchange(ref _tail, next, tail);
                }
                else
                {
                    result = next.Value;
                    if (Interlocked.CompareExchange(ref _head, next, head) == head)
                    {
                        return true;
                    }
                }
            }
        }
    }

    public int Count
    {
        get
        {
            int count = 0;
            var current = _head.Next;
            while (current != null)
            {
                count++;
                current = current.Next;
            }
            return count;
        }
    }
}