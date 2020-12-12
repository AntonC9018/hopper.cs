using Hopper.Core.Chains;
using Hopper.Core;
using Hopper.Core.Behaviors.Basic;
using Hopper.Core.Items;
using Hopper.Core.Retouchers;

namespace Hopper.Test_Content.Explosion
{
    public class BombContent
    {
        // adds a mapping to the controller
        public ITinker tinker;
        public CheckInventoryItem item;
        public PackedItem item_x3;
        public EntityFactory<BombEntity> bombFactory;

        public BombContent(CoreRetouchers retouchers)
        {
            bombFactory = BombEntity.CreateFactory(retouchers);

            tinker = new Tinker<TinkerData>(
                new ChainDefBuilder()
                    .AddDef(Controllable.Chains[InputMapping.Weapon_0])
                    .AddHandler((ev) => ev.SetAction(placeBombAction.Copy(), ev.actor.Orientation))
                    .End().ToStatic()
            );

            item = new CheckInventoryItem(
                new ItemMetadata("Bomb"), tinker, Slot.Counter);

            item_x3 = new PackedItem(
                new ItemMetadata("Bomb_x3"), item, 3);
        }

        public void RegisterSelf(Registry registry)
        {
            Explosion.EventPath.Event.RegisterSelf(registry);
            Explosion.AtkSource.RegisterOn(registry);

            bombFactory.RegisterSelf(registry);
            tinker.RegisterSelf(registry);
            item.RegisterSelf(registry);
            item_x3.RegisterSelf(registry);
        }

        public SimpleAction placeBombAction => new SimpleAction(
            (e, a) =>
            {
                var targetPos = e.Pos;
                if (e.HasBlockRelative(a.direction) == false)
                {
                    targetPos += a.direction;
                }

                // Place the bomb in the world only after the reals move
                // This is maybe too overcomplicated, I'm not sure if we need this.
                // System.Action activate = e.World.SpawnHangingEntity(BombEntity.Factory, targetPos);
                // e.World.m_state.OncePhaseStarts(Phase.REAL, activate);

                // TODO: Save the bomb factory somewhere
                e.World.SpawnEntity(bombFactory, targetPos);

                e.Inventory.Destroy(item);
            }
        );

    }
}