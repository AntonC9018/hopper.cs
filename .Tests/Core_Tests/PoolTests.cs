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
        public Identifier id_baz;

        public PoolTests()
        {
            id_foo = new Identifier(0, 1);
            id_bar = new Identifier(0, 2);
            id_baz = new Identifier(0, 3);
        }

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ExhaustWithOneItem()
        {
            var subpool = new SubPool{{ id_foo, 1, 1 }};

            var id = subpool.Draw(0.1);
            Assert.AreEqual(id, id_foo);
            subpool.AdjustAmount(id, -1);
            Assert.True(subpool.IsExhausted());
            
            subpool.AdjustAmount(id_foo, 2);
            Assert.False(subpool.IsExhausted());

            var id1 = subpool.Draw(0.1);
            Assert.AreEqual(id1, id_foo);
            subpool.AdjustAmount(id1, -1);
            Assert.False(subpool.IsExhausted());

            var id2 = subpool.Draw(0.1);
            subpool.AdjustAmount(id2, -1);
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
            var id  = subpool.Draw(0.1);
            subpool.AdjustAmount(id, -1);
            var id1 = subpool.Draw(0.1);
            subpool.AdjustAmount(id1, -1);
            Assert.AreNotEqual(id, id1);
            Assert.True(subpool.IsExhausted());
        }
    }
}