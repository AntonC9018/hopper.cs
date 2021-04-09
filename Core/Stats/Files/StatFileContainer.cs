
using Hopper.Utils.Chains;
using Hopper.Utils.FS;

namespace Hopper.Core.Stats
{
    public class StatFileContainer<T> : File where T : File
    {
        public Chain<StatContext<T>> chain;
        public T file;

        public StatFileContainer(T file)
        {
            this.chain = new Chain<StatContext<T>>();
            this.file = (T)file.Copy();
        }

        public override File Copy()
        {
            return new StatFileContainer<T>((T)file.Copy());
        }

        public T Retrieve()
        {
            var ev = new StatContext<T> { file = (T)file.Copy() };
            chain.Pass(ev);
            return ev.file;
        }
    }
}