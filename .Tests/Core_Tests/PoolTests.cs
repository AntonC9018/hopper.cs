using System.Collections.Generic;
using Hopper.Core;
using Hopper.Core.Items;
using NUnit.Framework;

namespace Hopper.Tests
{
    public class PoolTests
    {
        public Identifier id_foo;
        public Identifier id_bar;

        public PoolTests()
        {
            id_foo = new Identifier(0, 1);
            id_bar = new Identifier(0, 2);
        }

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ExhaustWithOneItem()
        {
            var subpool = new SubPool{{ id_foo, 1, 1 }};

            var id = subpool.Draw(0.1); subpool.AdjustAmount(id, -1);
            Assert.AreEqual(id, id_foo);
            Assert.True(subpool.IsExhausted());
            
            subpool.AdjustAmount(id_foo, 2);
            Assert.False(subpool.IsExhausted());

            var id1 = subpool.Draw(0.1); subpool.AdjustAmount(id1, -1);
            Assert.AreEqual(id1, id_foo);
            Assert.False(subpool.IsExhausted());

            var id2 = subpool.Draw(0.1); subpool.AdjustAmount(id2, -1);
            Assert.True(subpool.IsExhausted());
        }

        [Test]
        public void TwoItems()
        {
            var subpool = new SubPool{
                { id_foo, 1, 1 }, 
                { id_bar, 1, 1 }
            };

            // Now, we cannot predict in which order the items got placed in the
            // dictionary, since the dictionary is not sorted.
            // So lets just test that the items were both taken out
            var id  = subpool.Draw(0.1); subpool.AdjustAmount(id,  -1);
            var id1 = subpool.Draw(0.1); subpool.AdjustAmount(id1, -1);
            Assert.AreNotEqual(id, id1);
            Assert.True(subpool.IsExhausted());
        }

        [Test]
        public void PoolWorks()
        {
            var subpool1 = new SubPool{
                { id_foo, 1, 1 }, 
                { id_bar, 1, 1 }
            };

            var subpool2 = new SubPool{
                { id_foo, 1, 1 }, 
                { id_bar, 1, 1 }
            };

            var templatePool = new Pool { 
                // these id's in this case just serve as placeholders
                // for subpool id's, these are not item id's in this case.
                { id_foo, subpool1 }, 
                { id_bar, subpool2 } 
            };

            var pool = new Pool(templatePool);

            var id = pool.DrawFrom(id_foo);
            Assert.AreEqual(1, pool[id_foo].sum);
            Assert.AreEqual(1, pool[id_bar].sum);

            // Older sums are unaffected
            Assert.AreEqual(2, subpool1.sum);
            Assert.AreEqual(2, subpool2.sum);

            var id1 = pool.DrawFrom(id_bar);
            Assert.AreEqual(2, pool[id_foo].sum);
            Assert.AreEqual(2, pool[id_bar].sum);
        }


        [Test]
        public void LootTableWorks()
        {
            var table = new LootTable
            {
                { id_foo, 1 },
                { id_bar, 1 }
            };
            Assert.AreEqual(2, table.sum);

            var id  = table.Draw(0.1);
            var id1 = table.Draw(0.9);
            Assert.AreNotEqual(id, id1);
            Assert.False(table.IsEmpty());
        }

        [Test]
        public void LootTablesWrapperWorks()
        {
            var table = new LootTable
            {
                { id_foo, 1 }
            };

            var wrapper = new LootTablesWrapper
            {
                { id_bar, table }
            };

            var id = wrapper.DrawFrom(id_bar);
            Assert.AreEqual(id_foo, id);
        }
    }
}