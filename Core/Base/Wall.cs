using System.Runtime.Serialization;

namespace Hopper.Core
{
    [DataContract]
    public class Wall : Entity
    {
        public override Layer Layer => Layer.WALL;
    }
}