using Hopper.Core;
using Hopper.Core.Components.Basic;
using Hopper.Core.Items;
using Hopper.Core.Retouchers;
using Hopper.Core.Stat;
using Hopper.Core.Targeting;
using Hopper.TestContent;
using Hopper.TestContent.SimpleMobs;
using Hopper.Utils;
using Hopper.Utils.Vector;

namespace Hopper.Mine
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Hopper.Core.Main.Init();
            Hopper.TestContent.Main.Init();

            EntityFactory entityFactory;
            EntityFactory itemFactory;

            {
                entityFactory = new EntityFactory();

                // AddComponents
                var transform = Transform.AddTo(entityFactory, Layer.REAL);
                var stats = Stats.AddTo(entityFactory, Registry.Global._defaultStats);
                var acting = Acting.AddTo(entityFactory, null, Algos.SimpleAlgo, Order.Player);
                var moving = Moving.AddTo(entityFactory);
                var displaceable = Displaceable.AddTo(entityFactory, ExtendedLayer.BLOCK);
                var inventory = Inventory.AddTo(entityFactory);
                var ticking = Ticking.AddTo(entityFactory);

                // InitComponents
                acting.DefaultPreset(entityFactory);
                moving.DefaultPreset();
                displaceable.DefaultPreset();
                ticking.DefaultPreset();

                // Retouch
                Equip.OnDisplaceHandlerWrapper.HookTo(entityFactory);
                entityFactory.InitInWorldFunc = 
                    t => { 
                        t.entity.GetInventory().InitInWorld(); 
                        t.entity.GetStats().Init(); 
                    };
            }

            {
                itemFactory = new EntityFactory();
                var transform = Transform.AddTo(itemFactory, Layer.ITEM);
            }

            World.Global = new World(3, 3);
            
            {
                // The id of an unregistered factory is 0:0
                var item = World.Global.SpawnEntity(itemFactory, IntVector2.Zero);
                Equippable.AddTo(item, null);

                var entity = World.Global.SpawnEntity(entityFactory, new IntVector2(1, 0));
                var inventory = entity.GetInventory();

                // Move onto the item
                var moveAction = Action.FromActivateable(Moving.Index).ToDirectedParticular(new IntVector2(-1, 0));
                var acting = entity.GetActing();
                acting.nextAction = moveAction;
                acting.Activate();
                
                inventory.Remove(item.typeId);

                // Reset acting flags (so that we can act again)
                // acting._flags = 0;

                // Move the entity back
                entity.GetTransform().ResetPositionInGrid(new IntVector2(1, 0));
                var slot = new Slot(false); slot.Id = new Identifier(3, 1);

                // Spawn two items this time
                var item1 = World.Global.SpawnEntity(itemFactory, IntVector2.Zero);
                Equippable.AddTo(item1, null);
                SlotComponent.AddTo(item1, slot.Id);

                var item2 = World.Global.SpawnEntity(itemFactory, IntVector2.Zero);
                Equippable.AddTo(item2, null);
                SlotComponent.AddTo(item2, slot.Id);
                // item 1 gets picked up, then immediately replaced with item 2.
                acting.Activate();
            }
        }
    }
}