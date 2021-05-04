using NUnit.Framework;
using Hopper.Core;
using Hopper.Utils.Vector;
using System.Linq;
using Hopper.Core.History;
using Hopper.Core.Stat;
using Hopper.TestContent.Bind;
using Hopper.Core.Components.Basic;
using Hopper.Core.Targeting;

namespace Hopper.Tests.Test_Content
{
    public class Bind_Tests
    {
        public readonly EntityFactory entityFactory;
        public readonly EntityFactory bindingFactory;

        public Bind_Tests()
        {
            InitScript.Init();
            
            entityFactory = new EntityFactory();
            Transform.AddTo(entityFactory, Layer.REAL);
            Stats.AddTo(entityFactory, Registry.Global._defaultStats);
            Attackable.AddTo(entityFactory, Attackness.ALWAYS).DefaultPreset();
            Damageable.AddTo(entityFactory, new Health(1)).DefaultPreset();
            Displaceable.AddTo(entityFactory, Layer.WALL | Layer.REAL).DefaultPreset();
            Moving.AddTo(entityFactory).DefaultPreset();
            entityFactory.InitInWorldFunc = e => e.entity.GetStats().Init();

            bindingFactory = new EntityFactory();
            Transform.AddTo(bindingFactory, Layer.REAL);
            Stats.AddTo(bindingFactory, Registry.Global._defaultStats);
            Binding.AddTo(bindingFactory, Layer.REAL, BoundEntityModifier.DefaultHookable).DefaultPreset();
            Damageable.AddTo(bindingFactory, new Health(1)).DefaultPreset();
            Attackable.AddTo(bindingFactory, Attackness.ALWAYS).DefaultPreset();
            bindingFactory.InitInWorldFunc = e => e.entity.GetStats().Init();
        }

        [SetUp]
        public void Setup()
        {
            World.Global = new World(3, 3);
        }

        public GridManager Grid => World.Global.grid;

        [Test]
        public void Test_1()
        {
            var spider = World.Global.SpawnEntity(bindingFactory, new IntVector2(1, 1));
            var entity = World.Global.SpawnEntity(entityFactory, new IntVector2(0, 0));

            // Bind the entity by the spider
            Assert.False(spider.IsCurrentlyBinding());
            spider.GetBinding().Activate(spider, new IntVector2(-1, -1));
            Assert.That(spider.IsCurrentlyBinding());

            // The bind got applied to the spider and the entity
            Assert.True(Binding.GuestDiedCallbackHandlerWrapper.IsHookedTo(spider));
            Assert.True(entity.HasBoundEntityModifier());
            Assert.AreEqual(1, entity.GetTransform().GetCell().Count);

            // Moving is not allowed
            entity.Move(IntVector2.Right);
            Assert.AreEqual(IntVector2.Zero, entity.GetTransform().position);

            // Displacing is allowed
            entity.Displace(IntVector2.Right, new Move(power: 1, through: 0));
            Assert.AreEqual(new IntVector2(1, 0), entity.GetTransform().position);

            // When the guest dies, the host is released
            spider.Die();
            Assert.False(entity.HasBoundEntityModifier());

            // After the host is released, they can move
            entity.Move(IntVector2.Right);
            Assert.AreEqual(new IntVector2(2, 0), entity.GetTransform().position);

            // Let's apply the binding again
            spider = World.Global.SpawnEntity(bindingFactory, new IntVector2(2, 1));
            spider.GetBinding().Activate(spider, new IntVector2(0, -1));

            // Make sure it worked
            Assert.That(entity.HasBoundEntityModifier());

            // If the entity dies, the bound modifier will be removed
            entity.Die();
            Assert.False(spider.IsCurrentlyBinding());
            Assert.AreSame(spider, spider.GetTransform().GetCell().Single().entity);
        }
    }
}