using System.Collections.Generic;

namespace Hopper.Core.FS
{
    public class Directory : Node
    {
        public Dictionary<string, Node> nodes =
            new Dictionary<string, Node>();
    }
}
