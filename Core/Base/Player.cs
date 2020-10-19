using System.Runtime.Serialization;
using Core.Behaviors;
using Core.Items;

namespace Core
{
    [DataContract]
    public class Player : Entity
    {
        public override bool IsPlayer => true;
        public Player() : base()
        {
            var inventory = new Inventory(this);
            Inventory = inventory;
            // account for the weapon and the shovel slot
            inventory.AddContainer(Core.Items.Inventory.WeaponSlot, new CircularItemContainer(1));
            inventory.AddContainer(Core.Items.Inventory.ShovelSlot, new CircularItemContainer(1));
        }

        [DataMember] private int SomeSavedMember;

        public static EntityFactory<Player> CreateFactory() => new EntityFactory<Player>()
            .AddBehavior<Acting>(new Acting.Config(Algos.SimpleAlgo, null))
            .AddBehavior<Moving>()
            .AddBehavior<Displaceable>()
            // .AddBehavior<Controllable>() // needs to be reconfigured
            .AddBehavior<Attackable>()
            .AddBehavior<Damageable>()
            .AddBehavior<Attacking>()
            .Retouch(Core.Retouchers.Skip.EmptyAttack)
            .AddBehavior<Digging>()
            .Retouch(Core.Retouchers.Skip.EmptyDig)
            .Retouch(Core.Retouchers.Equip.OnDisplace);
    }
}