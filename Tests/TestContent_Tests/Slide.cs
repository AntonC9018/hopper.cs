using Hopper.Core;
using Hopper.Core.Components.Basic;
using Hopper.Core.Stat;
using Hopper.TestContent.SlidingNS;
using Hopper.Utils.Vector;
using NUnit.Framework;
using Hopper.Core.WorldNS;
using Hopper.Core.ActingNS;

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
            Transform.AddTo(entityFactory, Layers.REAL, TransformFlags.Default);
            Stats.AddTo(entityFactory, Registry.Global.Stats._map);
            
            Ticking.AddTo(entityFactory).DefaultPreset();
            Displaceable.AddTo(entityFactory, Layers.BLOCK).DefaultPreset();
            Moving.AddTo(entityFactory).DefaultPreset();
            Pushable.AddTo(entityFactory).DefaultPreset();
            Acting.AddTo(entityFactory, null, Algos.SimpleAlgo, Order.Entity).DefaultPreset(entityFactory);
        }

        [SetUp]
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
            //
            // _ _ _ _        _ _ _ _ 
            // i i i _        e i i _         i are ice floor
            // _ _ _ _    ->  _ _ _ _         e is the entity spawned on top of ice
            // _ _ _ _        _ _ _ _ 
            //
            var entity = World.Global.SpawnEntity(entityFactory, new IntVector2(0, 1));
            entity.Move(IntVector2.Right); // 1, 1
            // Once the entity stepped on ice, it gets the effect (entity modifier)
            Assert.True(entity.HasSlidingEntityModifier());

            // Trying to move is not going to help
            // The move itself is allowed, though. The action itself is replaced.
            // Right now any action gets replaced, but, in theory, it is easy to tweak.
            var acting = entity.GetActing();
            acting.SetPotentialAction(Moving.Action.Compile(IntVector2.Up)); 
            // Make sure the action actually ~fails~ succeeds.
            Assert.True(acting.Activate()); // 2, 1
            // Make sure the position has changed
            Assert.AreEqual(new IntVector2(2, 1), entity.GetTransform().position);
            Assert.True(entity.HasSlidingEntityModifier());
            
            // Let's slide again
            Assert.True(acting.Activate()); // 3, 1
            Assert.AreEqual(new IntVector2(3, 1), entity.GetTransform().position);

            // Now that we're past the last ice floor, the modifier must remove itself.
            Assert.False(entity.HasSlidingEntityModifier());
        }

        [Test]
        public void BeingPushed_WhileSliding()
        {
            // Let's add some ice below to set things up
            // _ _ _ _        _ _ _ _ 
            // i i i _        i i i _         i are ice floor
            // _ _ _ _    ->  i _ _ _         e is the entity
            // _ _ _ _        e _ _ _ 
            //
            var ice1 = World.Global.SpawnEntity(IceFloor.Factory, new IntVector2(0, 2));
            var entity = World.Global.SpawnEntity(entityFactory, new IntVector2(0, 3));
            
            // We let the entity move up
            entity.Move(IntVector2.Up);

            // _ _ _ _ 
            // i i i _ 
            // e _ _ _ 
            // _ _ _ _ 

            // As a result, it gets the sliding modifier
            Assert.AreEqual(new IntVector2(0, 2), entity.GetTransform().position);
            Assert.True(entity.HasSlidingEntityModifier());

            // If we then push it to the right, it gets pushed successfully and loses the effect
            
            // _ _ _ _ 
            // i i i _ 
            // i e _ _ 
            // _ _ _ _ 

            entity.BePushed(Push.Default(), IntVector2.Right); // < 1, 2 >
            Assert.AreEqual(new IntVector2(1, 2), entity.GetTransform().position);
            Assert.False(entity.HasSlidingEntityModifier());

            // Now we take the entity back to the initial position
            // Moving back to the left should not apply the effect since there is nowhere to slide
            
            // _ _ _ _ 
            // i i i _ 
            // e _ _ _ 
            // _ _ _ _ 

            entity.Move(IntVector2.Left); // < 0, 2 >
            Assert.AreEqual(new IntVector2(0, 2), entity.GetTransform().position);
            Assert.False(entity.HasSlidingEntityModifier());

            // _ _ _ _ 
            // i i i _ 
            // i _ _ _ 
            // e _ _ _ 

            entity.Move(IntVector2.Down); // < 0, 3 >
            Assert.AreEqual(new IntVector2(0, 3), entity.GetTransform().position);
            Assert.False(entity.HasSlidingEntityModifier());

            // Now we move up again
            entity.Move(IntVector2.Up);   // < 0, 2 >
            Assert.AreEqual(new IntVector2(0, 2), entity.GetTransform().position);
            Assert.True(entity.HasSlidingEntityModifier());

            // This time we skip an action.
            // The action is null but it will be modified by the effect.
            entity.GetActing().Activate();
            Assert.AreEqual(new IntVector2(0, 1), entity.GetTransform().position);
            Assert.True(entity.HasSlidingEntityModifier());

            // Now we get pushed to the right
            // The effect is still applied, but the direction of sliding is different
            entity.BePushed(Push.Default(), IntVector2.Right);
            Assert.AreEqual(new IntVector2(1, 1), entity.GetTransform().position);
            Assert.True(entity.HasSlidingEntityModifier());
            Assert.AreEqual(IntVector2.Right, entity.GetSlidingEntityModifier().directionOfSliding);

            // Say we try moving up through the acting now, but we should keep moving right
            entity.GetActing().ActivateWith(Moving.Action.Compile(IntVector2.Up));
            Assert.AreEqual(new IntVector2(2, 1), entity.GetTransform().position);
            Assert.True(entity.HasSlidingEntityModifier());
        }
    }

}