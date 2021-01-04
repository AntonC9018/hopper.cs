
using Hopper.Utils.Chains;
using Hopper.Utils.FS;

namespace Hopper.Core.Stats
{
    public class StatFileContainer<T> : File where T : File
    {
        public Chain<StatEvent<T>> chain;
        public T file;

        public StatFileContainer(T file)
        {
            this.chain = new Chain<StatEvent<T>>();
            this.file = (T)file.Copy();
        }

        public override File Copy()
        {
            return new StatFileContainer<T>((T)file.Copy());
        }

        public T Retrieve()
        {
            var ev = new StatEvent<T> { file = (T)file.Copy() };
            chain.Pass(ev);
            return ev.file;
        }
    }
}