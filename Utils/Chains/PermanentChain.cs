using System.Collections.Generic;

namespace Hopper.Utils.Chains
{
    public class PermanentChain<Context>
    {
        public List<System.Func<Context, bool>> _primaryBuffer;
        public List<System.Func<Context, bool>> _doubleBuffer;

        public PermanentChain()
        {
            _primaryBuffer = new List<System.Func<Context, bool>>();
            _doubleBuffer = new List<System.Func<Context, bool>>();
        }

        public PermanentChain(PermanentChain<Context> other)
        {
            _primaryBuffer = new List<System.Func<Context, bool>>(other._primaryBuffer);
            _doubleBuffer = new List<System.Func<Context, bool>>();
        }

        public PermanentChain(List<System.Func<Context, bool>> buffer)
        {
            _primaryBuffer = new List<System.Func<Context, bool>>(buffer);
            _doubleBuffer = new List<System.Func<Context, bool>>();
        }

        public void PassAndFilter(Context ev)
        {
            _doubleBuffer.Clear();
            foreach (var handler in _primaryBuffer)
            {
                if (handler(ev))
                {
                    _doubleBuffer.Add(handler);
                }
            }
            SwapBuffers();
        }

        public void Add(System.Func<Context, bool> handler)
        {
            _primaryBuffer.Add(handler);
        }

        private void SwapBuffers()
        {
            var t = _primaryBuffer;
            _primaryBuffer = _doubleBuffer;
            _doubleBuffer = t;
        }
    }
}