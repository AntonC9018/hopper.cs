using Core.FS;
using System.Collections.Generic;

namespace Core.Stats
{
    public class MapFile : File, IAddableWith<MapFile>
    {
        public Dictionary<int, int> content = new Dictionary<int, int>();

        public void _Add(MapFile otherFile, int sign)
        {
            // we assume it is the same type 
            var otherArray = otherFile.content;
            for (int i = 0; i < content.Count; i++)
            {
                content[i] += otherArray[i] * sign;
            }
        }

        public override File Copy()
        {
            var newFile = (MapFile)base.Copy();
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