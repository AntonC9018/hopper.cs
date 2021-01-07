using Hopper.Core;
using Hopper.Core.Behaviors.Basic;
using Hopper.Core.Chains;
using Hopper.Core.Items;

namespace Hopper.Test_Content.Explosion
{
    public static class Bomb
    {
        public static readonly Tinker<TinkerData> Tinker = new Tinker<TinkerData>(
            new ChainDefBuilder()
                .AddDef(Controllable.Chains[InputMapping.Weapon_0])
                .AddHandler(ev =>
                {
                    ev.action = placeBombAction;
                    ev.direction = ev.actor.Orientation;
                })
                .End().ToStatic()
        );
        public static readonly CheckInventoryItem Item = new CheckInventoryItem(
            new ItemMetadata("Bomb"), Tinker, BasicSlots.Counter);
        public static readonly PackedItem Item_x3 = new PackedItem(
            new ItemMetadata("Bomb_x3"), Item, 3);

        public static DirectedAction placeBombAction => Action.CreateSimple(
            (e, direction) =>
            {
                var targetPos = e.Pos;
                if (e.HasBlockRelative(direction) == false)
                {
                    targetPos += direction;
                }

                // Place the bomb in the world only after the reals move
                // This is maybe too overcomplicated, I'm not sure if we need this.
                // System.Action activate = e.World.SpawnHangingEntity(BombEntityFactory, targetPos);
                // e.World.m_state.OncePhaseStarts(Phase.REAL, activate);

                e.World.SpawnEntity(BombEntity.Factory, targetPos);

                e.Inventory.Destroy(Item);
            }
        );
    }
}