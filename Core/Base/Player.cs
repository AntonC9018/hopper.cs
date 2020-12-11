using System.Runtime.Serialization;
using Hopper.Core.Behaviors;
using Hopper.Core.Items;
using Hopper.Core.Retouchers;
using Hopper.Core.Stats.Basic;

namespace Hopper.Core
{
    [DataContract]
    public class Player : Entity
    {
        public override bool IsPlayer => true;

        public static EntityFactory<Player> CreateFactory(CoreRetouchers retouchers)
            => new EntityFactory<Player>()
                .AddBehavior<Acting>(new Acting.Config(Algos.SimpleAlgo, null))
                .AddBehavior<Moving>()
                .AddBehavior<Displaceable>()
                // .AddBehavior<Controllable>() // needs to be reconfigured
                .AddBehavior<Attackable>()
                .AddBehavior<Damageable>()
                .AddBehavior<Attacking>()
                .AddBehavior<Digging>()
                .AddBehavior<Pushable>()
                .Retouch(retouchers.Skip.EmptyAttack)
                .Retouch(retouchers.Skip.EmptyDig)
                .Retouch(retouchers.Equip.OnDisplace)
                .Retouch(retouchers.Reorient.OnActionSuccess)
                .AddInitListener(e => e.Inventory = new Inventory(e));
    }
}