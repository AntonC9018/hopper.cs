using Hopper.Core;
using Hopper.Core.Components.Basic;
using Hopper.Core.Mods;
using Hopper.Core.Stat.Basic;
using Hopper.TestContent.Floor;
using Hopper.Utils.Vector;
using NUnit.Framework;

namespace Hopper.Tests.Test_Content
{
    public class Bounce_Tests
    {
        public readonly EntityFactory<Entity> test_factory;
        public readonly ModResult mod_result;
        public World world;
        public Entity entity;

        public Bounce_Tests()
        {
            test_factory = new EntityFactory<Entity>()
                .AddBehavior(Displaceable.DefaultPreset)
                .AddBehavior(Pushable.Preset);
            mod_result = SetupThing.SetupContent();
            // .AddBehavior(Statused.Preset);
        }

        [SetUp]
        public void Setup()
        {
            world = new World(5, 3, mod_result.patchArea);
        }

        public void DisplaceEntity(IntVector2 vec)
        {
            entity.Behaviors.Get<Displaceable>().Activate(
                vec,
                new Move
                {
                    power = 1,
                    through = 0
                }
            );
        }

        [Test]
        public void Test_1()
        {
            entity = world.SpawnEntity(test_factory, new IntVector2(0, 1));
            var trap1 = world.SpawnEntity(BounceTrap.Factory, new IntVector2(1, 1), new IntVector2(1, 0));

            DisplaceEntity(new IntVector2(1, 0));
            Assert.AreEqual(new IntVector2(1, 1), entity.Pos);

            world.Loop();

            Assert.AreEqual(1, trap1.GetCell().m_transforms.Count);
            Assert.AreEqual(new IntVector2(2, 1), entity.Pos);

            var trap2 = world.SpawnEntity(BounceTrap.Factory, new IntVector2(2, 1), new IntVector2(1, 0));
            entity.ResetPosInGrid(new IntVector2(0, 1));

            world.Loop();
            DisplaceEntity(new IntVector2(1, 0));
            world.Loop();

            Assert.AreEqual(new IntVector2(3, 1), entity.Pos);
            Assert.AreEqual(1, trap1.GetCell().m_transforms.Count);
            Assert.AreEqual(1, trap2.GetCell().m_transforms.Count);
        }
    }
}