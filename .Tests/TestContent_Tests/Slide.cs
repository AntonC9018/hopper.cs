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
            Acting.AddTo(entityFactory, null, Algos.SimpleAlgo, Order.Entity).DefaultPreset(entityFactory);
            entityFactory.InitInWorldFunc += t => t.entity.GetStats().Init();
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
            entity.Move(IntVector2.Right); // 1, 1
            // Once the entity stepped on ice, it gets the effect (entity modifier)
            Assert.True(entity.HasSlidingEntityModifier());

            // Trying to move is not going to help
            // The move itself is allowed, though. The action itself is replaced.
            // Right now any action gets replaced, but, in theory, it is easy to tweak.
            var acting = entity.GetActing();
            acting.nextAction = Moving.Action.Compile(IntVector2.Up); 
            // Make sure the action actually fails.
            Assert.False(acting.Activate()); // 2, 1
            // Make sure the position has changed
            Assert.AreEqual(new IntVector2(2, 1), entity.GetTransform().position);
            Assert.True(entity.HasSlidingEntityModifier());
            
            // Let's slide again
            Assert.False(acting.Activate()); // 3, 1
            Assert.AreEqual(new IntVector2(3, 1), entity.GetTransform().position);

            // Now that we're past the last ice floor, the modifier must remove itself.
            Assert.False(entity.HasSlidingEntityModifier());
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