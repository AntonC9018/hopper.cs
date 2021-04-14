using Hopper.Core;
using Hopper.Core.Components.Basic;
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
    public class Interactable_Tests
    {
        public World world;
        public Interactable interactable;
        public Entity entity;
        public ModResult result;
        public EntityFactory<Dummy> dummy_factory;
        public IItem item;

        public Interactable_Tests()
        {
            result = SetupThing.SetupContent();
            dummy_factory = Dummy.Factory;
            item = Bomb.Item;

            var entityPool = Pool.CreateEndless();
            entityPool.AddItemToSubpool("test", new PoolItem(dummy_factory.Id, 1));
            entityPool.FinishConfiguring();
            var itemPool = Pool.CreateEndless();
            itemPool.AddItemToSubpool("test", new PoolItem(item.Id, 1));
            itemPool.FinishConfiguring();
            result.patchArea.DefaultPools = new Pools(result.registry);
            result.patchArea.DefaultPools.UsePools(entityPool, itemPool);
        }

        [SetUp]
        public void Setup()
        {
            world = new World(1, 1, result.patchArea);
            entity = dummy_factory.Instantiate();
        }

        public Entity GetFirstEntity()
        {
            return world.grid.GetCellAt(IntVector2.Zero).GetFirstEntity();
        }

        public void ActivateInteractableWith(IContentSpec spec)
        {
            var interactable = Interactable.Preset(spec).Instantiate(entity);
            entity.Init(IntVector2.Zero, IntVector2.Zero, world);
            entity.ResetInGrid();
            interactable.Activate();
        }

        [Test]
        public void Test_PoolEntity()
        {
            var spec = new PoolEntityContentSpec("test");
            ActivateInteractableWith(spec);

            Assert.AreEqual(GetFirstEntity().GetFactoryId(), dummy_factory.Id);
        }

        [Test]
        public void Test_PoolItem()
        {
            var spec = new PoolItemContentSpec("test");
            ActivateInteractableWith(spec);

            Assert.AreEqual(((DroppedItem)GetFirstEntity()).Item.Id, item.Id);
        }

        [Test]
        public void Test_SetEntity()
        {
            var spec = new SetEntityContentSpec(DroppedItem.Factory);
            ActivateInteractableWith(spec);

            Assert.AreEqual(GetFirstEntity().GetFactoryId(), DroppedItem.Factory.Id);
        }

        [Test]
        public void Test_SetItem()
        {
            var spec = new SetItemContentSpec(item);
            ActivateInteractableWith(spec);

            Assert.AreEqual(((DroppedItem)GetFirstEntity()).Item.Id, item.Id);
        }
    }
}