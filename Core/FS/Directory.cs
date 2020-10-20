using System.Collections.Generic;

namespace Core.FS
{
    public class Directory : Node
    {
        public Dictionary<string, Node> nodes =
            new Dictionary<string, Node>();
    }
}
