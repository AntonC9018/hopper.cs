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
                if (e.GetCellRelative(a.direction)?.GetEntityFromLayer(ExtendedLayer.BLOCK) == null)
                {
                    e.World.SpawnEntity(BombEntity.Factory, e.Pos + a.direction);
                }
                else
                {
                    e.World.SpawnEntity(BombEntity.Factory, e.Pos);
                }
                e.Inventory.Destroy(item);
            }
        );

        // adds a mapping to the controller
        public static ITinker tinker = new Tinker<TinkerData>(
            new ChainDefBuilder()
                .AddDef(Controllable.Chains[InputMapping.Weapon_0])
                .AddHandler((ev) => ev.SetAction(placeBombAction.Copy(), ev.actor.Orientation))
                .End().ToStatic()
        );

        public static CheckInventoryItem item = new CheckInventoryItem(tinker, Inventory.CounterSlot);
        public static PackedItem item_x3 = new PackedItem(item, 3);

    }
}