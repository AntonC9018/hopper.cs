using System.Collections;
using System.Collections.Generic;

namespace Hopper.Utils
{
    public class DoubleList<T> : IEnumerable<T>
    {
        public List<T> _primaryBuffer;
        public List<T> _secondaryBuffer;
        public bool _isFiltering;

        public DoubleList()
        {
            _primaryBuffer = new List<T>();
            _secondaryBuffer = new List<T>();
            _isFiltering = false;
        }

        public DoubleList(List<T> buffer)
        {
            _primaryBuffer = buffer;
            _secondaryBuffer = new List<T>();
            _isFiltering = false;
        }


        public IEnumerator<T> GetEnumerator()
        {
            return (IEnumerator<T>) _primaryBuffer.GetEnumerator();
        }

        public IEnumerable<T> StartFiltering()
        {
            Assert.False(_isFiltering, $"Cannot initialize another filter while filtering the double list of {typeof(T).Name}");
            _secondaryBuffer.Clear();
            _isFiltering = true;
            foreach (var item in _primaryBuffer)
            {
                yield return item;
            }
            SwapBuffers();
            _isFiltering = false;
        }

        public void Filter(System.Predicate<T> predicate)
        {
            foreach (var item in this)
            {
                if (predicate(item))
                {
                    _secondaryBuffer.Add(item);
                }
            }
        }

        private void SwapBuffers()
        {
            var t = _primaryBuffer;
            _primaryBuffer   = _secondaryBuffer;
            _secondaryBuffer = t;
        }

        public void AddDirectly(T item)
        {
            Assert.False(_isFiltering, $"Cannot add to the primary buffer of {typeof(T).Name} directly while filtering");
            _primaryBuffer.Add(item);
        }

        public void AddMaybeWhileIterating(T item)
        {
            if (_isFiltering) _secondaryBuffer.Add(item);
            else              _primaryBuffer.Add(item);
        }

        public void AddToSecondaryBuffer(T item)
        {
            Assert.That(_isFiltering, $"Cannot add to the secondary buffer of {typeof(T).Name} directly while not filtering");
            _secondaryBuffer.Add(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator) GetEnumerator();
        }
    }
}