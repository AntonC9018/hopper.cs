using System.Runtime.Serialization;
using Hopper.Core.Behaviors.Basic;
using Hopper.Core.Items;
using Hopper.Core.Retouchers;

namespace Hopper.Core
{
    [DataContract]
    public class Player : Entity
    {
        public override bool IsPlayer => true;

        public static EntityFactory<Player> CreateFactory()
        {
            return new EntityFactory<Player>()
                .AddBehavior<Acting>(new Acting.Config(Algos.SimpleAlgo, null))
                .AddBehavior<Moving>()
                .AddBehavior<Displaceable>()
                // .AddBehavior<Controllable>() // needs to be reconfigured
                .AddBehavior<Attackable>()
                .AddBehavior<Damageable>()
                .AddBehavior<Attacking>()
                .AddBehavior<Digging>()
                .AddBehavior<Pushable>()
                .Retouch(Skip.EmptyAttack)
                .Retouch(Skip.EmptyDig)
                .Retouch(Equip.OnDisplace)
                .Retouch(Reorient.OnActionSuccess)
                .AddInitListener(e => e.Inventory = new Inventory(e));
        }
    }
}