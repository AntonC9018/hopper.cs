
using Chains;
using Core.FS;

namespace Core.Stats
{
    public class StatFileContainer : File
    {
        public Chain<StatEvent> chain;
        public StatFile file;

        public StatFileContainer(StatFile file)
        {
            this.chain = new Chain<StatEvent>();
            this.file = (StatFile)file.Copy();
        }

        public override File Copy()
        {
            var statNode = new StatFileContainer((StatFile)file.Copy());
            return statNode;
        }
    }
}