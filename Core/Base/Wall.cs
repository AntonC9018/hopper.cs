using System.Runtime.Serialization;
using Hopper.Core.Items;

namespace Hopper.Core
{
    [DataContract]
    public class Wall : Entity
    {
        public override Layer Layer => Layer.WALL;
    }
}