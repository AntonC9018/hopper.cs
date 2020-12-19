using Hopper.Core.Registries;

namespace Hopper.Core.Stats
{
    // these are either static or created uniquely for kinds
    public class DictStatPath : IStatPath<DictFile>
    {
        public string String { get; protected set; }

        public DictStatPath(string path)
        {
            this.String = path;
        }

        public DictFile Path(StatManager sm)
        {
            return sm.GetLazy<DictFile>(this);
        }

        public DictFile GetDefault(PatchArea patchArea)
        {
            // we know that at this point the stat has been initialized since it is
            // called per Repository in the startup function
            return patchArea.DefaultStats.statManager.GetUnsafe<DictFile>(String);
        }
    }
}