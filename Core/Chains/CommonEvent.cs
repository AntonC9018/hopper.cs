using Hopper.Utils.Chains;
using Hopper.Utils.Vector;

namespace Hopper.Core
{
    public class ActorContext : ContextBase
    {
        public Entity actor;
    }
    public class StandartContext : ActorContext
    {
        public IntVector2 direction;
    }
}