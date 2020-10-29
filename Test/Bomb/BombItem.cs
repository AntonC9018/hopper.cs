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
                var targetPos = e.Pos;
                if (e.GetCellRelative(a.direction)?.GetEntityFromLayer(ExtendedLayer.BLOCK) == null)
                {
                    targetPos += a.direction;
                }

                // Place the bomb in the world only after the reals move
                // This is maybe too overcomplicated, I'm not sure if we need this.
                // System.Action activate = e.World.SpawnHangingEntity(BombEntity.Factory, targetPos);
                // e.World.m_state.OncePhaseStarts(Phase.REAL, activate);
                e.World.SpawnEntity(BombEntity.Factory, targetPos);

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