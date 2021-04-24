using NUnit.Framework;
using Hopper.Core;
using Hopper.Core.Components.Basic;
using Hopper.Core.Mods;
using Hopper.TestContent.Bind;
using Hopper.Utils.Vector;
using Hopper.Core.Stat.Basic;
using Hopper.TestContent.SimpleMobs;
using System.Linq;
using Hopper.Core.History;
using Hopper.TestContent.Explosion;

namespace Hopper.Tests.Test_Content
{
    public class Bind_Tests
    {
        public readonly EntityFactory<Entity> test_entity_factory;
        public readonly EntityFactory<Player> test_player_factory;
        public readonly DirectedAction move_action;
        public readonly ModResult mod_result;
        public World world;

        public Bind_Tests()
        {
            mod_result = SetupThing.SetupContent();
            test_entity_factory = new EntityFactory<Entity>().AddBehavior(Statused.Preset);
            test_player_factory = new EntityFactory<Player>()
                .AddBehavior(Acting.Preset(new Acting.Config(Algos.SimpleAlgo)))
                .AddBehavior(Statused.Preset)
                .AddBehavior(Displaceable.DefaultPreset)
                .AddBehavior(Moving.Preset)
                .AddBehavior(Attackable.DefaultPreset);
            move_action = Action.CreateBehavioral<Moving>();
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
            var zero_zero_cell = world.grid.GetCellAt(IntVector2.Zero);

            world.Loop();

            Assert.That(Bind.StopMoveStatus.IsApplied(player));
            Assert.AreEqual(player.Pos, spider.Pos);
            Assert.AreEqual(2, zero_zero_cell._transforms.Count);
            Assert.AreEqual(player, zero_zero_cell._transforms[0]);

            player.Behaviors.Get<Acting>().nextAction = move_action.ToDirectedParticular(IntVector2.Right);
            world.Loop();

            Assert.That(Bind.StopMoveStatus.IsApplied(player));
            Assert.AreEqual(IntVector2.Zero, player.Pos);
            Assert.AreEqual(player.Pos, spider.Pos);

            player.Behaviors.Get<Displaceable>().Activate(
                new IntVector2(1, 0),
                new Move { power = 1, through = 0 }
            );

            Assert.AreEqual(new IntVector2(1, 0), player.Pos);

            Assert.AreEqual(0, zero_zero_cell._transforms.Count);
            Assert.AreEqual(player.Pos, spider.Pos);
            Assert.AreEqual(player, player.GetCell().m_transforms[0]);

            var test_enemy = world.SpawnEntity(Skeleton.Factory, player.Pos + new IntVector2(1, 0));
            world.Loop();

            Assert.That(player.History.Updates.Any(update => update.updateCode == UpdateCode.attacked_do));
            Assert.That(!spider.History.Updates.Any(update => update.updateCode == UpdateCode.attacked_do));

            spider.Die();
            test_enemy.Die();
            world.Loop();

            Assert.False(Bind.StopMoveStatus.IsApplied(player));

            player.Behaviors.Get<Acting>().nextAction = move_action.ToDirectedParticular(IntVector2.Right);
            world.Loop();

            Assert.AreEqual(new IntVector2(2, 0), player.Pos);
        }

        [Test]
        public void Test_2()
        {
            var spider = world.SpawnEntity(Spider.Factory, new IntVector2(1, 1), new IntVector2(1, 0));
            var player = world.SpawnPlayer(test_player_factory, new IntVector2(0, 0));
            var zero_zero_cell = world.grid.GetCellAt(IntVector2.Zero);

            world.Loop();
            world.InitializeWorldEvents();

            spider.History.Clear();
            player.History.Clear();

            Explosion.Explode(IntVector2.Zero, 0, world);

            Assert.That(spider.History.Updates.Any(u => u.updateCode == UpdateCode.attacked_do));
            Assert.That(!player.History.Updates.Any(u => u.updateCode == UpdateCode.attacked_do));
        }
    }
}