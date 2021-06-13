using Hopper.Core;
using Hopper.Core.Components.Basic;
using Hopper.Core.WorldNS;
using Hopper.Utils.Chains;
using Hopper.Utils.Vector;
using NUnit.Framework;

namespace Hopper.Tests
{
    public class MoreChainTests
    {
        public MoreChainTests()
        {
            InitScript.Init();
        }

        [SetUp]
        public void Setup()
        {
            World.Global = new World(1, 1);
        }

        [Test]
        public void EntityDieWorks()
        {
            var factory = new EntityFactory();
            MoreChains.AddTo(factory, Registry.Global.MoreChains._map);
            Transform.AddTo(factory, 0, 0);

            var entity = World.Global.SpawnEntity(factory, IntVector2.Zero);
            int counter = 0;
            var handler = new Handler<Entity>(10, ctx => counter++);
            Entity.DeathPath.Get(entity).Add(handler);
            Assert.AreEqual(0, counter);
            entity.Die();
            Assert.AreEqual(1, counter);
        }
    }
}