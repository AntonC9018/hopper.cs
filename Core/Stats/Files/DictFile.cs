using Hopper.Core.FS;
using System.Collections.Generic;

namespace Hopper.Core.Stats
{
    public class DictFile : File, IAddableWith<DictFile>
    {
        public Dictionary<int, int> content;

        public void _Add(DictFile otherFile, int sign)
        {
            var otherDict = otherFile.content;
            foreach (int i in otherDict.Keys)
            {
                content[i] += otherDict[i] * sign;
            }
        }

        public override File Copy()
        {
            var newFile = (DictFile)base.Copy();
            newFile.content = new Dictionary<int, int>(content);
            return newFile;
        }

        public int this[int id]
        {
            get => content[id];
            set => content[id] = value;
        }
    }
}