using NUnit.Framework;
using Hopper.Core;
using Hopper.Core.Behaviors.Basic;
using Hopper.Core.Mods;
using Hopper.Test_Content.Bind;
using Hopper.Utils.Vector;
using Hopper.Core.Stats.Basic;

namespace Hopper.Tests.Test_Content
{
    public class Bind_Tests
    {
        public readonly EntityFactory<Entity> test_entity_factory;
        public readonly EntityFactory<Player> test_player_factory;
        public readonly Action move_action;
        public readonly ModResult mod_result;
        public World world;

        public Bind_Tests()
        {
            mod_result = SetupThing.SetupContent();
            test_entity_factory = new EntityFactory<Entity>().AddBehavior<Statused>();
            test_player_factory = new EntityFactory<Player>()
                .AddBehavior<Acting>(new Acting.Config(Algos.SimpleAlgo))
                .AddBehavior<Statused>()
                .AddBehavior<Displaceable>()
                .AddBehavior<Moving>();
            move_action = new BehaviorAction<Moving>();
        }

        [SetUp]
        public void Setup()
        {
            world = new World(3, 3, mod_result.patchArea);
        }

        [Test]
        public void Test_1()
        {
            var spider = world.SpawnEntity(Spider.Factory, new IntVector2(1, 1), new IntVector2(1, 0));
            var player = world.SpawnPlayer(test_player_factory, new IntVector2(0, 0));
            var zero_zero_cell = world.Grid.GetCellAt(IntVector2.Zero);

            world.Loop();

            Assert.That(BindStatuses.StopMove.IsApplied(player));
            Assert.AreEqual(player.Pos, spider.Pos);
            Assert.AreEqual(2, zero_zero_cell.m_entities.Count);
            Assert.AreEqual(player, zero_zero_cell.m_entities[0]);

            player.Behaviors.Get<Acting>().NextAction = move_action.WithDir(IntVector2.Right);
            world.Loop();

            Assert.That(BindStatuses.StopMove.IsApplied(player));
            Assert.AreEqual(IntVector2.Zero, player.Pos);
            Assert.AreEqual(player.Pos, spider.Pos);

            player.Behaviors.Get<Displaceable>().Activate(
                new IntVector2(1, 0),
                new Move { power = 1, through = 0 }
            );

            Assert.AreEqual(new IntVector2(1, 0), player.Pos);

            Assert.AreEqual(0, zero_zero_cell.m_entities.Count);
            Assert.AreEqual(player.Pos, spider.Pos);
            Assert.AreEqual(player, player.GetCell().m_entities[0]);

            spider.Die();
            world.Loop();

            Assert.That(BindStatuses.StopMove.IsApplied(player) == false);

            player.Behaviors.Get<Acting>().NextAction = move_action.WithDir(IntVector2.Right);
            world.Loop();

            Assert.AreEqual(new IntVector2(2, 0), player.Pos);
        }
    }
}