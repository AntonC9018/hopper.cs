using System.Runtime.Serialization;
using Hopper.Core.Components.Basic;
using Hopper.Core.Items;
using Hopper.Core.Retouchers;

namespace Hopper.Core
{
    [DataContract]
    public class Player : Entity
    {
        public override Faction Faction => Faction.Player;

        public static EntityFactory<Player> CreateFactory()
        {
            return new EntityFactory<Player>()
                .AddBehavior(Acting.Preset(new Acting.Config(Algos.SimpleAlgo, null)))
                .AddBehavior(Moving.Preset)
                .AddBehavior(Displaceable.DefaultPreset)
                // .AddBehavior(Controllable.Preset) // needs to be reconfigured
                .AddBehavior(Attackable.DefaultPreset)
                .AddBehavior(Damageable.Preset)
                .AddBehavior(Attacking.Preset)
                .AddBehavior(Digging.Preset)
                .AddBehavior(Pushable.Preset)
                .Retouch(Skip.EmptyAttack)
                .Retouch(Skip.EmptyDig)
                .Retouch(Equip.OnDisplace)
                .Retouch(Reorient.OnActionSuccess)
                .AddInitListener(e => e.Inventory = new Inventory(e));
        }
    }
}