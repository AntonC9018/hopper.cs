using Hopper.Utils.FS;
using System.Collections.Generic;

namespace Hopper.Core.Stats
{
    public class ArrayFile : File, IAddableWith<ArrayFile>
    {
        public int[] content;

        public void _Add(ArrayFile otherFile, int sign)
        {
            var otherArray = otherFile.content;
            for (int i = 0; i < otherArray.Length; i++)
            {
                content[i] += otherArray[i] * sign;
            }
        }

        public override File Copy()
        {
            var newFile = (ArrayFile)base.Copy();
            newFile.content = new int[content.Length];
            content.CopyTo(newFile.content, 0);
            return newFile;
        }

        public int this[int id]
        {
            get => content[id];
            set => content[id] = value;
        }
    }
}