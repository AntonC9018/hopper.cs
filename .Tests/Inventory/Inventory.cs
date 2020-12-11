using NUnit.Framework;
using Hopper.Core.Items;
using Hopper.Core;
using System.Collections.Generic;

namespace Hopper.Tests
{
    public class Inventory_Tests
    {
        private static ISlot<CircularItemContainer<TestItem>> TestSlot =
            new SizedSlot<CircularItemContainer<TestItem>>(
                name: "Test",
                defaultSize: 2);

        private enum ItemAction
        {
            Equip, Unequip, Destroy
        }

        private class TestItem : Item
        {
            public override ISlot<IItemContainer<IItem>> Slot => TestSlot;
            public List<ItemAction> messages;

            public TestItem(ItemMetadata meta) : base(meta)
            {
                messages = new List<ItemAction>();
            }

            public override void BeUnequipped(Entity entity) => messages.Add(ItemAction.Unequip);
            public override void BeDestroyed(Entity entity) => messages.Add(ItemAction.Destroy);
            public override void BeEquipped(Entity entity) => messages.Add(ItemAction.Equip);
        }

        private Inventory inventory;
        private TestItem item_Hello;
        private TestItem item_World;

        public Inventory_Tests()
        {
            Registry registry = new Registry();
            item_Hello = new TestItem(new ItemMetadata("Hello"));
            item_World = new TestItem(new ItemMetadata("World"));
            item_Hello.RegisterSelf(registry);
            item_World.RegisterSelf(registry);
        }

        [SetUp]
        public void Setup()
        {
            item_Hello.messages.Clear();
            item_World.messages.Clear();
        }

        [Test]
        public void EmptyInventory()
        {
            inventory = new Inventory(null, new Dictionary<ISlot<IItemContainer<IItem>>, IItemContainer<IItem>>());
        }

        [Test]
        public void CustomSlotIsAdded()
        {
            inventory = new Inventory(null, new Dictionary<ISlot<IItemContainer<IItem>>, IItemContainer<IItem>>());
            Assert.That(
                inventory.CanEquipItem(item_Hello) == false,
                "We can't add items to the custom slot, since the inventory has no slots");

            // initialize with default slots (all members of the Slot enum)
            inventory = new Inventory(null);
            Assert.That(inventory.CanEquipItem(item_Hello), "We can add items to the custom slot");
        }

        [Test]
        public void IsItemEquipped()
        {
            // initialize with default slots (all members of the enum Slot)
            inventory = new Inventory(null);
            inventory.Equip(item_Hello);

            Assert.AreEqual(inventory.GetContainer(TestSlot)[0], item_Hello);
            Assert.AreEqual(ItemAction.Equip, item_Hello.messages[0], "The equip method was called");
        }

        [Test]
        public void IsItemUnEquipped()
        {
            // initialize with default slots (all members of the enum Slot)
            inventory = new Inventory(null);
            inventory.Equip(item_Hello);   // [0]
            inventory.Unequip(item_Hello); // [1]
            Assert.AreEqual(ItemAction.Unequip, item_Hello.messages[1], "The unequip method was called");
        }

        [Test]
        public void IsItemDestroyed()
        {
            // initialize with default slots (all members of the enum Slot)
            inventory = new Inventory(null);
            inventory.Equip(item_Hello);   // [0]
            inventory.Destroy(item_Hello); // [1]
            Assert.AreEqual(ItemAction.Destroy, item_Hello.messages[1], "The destroy method was called");
        }

        [Test]
        public void IsExcessDropped()
        {
            // initialize with default slots (all members of the enum Slot)
            inventory = new Inventory(null);
            inventory.GetContainer(TestSlot).Size = 1;
            inventory.Equip(item_Hello); // [0]
            inventory.Equip(item_World);
            inventory.DropExcess();      // [1] for item_Hello
            Assert.AreEqual(ItemAction.Unequip, item_Hello.messages[1]);
        }
    }
}