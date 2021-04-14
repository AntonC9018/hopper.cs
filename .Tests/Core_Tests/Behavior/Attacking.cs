using Hopper.Core;
using Hopper.Core.Components.Basic;
using Hopper.Core.History;
using Hopper.Core.Items;
using Hopper.Core.Mods;
using Hopper.Core.Registries;
using Hopper.Test_Content.Explosion;
using Hopper.Test_Content.SimpleMobs;
using Hopper.Utils.Chains;
using Hopper.Utils.Vector;
using NUnit.Framework;

namespace Hopper.Tests
{
    public class Attacking_Tests
    {
        public World world;
        public ModResult result;
        public Entity attacking_entity;
        public Entity dummy;
        public EntityFactory<Entity> attacking_entity_factory;
        public EntityFactory<Dummy> dummy_factory;

        public Attacking_Tests()
        {
            result = SetupThing.SetupContent();
            attacking_entity_factory = new EntityFactory<Entity>().AddBehavior(Attacking.Preset);
            dummy_factory = Dummy.Factory;
        }

        [SetUp]
        public void Setup()
        {
            world = new World(3, 3, result.patchArea);
            dummy = world.SpawnEntity(dummy_factory, new IntVector2(1, 1));
            attacking_entity = world.SpawnEntity(attacking_entity_factory, new IntVector2(0, 1));
        }

        public Entity GetFirstEntity()
        {
            return world.grid.GetCellAt(IntVector2.Zero).GetFirstEntity();
        }

        public void AttackRight()
        {
            attacking_entity.Behaviors.Get<Attacking>().Activate(new IntVector2(1, 0));
        }

        [Test]
        public void AttackingWithoutInventory_WorksAsDagger()
        {
            AttackRight();
            Assert.True(dummy.Did(UpdateCode.attacked_do));
            Assert.True(attacking_entity.Did(UpdateCode.attacking_do));
        }

        [Test]
        public void AttackingWithInventory_FailsIfNoWeapon()
        {
            attacking_entity.Inventory = new Inventory(attacking_entity);
            AttackRight();

            Assert.False(dummy.Did(UpdateCode.attacked_do));

            // though the attacking by default is considered to have worked
            Assert.True(attacking_entity.Did(UpdateCode.attacking_do));
        }

        [Test]
        public void AttackingPushes()
        {
            // Add pushable behavior
            var pushable = Pushable.Preset.Instantiate(dummy);
            dummy.Behaviors.Add(typeof(Pushable), pushable);
            // As well as displaceable
            var displaceable = Displaceable.DefaultPreset.Instantiate(dummy);
            dummy.Behaviors.Add(typeof(Displaceable), displaceable);
            AttackRight();
            Assert.True(dummy.Did(UpdateCode.pushed_do));
        }
    }
}