using Chains;
using Core;
using Core.Behaviors;
using Core.Items;

namespace Test
{
    public static class Bombing
    {
        // TODO: reserve slots
        public static SimpleAction placeBombAction = new SimpleAction(
            (e, a) =>
            {
                e.World.SpawnEntity(BombEntity.Factory, e.Pos + a.direction);
                e.Inventory.Destroy(item);
            }
        );

        // adds a mapping to the controller
        public static ITinker tinker = new Tinker<TinkerData>(
            new ChainDefBuilder()
                .AddDef(Controllable.Chains[InputMappings.Weapon_0])
                .AddHandler((ev) => ev.SetAction(placeBombAction.Copy(), ev.actor.Orientation))
                .End().ToStatic()
        );

        public static CheckInventoryItem item = new CheckInventoryItem(tinker, Inventory.EndlessSlot);
        public static MultiItem item_x3 = new MultiItem(item, 3);

    }
}