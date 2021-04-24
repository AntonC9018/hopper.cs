using Hopper.Core;
using Hopper.Core.Components.Basic;
using Hopper.Core.History;
using Hopper.Core.Items;
using Hopper.Core.Mods;
using Hopper.Core.Stat;
using Hopper.Core.Targeting;
using Hopper.TestContent.Explosion;
using Hopper.TestContent.SimpleMobs;
using Hopper.Utils.Chains;
using Hopper.Utils.Vector;
using NUnit.Framework;

namespace Hopper.Tests
{
    public class Attacking_Tests
    {
        public World world;
        public Entity attacking_entity;
        public Entity dummy;
        public EntityFactory attacking_entity_factory;
        public EntityFactory dummy_factory;

        public Attacking_Tests()
        {
            SetupThing.SetupContent();

            attacking_entity_factory = new EntityFactory();
            Attacking.AddTo(attacking_entity_factory).NoInventoryPreset();
            Transform.AddTo(attacking_entity_factory, Layer.REAL);
            Stats    .AddTo(attacking_entity_factory, Registry.Global._defaultStats);
            
            dummy_factory = Dummy.Factory;
            Transform .AddTo(dummy_factory, Layer.REAL);
            Attackable.AddTo(dummy_factory, Attackness.ALWAYS);
            Stats     .AddTo(dummy_factory, Registry.Global._defaultStats);
        }

        [SetUp]
        public void Setup()
        {
            World.Global = new World(3, 3);
            dummy            = world.SpawnEntity(dummy_factory, new IntVector2(1, 1));
            attacking_entity = world.SpawnEntity(attacking_entity_factory, new IntVector2(0, 1));
        }

        public Transform GetFirstTransform()
        {
            return World.Global.grid.GetCellAt(IntVector2.Zero).GetFirstTransform();
        }

        public void AttackRight()
        {
            attacking_entity.Attack(new IntVector2(1, 0), null);
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
            Pushable.AddTo(dummy).DefaultPreset();
            // As well as displaceable
            Displaceable.AddTo(dummy, ExtendedLayer.BLOCK).DefaultPreset();

            AttackRight();
            Assert.True(dummy.Did(UpdateCode.pushed_do));
        }
    }
}