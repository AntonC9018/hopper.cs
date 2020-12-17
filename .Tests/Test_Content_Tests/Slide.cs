using Hopper.Core;
using Hopper.Core.Behaviors.Basic;
using Hopper.Core.Mods;
using Hopper.Core.Retouchers;
using Hopper.Core.Stats.Basic;
using Hopper.Test_Content.Floor;
using Hopper.Utils.Vector;
using NUnit.Framework;

namespace Hopper.Tests.Test_Content
{
    public class Slide_Tests
    {
        public readonly EntityFactory<Player> test_player_factory;

        public readonly Action move_action;
        public readonly ModResult mod_result;
        public World world;
        public Player player;
        public IceFloor[] ice_floors;


        public Slide_Tests()
        {
            move_action = new BehaviorAction<Moving>();
            test_player_factory = new EntityFactory<Player>()
                .AddBehavior<Acting>(new Acting.Config(Algos.SimpleAlgo))
                .AddBehavior<Statused>()
                .AddBehavior<Displaceable>()
                .AddBehavior<Moving>()
                .AddBehavior<Pushable>()
                .Retouch(Reorient.OnDisplace)
                .AddBehavior<Controllable>(new Controllable.Config(move_action));

            mod_result = SetupThing.SetupContent();
        }

        [SetUp]
        public void Setup()
        {
            world = new World(5, 4, mod_result.patchArea);
            player = world.SpawnPlayer(test_player_factory, new IntVector2(0, 1));
            ice_floors = new IceFloor[3];
            for (int i = 0; i < 3; i++)
            {
                ice_floors[i] = world.SpawnEntity(IceFloor.Factory, new IntVector2(i + 1, 1));
            }
        }

        public void PushPlayer(IntVector2 vec)
        {
            player.Behaviors.Get<Pushable>().Activate(vec,
                new Push { distance = 1, power = 2, sourceId = Push.BasicSource.Id, pierce = 2 });
        }

        [Test]
        public void Test_Linear_Slide()
        {
            PushPlayer(new IntVector2(1, 0));

            world.Loop(); // first one just applies, they do not slide on first turn
            world.Loop(); // e__ -> _e_
            world.Loop(); // _e_ -> __e
            world.Loop(); // __e -> ___e

            Assert.AreEqual(ice_floors[2].Pos + IntVector2.Right, player.Pos);
        }

        [Test]
        public void Test_1()
        {
            PushPlayer(new IntVector2(1, 0));

            Assert.AreEqual(new IntVector2(1, 1), player.Pos);

            world.Loop();

            // the sliding does not affect the actual position directly. 
            // It simply applies a status effect.
            Assert.AreEqual(new IntVector2(1, 1), player.Pos);
            Assert.That(SlideStatus.Status.IsApplied(player));

            // Voluntary moving gets prevented by changing the default action
            var calculatedAction = player.Behaviors.Get<Controllable>()
                .ConvertVectorToAction(new IntVector2(0, 1));
            Assert.IsNull(calculatedAction);

            world.Loop();
            Assert.AreEqual(new IntVector2(2, 1), player.Pos);

            // However, if the player gets pushed, then they should slide it that direction
            // that is, the player's displacement is in the direction that they stepped onto the 
            // ice floor with. If the player gets pushed, so does this variable, reflecting
            // the new direction.
            var ice_floor_below = world.SpawnEntity(IceFloor.Factory, new IntVector2(2, 2));
            PushPlayer(new IntVector2(0, 1));

            var x = (SlideData)player.Tinkers.GetStore(SlideStatus.Status.m_tinker);

            Assert.AreEqual(new IntVector2(2, 2), player.Pos);
            Assert.AreEqual(new IntVector2(0, 1), x.currentDirection);
            Assert.That(SlideStatus.Status.IsApplied(player));

            world.Loop();

            Assert.AreEqual(new IntVector2(2, 3), player.Pos);
        }
    }

}