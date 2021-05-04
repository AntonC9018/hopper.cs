using System.Linq;
using Hopper.Core;
using Hopper.Core.Components.Basic;
using Hopper.Core.Stat;
using Hopper.TestContent.Floor;
using Hopper.Utils.Vector;
using NUnit.Framework;

namespace Hopper.Tests.Test_Content
{
    public class Slide_Tests
    {
        public readonly EntityFactory entityFactory;
        public Entity[] _iceFloors;


        public Slide_Tests()
        {
            InitScript.Init();
            entityFactory = new EntityFactory();
            Transform.AddTo(entityFactory, Layer.REAL);
            Stats.AddTo(entityFactory, Registry.Global._defaultStats);
            Ticking.AddTo(entityFactory).DefaultPreset();
            Displaceable.AddTo(entityFactory, ExtendedLayer.BLOCK).DefaultPreset();
            Moving.AddTo(entityFactory).DefaultPreset();
            Pushable.AddTo(entityFactory).DefaultPreset();
        }

        [Test]
        public void Setup()
        {
            World.Global = new World(4, 4);
            _iceFloors = new Entity[3];
            for (int i = 0; i < 3; i++)
            {
                _iceFloors[i] = World.Global.SpawnEntity(IceFloor.Factory, new IntVector2(i, 1));
            }
        }

        [Test]
        public void Test_Linear_Slide()
        {
            var entity = World.Global.SpawnEntity(entityFactory, new IntVector2(0, 1));
            entity.Move(IntVector2.Right);
            World.Global.Loop(); // first one just applies the status effect, they do not slide on first turn
            World.Global.Loop(); // e__ -> _e_
            World.Global.Loop(); // _e_ -> __e
            World.Global.Loop(); // __e -> ___e
            Assert.AreEqual(_iceFloors.Last().GetTransform().position + IntVector2.Right, entity.GetTransform().position);
        }

        [Test]
        public void Test_1()
        {
            // PushPlayer(new IntVector2(1, 0));

            // Assert.AreEqual(new IntVector2(1, 1), player.Pos);

            // world.Loop();

            // // the sliding does not affect the actual position directly. 
            // // It simply applies a status effect.
            // Assert.AreEqual(new IntVector2(1, 1), player.Pos);
            // Assert.That(Slide.Status.IsApplied(player));

            // // Voluntary moving gets prevented by changing the default action
            // var calculatedAction = player.Behaviors.Get<Controllable>()
            //     .ConvertVectorToAction(new IntVector2(0, 1));
            // Assert.IsNull(calculatedAction);

            // world.Loop();
            // Assert.AreEqual(new IntVector2(2, 1), player.Pos);

            // // However, if the player gets pushed, then they should slide it that direction
            // // that is, the player's displacement is in the direction that they stepped onto the 
            // // ice floor with. If the player gets pushed, so does this variable, reflecting
            // // the new direction.
            // // This time the player does get displaced after having been pushed, since the sliding
            // // has been already applied before the push. 
            // var ice_floor_below = world.SpawnEntity(IceFloor.Factory, new IntVector2(2, 2));
            // PushPlayer(new IntVector2(0, 1));

            // var x = (SlideData)player.Tinkers.GetStore(Slide.Status.m_tinker);

            // Assert.AreEqual(new IntVector2(2, 2), player.Pos);
            // Assert.AreEqual(new IntVector2(0, 1), x.currentDirection);
            // Assert.That(Slide.Status.IsApplied(player));

            // world.Loop();

            // Assert.AreEqual(new IntVector2(2, 3), player.Pos);
        }
    }

}