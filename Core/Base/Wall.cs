using System.Runtime.Serialization;
using Core.Items;

namespace Core
{
    [DataContract]
    public class Wall : Entity
    {
        public override Layer Layer => Layer.WALL;
    }
}