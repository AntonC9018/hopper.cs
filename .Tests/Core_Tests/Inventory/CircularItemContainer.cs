using NUnit.Framework;
using Hopper.Core.Items;
using Hopper.Core;
using System.Linq;

namespace Hopper.Tests
{
    public class CircularItemContainer_Tests
    {
        private static ISlot<CircularItemContainer<TestItem>> TestSlot =
            new SizedSlot<CircularItemContainer<TestItem>>(
                name: "Test",
                defaultSize: 2);

        private class TestItem : Item
        {
            public override ISlot<IItemContainer<IItem>> Slot => TestSlot;
            public TestItem(ItemMetadata meta) : base(meta)
            {
            }
        }

        private TestItem item_Hello;
        private TestItem item_World;

        public CircularItemContainer_Tests()
        {
            item_Hello = new TestItem(new ItemMetadata("Item_Hello"));
            item_World = new TestItem(new ItemMetadata("Item_World"));
            item_Hello.m_id = 1;
            item_World.m_id = 2;
        }

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void DefaultSizeIsCorrect()
        {
            var container = TestSlot.CreateContainer();
            Assert.AreEqual(2, container.Size);
        }

        [Test]
        public void ItemIsInserted()
        {
            var container = new CircularItemContainer<TestItem>(1);
            container.Insert(item_Hello.Decompose());
            Assert.AreSame(item_Hello, container.AllItems.First());
        }

        [Test]
        public void ItemIsRemoved()
        {
            var container = new CircularItemContainer<TestItem>(1);
            container.Insert(item_Hello.Decompose());
            container.Remove(item_Hello.Decompose());
            Assert.AreEqual(0, container.AllItems.Count());
        }

        [Test]
        public void RemovingUnequippedThrows()
        {
            var container = new CircularItemContainer<TestItem>(1);
            Assert.Throws<Hopper.Utils.Exception>(() => container.Remove(item_Hello.Decompose()));

            container.Insert(item_Hello.Decompose());
            Assert.Throws<System.IndexOutOfRangeException>(() => container.Remove(item_World.Decompose()));
        }

        [Test]
        public void InsertingDecomposedItem_WithCountMoreThan1_Throws()
        {
            var container = new CircularItemContainer<TestItem>(1);
            var decomposedItem = new DecomposedItem(item_Hello, 2);
            Assert.Throws<Hopper.Utils.Exception>(() => container.Insert(decomposedItem));
        }

        [Test]
        public void RemovingDecomposedItem_WithCountMoreThan1_Throws()
        {
            var container = new CircularItemContainer<TestItem>(1);
            var decomposedItem = new DecomposedItem(item_Hello, 2);
            Assert.Throws<Hopper.Utils.Exception>(() => container.Remove(decomposedItem));
        }

        [Test]
        public void ExcessIsCreated_IfAddingOverSize()
        {
            var container = new CircularItemContainer<TestItem>(1);
            container.Insert(item_Hello.Decompose());
            container.Insert(item_World.Decompose());
            var excess = container.PullOutExcess();
            Assert.AreSame(item_Hello, excess[0]);
        }

        [Test]
        public void ResizingWorks()
        {
            var container = new CircularItemContainer<TestItem>(1);
            container.Size = 2;
            Assert.AreEqual(2, container.Size);
        }

        [Test]
        public void ResizingGeneratesExcess()
        {
            var container = new CircularItemContainer<TestItem>(2);
            container.Insert(item_Hello.Decompose());
            container.Insert(item_World.Decompose());
            container.Size = 1;
            var excess = container.PullOutExcess();
            Assert.AreEqual(1, excess.Count);
            Assert.AreSame(item_World, excess[0]);
        }
    }
}