using NUnit.Framework;
using Hopper.Core.Items;

namespace Hopper.Tests
{
    public class SuperPool
    {
        private Pool pool;

        public SuperPool()
        {
        }

        [SetUp]
        public void Setup()
        {
            pool = new Pool();

        }

        [Test]
        public void DrawingFrom_EmptyPool_Throws()
        {
            Assert.Throws<Hopper.Utils.Exception>(() => pool.(""));
        }

        [Test]
        public void FinishingConfuguringOf_EmptyPool_Throws()
        {
            Assert.Throws<Hopper.Utils.Exception>(() => endlessPool.FinishConfiguring());
        }

        [Test]
        public void PoolIsCreated()
        {
            endlessPool.AddItemToSubpool("test", new PoolItem(1, 1));
            endlessPool.FinishConfiguring();
            Assert.DoesNotThrow(() => endlessPool.GetNextItem("test"));
        }

        [Test]
        public void ItemIsAdded()
        {
            endlessPool.AddItemToSubpool("test", new PoolItem(1, 1));
            endlessPool.FinishConfiguring();
            Assert.AreEqual(1, endlessPool.GetNextItem("test").id);
        }

        [Test]
        public void QuantityDecrements_ForNormalSubpools()
        {
            var it = new PoolItem(1, 1);
            normalPool.AddItemToSubpool("test", it);
            normalPool.FinishConfiguring();
            normalPool.GetNextItem("test");
            Assert.AreEqual(0, it.quantity);
        }

        [Test]
        public void ResetsCorrectly()
        {
            var it = new PoolItem(1, 2);
            normalPool.AddItemToSubpool("test", it);
            normalPool.FinishConfiguring();

            Assert.AreEqual(1, normalPool.GetNextItem("test").id);
            Assert.AreEqual(1, it.quantity);

            normalPool.ResetAll();

            Assert.AreEqual(2, it.quantity);
        }

        [Test]
        public void NormalSubpoolResets_AfterTheItemsAreExhausted()
        {
            var it1 = new PoolItem(1, 1);
            var it2 = new PoolItem(2, 1);
            var it3 = new PoolItem(3, 1);
            normalPool.AddItemToSubpool("1", it1);
            normalPool.AddItemToSubpool("1", it2);
            normalPool.AddItemToSubpool("2", it3);
            normalPool.FinishConfiguring();

            // Draw all items from the first pool
            normalPool.GetNextItem("1");
            normalPool.GetNextItem("1");
            Assert.AreEqual(0, it1.quantity);
            Assert.AreEqual(0, it2.quantity);

            // Watch the pool get reset and then of the items drawn again
            normalPool.GetNextItem("1");
            Assert.That(it1.quantity == 1 || it2.quantity == 1);

            // Draw all items from all pools
            normalPool.GetNextItem("1");
            normalPool.GetNextItem("2");

            // Reset the first pool again
            normalPool.GetNextItem("1");

            Assert.AreEqual(0, it3.quantity, "The second subpool must not be affected in this case.");
        }
    }
}