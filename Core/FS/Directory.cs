using System.Collections.Generic;

namespace Core.FS
{
    public class Directory : Node
    {
        public Dictionary<string, Node> nodes =
            new Dictionary<string, Node>();

        public virtual File GetFile(string name)
        {
            return (File)nodes[name];
        }

        public void AddFile(string name, File file)
        {
            nodes.Add(name, file);
        }

        public void AddDirectory(string name, Directory directory)
        {
            nodes.Add(name, directory);
        }
    }
}
