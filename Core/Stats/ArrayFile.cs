using Core.FS;
using System.Collections.Generic;

namespace Core.Stats
{
    public class ArrayFile : StatFile
    {
        public List<int> content = new List<int>();

        public override void _Add(StatFile f, int sign)
        {
            // we assume it is the same type 
            var otherFile = (ArrayFile)f;
            var otherArray = otherFile.content;
            for (int i = 0; i < content.Count; i++)
            {
                content[i] += otherArray[i] * sign;
            }
        }

        public override File Copy()
        {
            var newFile = (ArrayFile)base.Copy();
            newFile.content = new List<int>(content);
            return newFile;
        }

        public int this[int index]
        {
            get => content[index];
            set => content[index] = value;
        }
    }
}