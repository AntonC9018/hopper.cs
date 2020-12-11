using System.Collections.Generic;

namespace Hopper.Core.FS
{
    public class Directory : Node
    {
        public Dictionary<string, Node> nodes =
            new Dictionary<string, Node>();

        public void CopyDirectoryStructureFrom(Directory from)
        {
            foreach (var kvp in from.nodes)
            {
                if (kvp.Value is Directory)
                {
                    var subdir = new Directory();
                    this.nodes.Add(kvp.Key, subdir);
                    subdir.CopyDirectoryStructureFrom((Directory)kvp.Value);
                }
                else
                {
                    var copy = ((File)kvp.Value).Copy();
                    this.nodes.Add(kvp.Key, copy);
                }
            }
        }
    }
}
