using NUnit.Framework;
using Hopper.Core;
using Hopper.Utils.Vector;
using Hopper.Core.Stat;
using Hopper.Core.Components.Basic;
using System.Collections.Generic;
using Hopper.TestContent.BouncingNS;
using Hopper.Core.WorldNS;

namespace Hopper.Tests.Test_Content
{
    public class Bouncing_Test
    {
        public readonly EntityFactory entityFactory;

        public Bouncing_Test()
        {
            InitScript.Init();

            entityFactory = new EntityFactory();
            Transform.AddTo(entityFactory, Layer.REAL);
            Stats.AddTo(entityFactory, Registry.Global._defaultStats);
            Displaceable.AddTo(entityFactory, ExtendedLayer.BLOCK).DefaultPreset();
            Pushable.AddTo(entityFactory).DefaultPreset();
            
        }

        [SetUp]
        public void Setup()
        {
            World.Global = new World(5, 3);
        }

        public GridManager Grid => World.Global.Grid;

        [Test]
        public void Test_One_Two()
        {
            var entity = World.Global.SpawnEntity(entityFactory, new IntVector2(0, 1));
            var trap1 = World.Global.SpawnEntity(BounceTrap.Factory, new IntVector2(1, 1), new IntVector2(1, 0));

            entity.Displace(new IntVector2(1, 0), Move.Default());
            Assert.AreEqual(new IntVector2(1, 1), entity.GetTransform().position);

            World.Global.Loop();

            Assert.AreEqual(1, trap1.GetTransform().GetCell().Count);
            Assert.AreEqual(new IntVector2(2, 1), entity.GetTransform().position);

            var trap2 = World.Global.SpawnEntity(BounceTrap.Factory, new IntVector2(2, 1), new IntVector2(1, 0));
            entity.GetTransform().ResetPositionInGrid(new IntVector2(0, 1));

            World.Global.Loop();
            entity.Displace(new IntVector2(1, 0), Move.Default());
            World.Global.Loop();

            Assert.AreEqual(new IntVector2(3, 1), entity.GetTransform().position);
            Assert.AreEqual(1, trap1.GetTransform().GetCell().Count);
            Assert.AreEqual(1, trap2.GetTransform().GetCell().Count);
        }

        [Test]
        public void Test_Cycle()
        {
            var entity = World.Global.SpawnEntity(entityFactory, new IntVector2(0, 2));
            var traps = new List<Entity>(4);

            var position = new IntVector2(1, 2);
            foreach (var direction in IntVector2.OrthogonallyAdjacentToOrigin)
            {
                traps.Add(World.Global.SpawnEntity(BounceTrap.Factory, position, direction));
                position += direction;
            }

            entity.Displace(IntVector2.Right, Move.Default());
            World.Global.Loop();

            Assert.AreEqual(new IntVector2(1, 2), entity.GetTransform().position);
        }
    }
}