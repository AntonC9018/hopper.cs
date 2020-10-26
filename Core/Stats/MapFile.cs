using Core.FS;
using System.Collections.Generic;

namespace Core.Stats
{
    public abstract class MapFile : File, IAddableWith<MapFile>
    {
        private Dictionary<int, int> content = new Dictionary<int, int>();
        protected abstract MapFile DefaultFile { get; }

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
            get
            {
                if (!content.ContainsKey(id))
                {
                    foreach (var kvp in DefaultFile.content)
                    {
                        if (!content.ContainsKey(kvp.Key))
                        {
                            content.Add(kvp.Key, kvp.Value);
                        }
                    }
                }
                return content[id];
            }
            set => content[id] = value;
        }

        public void Add(int key, int val)
        {
            content.Add(key, val);
        }
    }
}