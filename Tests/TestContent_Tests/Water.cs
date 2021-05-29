using Hopper.Core;
using Hopper.Core.Components.Basic;
using Hopper.Core.Stat;
using Hopper.TestContent.PinningNS;
using NUnit.Framework;
using Hopper.Core.WorldNS;
using static Hopper.Utils.Vector.IntVector2;
using Hopper.Core.ActingNS;

namespace Hopper.Tests.Test_Content
{
    public class WaterTests
    {
        public readonly EntityFactory entityFactory;


        public WaterTests()
        {
            InitScript.Init();

            entityFactory = new EntityFactory();
            Transform.AddTo(entityFactory, Layer.REAL);
            Stats.AddTo(entityFactory, Registry.Global.Stats._map);
            
            Displaceable.AddTo(entityFactory, ExtendedLayer.BLOCK).DefaultPreset();
            Moving.AddTo(entityFactory).DefaultPreset();
            Pushable.AddTo(entityFactory).DefaultPreset();
            Ticking.AddTo(entityFactory);
            Acting.AddTo(entityFactory, null, Algos.SimpleAlgo, Order.Entity)
                .DefaultPreset(entityFactory);
        }

        [SetUp]
        public void Setup()
        {
            World.Global = new World(3, 3);
        }

        [Test]
        public void Simple()
        {
            var entity = World.Global.SpawnEntity(entityFactory, Zero);
            var water = World.Global.SpawnEntity(Water.Factory, Zero + Right);
            entity.Move(Right);

            Assert.That(entity.HasPinnedEntityModifier());
            Assert.AreSame(water, entity.GetPinnedEntityModifier().pinner);
            
            entity.GetActing().ActivateWith(Moving.Action.Compile(Right));

            Assert.False(entity.HasPinnedEntityModifier());
            Assert.False(water.IsDead());
            Assert.AreEqual(Zero + Right, entity.GetTransform().position);
            Assert.AreEqual(2, entity.GetTransform().GetCell().Count);

            // No ticking has happened, so nextAction is still set
            entity.GetActing().Activate();
            Assert.AreEqual(Zero + Right + Right, entity.GetTransform().position);
        }

        [Test]
        public void TeleportationRemovesWater()
        {
            var entity = World.Global.SpawnEntity(entityFactory, Zero);
            var water = World.Global.SpawnEntity(Water.Factory, Zero + Right);
            entity.Move(Right);

            entity.GetTransform().ResetPositionInGrid(Zero + Down);
            entity.BePushed(Push.Default(), Right);
            Assert.False(entity.HasPinnedEntityModifier());
        }
    }

}