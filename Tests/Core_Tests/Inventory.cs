using System.Linq;
using Hopper.Core;
using Hopper.Core.ActingNS;
using Hopper.Core.Components.Basic;
using Hopper.Core.Items;
using Hopper.Core.Retouchers;
using Hopper.Core.Stat;
using Hopper.Core.WorldNS;
using Hopper.Utils.Vector;
using NUnit.Framework;

namespace Hopper.Tests
{
    public class InventoryTests
    {
        public EntityFactory itemFactory;
        public EntityFactory entityFactory;

        public InventoryTests()
        {
            InitScript.Init();
            
            {
                entityFactory = new EntityFactory();

                // AddComponents
                var transform = Transform.AddTo(entityFactory, Layers.REAL, TransformFlags.Default);
                var stats = Stats.AddTo(entityFactory, Registry.Global.Stats._map);
                var acting = Acting.AddTo(entityFactory, null, Algos.SimpleAlgo, Order.Player);
                var moving = Moving.AddTo(entityFactory);
                var displaceable = Displaceable.AddTo(entityFactory, Layers.BLOCK);
                var inventory = Inventory.AddTo(entityFactory);
                var ticking = Ticking.AddTo(entityFactory);

                // InitComponents
                acting.DefaultPreset(entityFactory);
                moving.DefaultPreset();
                displaceable.DefaultPreset();
                ticking.DefaultPreset();

                // Retouch
                Equip.OnDisplaceHandlerWrapper.HookTo(entityFactory);
                
                Inventory.AddInitTo(entityFactory);
            }

            {
                itemFactory = new EntityFactory();
                var transform = Transform.AddTo(itemFactory, Layers.ITEM, 0);
            }
        }

        public GridManager Grid => World.Global.Grid;

        [SetUp]
        public void Setup()
        {
            World.Global = new World(3, 3);
        }

        [Test]
        public void EquipSimple()
        {
            // The id of an unregistered factory is 0:0
            var item = World.Global.SpawnEntity(itemFactory, IntVector2.Zero);
            Equippable.AddTo(item, null);

            var entity = World.Global.SpawnEntity(entityFactory, new IntVector2(1, 0));
            var inventory = entity.GetInventory();
            Assert.Zero(inventory._generalStorage.Count);
            Assert.Zero(inventory._excess.Count);
            Assert.Zero(inventory._slots.Count);
            Assert.False(inventory.ContainsItem(item.typeId));

            // Move onto the item
            var moveAction = Moving.Action.Compile(new IntVector2(-1, 0));
            entity.GetActing().ActivateWith(moveAction);
            Assert.AreEqual(new IntVector2(0, 0), entity.GetTransform().position);
            Assert.True(inventory.ContainsItem(item.typeId));
            
            inventory.Remove(item.typeId);
            Assert.False(inventory.ContainsItem(item.typeId));

            // Reset acting flags (so that we can act again)
            // acting._flags = 0;

            // Move the entity back
            entity.GetTransform().ResetPositionInGrid(new IntVector2(1, 0));

            // Spawn two items this time
            // Since two of the same items without a slot is not allowed, assign them a slot
            var slot = new Slot(false); slot.Id = new Identifier(3, 1);

            var item1 = World.Global.SpawnEntity(itemFactory, IntVector2.Zero);
            Equippable.AddTo(item1, null);
            SlotComponent.AddTo(item1, slot.Id);

            var item2 = World.Global.SpawnEntity(itemFactory, IntVector2.Zero);
            Equippable.AddTo(item2, null);
            SlotComponent.AddTo(item2, slot.Id);
            
            // item 1 gets picked up, then immediately replaced by item 2.
            // item 1 is dropped back as excess.
            entity.GetActing().ActivateWith(moveAction);
            Assert.True(inventory.ContainsItem(item2.typeId));
            Assert.AreSame(inventory.GetItem(item2.typeId), item2);
            Assert.AreSame(item1.GetTransform().GetAllFromLayer(Layers.ITEM).Single().entity, item1);
            Assert.AreSame(inventory.GetItemFromSlot(slot.Id), item2);
        }
    }
}