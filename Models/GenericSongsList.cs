using System.Collections;

namespace PSI.Models
{
    public class GenericSongList<T> : ICollection<T>
        where T : notnull, PlaylistSong 
    {
        private class Node 
        {
            public Node(T t) => (Next, Data) = (null, t);
            public Node? Next { get; set; }
            public T Data { get; set; }
        }

        private Node? head;
        private Node? tail;
        private int count = 0;

        public int Count => count;
        public bool IsReadOnly => false;

        public void Add(T item)
        {
            Node newNode = new(item);

            if (head == null)
            {
                head = newNode;
                tail = newNode;
            }
            else
            {
                tail!.Next = newNode;
                tail = newNode;
            }
            count++;
        }

        public bool Remove(T item)
        {
            Node? current = head;
            Node? previous = null;

            while (current != null)
            {
                if (current.Data.Equals(item))
                {
                    if (previous == null)
                    {
                        head = current.Next;
                    }
                    else
                    {
                        previous.Next = current.Next;
                    }
                    count--;
                    return true;
                }
                previous = current;
                current = current.Next;
            }
            return false;
        }

        public void Clear()
        {
            head = null;
            count = 0;
        }

        public bool Contains(T item)
        {
            Node? current = head;
            while (current != null)
            {
                if (current.Data.Equals(item))
                {
                    return true;
                }
                current = current.Next;
            }
            return false;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            if (array.Length - arrayIndex < count)
                throw new ArgumentException("Array is too small");

            Node? current = head;
            while (current != null)
            {
                array[arrayIndex++] = current.Data;
                current = current.Next;
            }
        }

        public T? FindById(Guid id)
        {
            Node? current = head;
            while (current != null)
            {
                if (current.Data.SongId == id)
                {
                    return current.Data;
                }
                current = current.Next;
            }
            return null;
        }

        public bool ContainsId(Guid id)
        {
            Node? current = head;
            while (current != null)
            {
                if (current.Data.SongId == id)
                {
                    return true;
                }
                current = current.Next;
            }
            return false;
        }
        // istaisyta problema sortinimo
        public List<T> GetOrderedByVotes()
        {
            List<T> items = new List<T>();
            Node? current = head;
            while (current != null)
            {
                items.Add(current.Data);
                current = current.Next;
            }

            for (int i = 0; i < items.Count - 1; i++)
            {
                for (int j = 0; j < items.Count - i - 1; j++)
                {
                    if (items[j].Votes < items[j + 1].Votes)
                    {
                        T temp = items[j];
                        items[j] = items[j + 1];
                        items[j + 1] = temp;
                    }
                }
            }
            return items;
        }

        public IEnumerator<T> GetEnumerator()
        {
            Node? current = head;
            while (current != null)
            {
                yield return current.Data;
                current = current.Next;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}