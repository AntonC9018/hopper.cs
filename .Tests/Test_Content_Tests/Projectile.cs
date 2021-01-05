using NUnit.Framework;
using Hopper.Core;
using Hopper.Core.Behaviors.Basic;
using Hopper.Test_Content.Projectiles;
using Hopper.Core.Mods;
using Hopper.Test_Content.SimpleMobs;
using Hopper.Utils.Vector;
using Hopper.Core.History;

namespace Hopper.Tests.Test_Content
{
    public class Projectile_Tests
    {
        public World world;
        public Entity dummy_at_1_1;
        public ModResult result;

        public Projectile_Tests()
        {
            result = SetupThing.SetupContent();
        }

        [SetUp]
        public void Setup()
        {
            world = new World(3, 3, result.patchArea);
            dummy_at_1_1 = world.SpawnEntity(Dummy.Factory, new IntVector2(1, 1));
        }

        [Test]
        public void Moves()
        {
            var projectile = world.SpawnEntity(Projectile.SimpleFactory, new IntVector2(0, 0), new IntVector2(1, 0));

            world.Loop();
            Assert.AreEqual(new IntVector2(1, 0), projectile.Pos, "It moved");
            Assert.True(projectile.Did(UpdateCode.displaced_do));
            Assert.False(projectile.IsDead, "It didn't die of nothing");
        }

        [Test]
        public void AttacksWithoutMoving_IfTargetIsLookingAtIt()
        {
            var projectile = world.SpawnEntity(Projectile.SimpleFactory, new IntVector2(1, 1), new IntVector2(1, 0));
            // give the dummy an orientation of minus the projectile's orientation
            dummy_at_1_1.Orientation = new IntVector2(-1, 0);

            world.Loop();
            Assert.AreEqual(new IntVector2(1, 1), projectile.Pos, "It didn't move");
            Assert.False(projectile.Did(UpdateCode.displaced_do));
            Assert.True(dummy_at_1_1.Did(UpdateCode.attacked_do));
            Assert.True(projectile.IsDead);
        }

        [Test]
        public void Attaks_IfSomethingEntersTheCell()
        {
            var projectile = world.SpawnEntity(Projectile.SimpleFactory, new IntVector2(0, 1), new IntVector2(0, -1));
            // projectile goes to cell 0, 0
            projectile.Behaviors.Get<Acting>().CalculateNextAction();
            projectile.Behaviors.Get<Acting>().Activate();
            Assert.False(projectile.IsDead);
            Assert.AreEqual(new IntVector2(0, 0), projectile.Pos);

            // place dummy at 0, 0
            dummy_at_1_1.ResetPosInGrid(new IntVector2(0, 0));

            Assert.True(dummy_at_1_1.Did(UpdateCode.attacked_do));
            Assert.True(projectile.IsDead);
        }
    }
}