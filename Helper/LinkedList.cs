namespace MyLinkedList
{
    public class MyLinkedList<T>
    {
        public MyListNode<T> Head;

        public int Count
        {
            get
            {
                int i = 0;
                foreach (var node in this)
                    i++;
                return i;
            }
        }

        public void AddFront(T item)
        {
            var node = new MyListNode<T>(item);
            if (Head != null)
            {
                Head.AddBefore(node);
            }
            Head = node;
        }

        public void RemoveNode(MyListNode<T> node)
        {
            if (Head == node)
            {
                Head = node.Next;
            }
            node.RemoveSelf();
        }

        public System.Collections.Generic.IEnumerator<T> GetEnumerator()
        {
            var node = Head;
            while (node != null)
            {
                yield return node.Item;
                node = node.Next;
            }
        }

        public delegate int CompareFunc(T a, T b);

        // sorts in descending order
        public void Sort(CompareFunc compareFunc)
        {
            int blockSize = 1, blockCount;
            do
            {
                // Maintain two lists pointing to two blocks, left and right
                MyListNode<T> left = Head, right = Head, tail = null;
                Head = null; // Start a new list
                blockCount = 0;

                // Walk through entire list in blocks of size blockCount
                while (left != null)
                {
                    blockCount++;
                    // Advance right to start of next block, measure size of left list while doing so
                    int leftSize = 0, rightSize = blockSize;
                    for (; leftSize < blockSize && right != null; ++leftSize)
                        right = right.Next;

                    // Merge two list until their individual ends
                    bool leftEmpty = leftSize == 0, rightEmpty = rightSize == 0 || right == null;
                    while (!leftEmpty || !rightEmpty)
                    {
                        MyListNode<T> smaller;
                        // Using <= instead of < gives us sort stability
                        if (rightEmpty || (!leftEmpty && compareFunc(left.Item, right.Item) > 0))
                        {
                            smaller = left; left = left.Next; --leftSize;
                            leftEmpty = leftSize == 0;
                        }
                        else
                        {
                            smaller = right; right = right.Next; --rightSize;
                            rightEmpty = rightSize == 0 || right == null;
                        }

                        // Update new list
                        if (tail != null)
                            tail.Next = smaller;
                        else
                            Head = smaller;
                        tail = smaller;
                    }

                    // right now points to next block for left
                    left = right;
                }

                // terminate new list, take care of case when input list is null
                if (tail != null)
                    tail.Next = null;

                // Lg n iterations
                blockSize <<= 1;

            } while (blockCount > 1);

            // restore the double link
            if (Head != null)
            {
                Head.Prev = null;
                var current = Head;
                var next = Head.Next;

                while (next != null)
                {
                    next.Prev = current;
                    current = next;
                    next = next.Next;
                }
            }

        }
    }

    public class MyListNode<T>
    {
        public MyListNode<T> Next;
        public MyListNode<T> Prev;

        public T Item;

        public MyListNode(T item)
        {
            Item = item;
        }

        public void RemoveSelf()
        {
            if (Prev != null)
            {
                Prev.Next = Next;
            }
            if (Next != null)
            {
                Next.Prev = Prev;
            }
        }

        public void AddBefore(MyListNode<T> node)
        {
            if (Prev != null)
            {
                Prev.Next = node;
                node.Prev = Prev;
            }
            Prev = node;
            node.Next = this;
        }
    }
}
