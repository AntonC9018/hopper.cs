namespace Hopper.Utils.MyLinkedList
{
    public class MyLinkedList<T>
    {
        public MyListNode<T> head;

        public void AddFront(T item)
        {
            var node = new MyListNode<T>(item);
            if (head != null)
            {
                head.AddBefore(node);
            }
            head = node;
        }

        public void RemoveNode(MyListNode<T> node)
        {
            if (head == node)
            {
                head = node.next;
            }
            node.RemoveSelf();
        }

        public System.Collections.Generic.IEnumerator<T> GetEnumerator()
        {
            var node = head;
            while (node != null)
            {
                yield return node.item;
                node = node.next;
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
                MyListNode<T> left = head, right = head, tail = null;
                head = null; // Start a new list
                blockCount = 0;

                // Walk through entire list in blocks of size blockCount
                while (left != null)
                {
                    blockCount++;
                    // Advance right to start of next block, measure size of left list while doing so
                    int leftSize = 0, rightSize = blockSize;
                    for (; leftSize < blockSize && right != null; ++leftSize)
                        right = right.next;

                    // Merge two list until their individual ends
                    bool leftEmpty = leftSize == 0, rightEmpty = rightSize == 0 || right == null;
                    while (!leftEmpty || !rightEmpty)
                    {
                        MyListNode<T> smaller;
                        // Using <= instead of < gives us sort stability
                        if (rightEmpty || (!leftEmpty && compareFunc(left.item, right.item) > 0))
                        {
                            smaller = left; left = left.next; --leftSize;
                            leftEmpty = leftSize == 0;
                        }
                        else
                        {
                            smaller = right; right = right.next; --rightSize;
                            rightEmpty = rightSize == 0 || right == null;
                        }

                        // Update new list
                        if (tail != null)
                            tail.next = smaller;
                        else
                            head = smaller;
                        tail = smaller;
                    }

                    // right now points to next block for left
                    left = right;
                }

                // terminate new list, take care of case when input list is null
                if (tail != null)
                    tail.next = null;

                // Lg n iterations
                blockSize <<= 1;

            } while (blockCount > 1);

            // restore the double link
            if (head != null)
            {
                head.prev = null;
                var current = head;
                var next = head.next;

                while (next != null)
                {
                    next.prev = current;
                    current = next;
                    next = next.next;
                }
            }
        }
    }

    public class MyListNode<T>
    {
        public MyListNode<T> next;
        public MyListNode<T> prev;
        public T item;

        public MyListNode(T item)
        {
            this.item = item;
        }

        public void RemoveSelf()
        {
            if (prev != null)
            {
                prev.next = next;
            }
            if (next != null)
            {
                next.prev = prev;
            }
        }

        public void AddBefore(MyListNode<T> node)
        {
            if (prev != null)
            {
                prev.next = node;
                node.prev = prev;
            }
            prev = node;
            node.next = this;
        }

        public override int GetHashCode()
        {
            return item.GetHashCode();
        }
    }
}
