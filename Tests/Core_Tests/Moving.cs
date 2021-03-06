using NUnit.Framework;
using Hopper.Core;
using Hopper.Utils.Vector;
using static Hopper.Utils.Vector.IntVector2;
using Hopper.Core.Stat;
using Hopper.Core.Components.Basic;
using Hopper.Core.WorldNS;

namespace Hopper.Tests
{
    public class MovingTests
    {
        public readonly EntityFactory entityFactory;

        public MovingTests()
        {
            InitScript.Init();

            entityFactory = new EntityFactory();
            Transform.AddTo(entityFactory, Layers.REAL, 0);
            Stats.AddTo(entityFactory, Registry.Global.Stats._map);
            Displaceable.AddTo(entityFactory, Layers.BLOCK).DefaultPreset();
            Moving.AddTo(entityFactory).DefaultPreset();
        }

        [SetUp]
        public void Setup()
        {
            World.Global = new World(3, 3);
        }

        public GridManager Grid => World.Global.Grid;

        [Test]
        public void MovingWorks()
        {
            var entity = World.Global.SpawnEntity(entityFactory, Zero);
            entity.Move(Right);
            Assert.AreEqual(Zero + Right, entity.GetTransform().position);
        }

        [Test]
        public void CannotMoveOutside()
        {
            var entity = World.Global.SpawnEntity(entityFactory, Zero);
            entity.Move(Left);
            Assert.AreEqual(Zero, entity.GetTransform().position);
        }

        [Test]
        public void StressTest()
        {
            var entity = World.Global.SpawnEntity(entityFactory, Zero);
            for (int i = 0; i < 1000; i++)
                entity.Move(Left);
            Assert.AreEqual(Zero, entity.GetTransform().position);
        }

        [Test]
        public void CannotGoThroughWalls()
        {
            var wallFactory = new EntityFactory();
            Transform.AddTo(wallFactory, Layers.WALL, 0);
            var wall = World.Global.SpawnEntity(wallFactory, Zero + Right);
            var entity = World.Global.SpawnEntity(entityFactory, Zero);
            entity.Move(Right);
            Assert.AreEqual(Zero, entity.GetTransform().position);
            entity.Move(Right);
            entity.Move(Right);
            entity.Move(Right);
            Assert.AreEqual(Zero, entity.GetTransform().position);
        }
    }
}