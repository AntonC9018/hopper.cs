using System;
using Hopper.Core;
using Hopper.Core.Components.Basic;
using Hopper.Core.Items;
using Hopper.Core.Predictions;
using Hopper.Core.Retouchers;
using Hopper.Core.Stat;
using Hopper.Core.Targeting;
using Hopper.TestContent;
using Hopper.TestContent.Bind;
using Hopper.TestContent.Floor;
using Hopper.TestContent.SimpleMobs;
using Hopper.Utils;
using Hopper.Utils.Vector;

namespace Hopper.Mine
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Hopper.Core.Main.Init();
            Hopper.TestContent.Main.Init();

            EntityFactory entityFactory;
            Entity[] _iceFloors;
            entityFactory = new EntityFactory();
            Transform.AddTo(entityFactory, Layer.REAL);
            Stats.AddTo(entityFactory, Registry.Global._defaultStats);
            Ticking.AddTo(entityFactory).DefaultPreset();
            Displaceable.AddTo(entityFactory, ExtendedLayer.BLOCK).DefaultPreset();
            Moving.AddTo(entityFactory).DefaultPreset();
            Pushable.AddTo(entityFactory).DefaultPreset();
            Acting.AddTo(entityFactory, null, Algos.SimpleAlgo, Order.Entity).DefaultPreset(entityFactory);
            entityFactory.InitInWorldFunc += t => t.entity.GetStats().Init();
            World.Global = new World(4, 4);
            var world = World.Global;
            _iceFloors = new Entity[3];
            for (int i = 0; i < 3; i++)
            {
                _iceFloors[i] = World.Global.SpawnEntity(IceFloor.Factory, new IntVector2(i, 1));
            }
            var ice1 = World.Global.SpawnEntity(IceFloor.Factory, new IntVector2(0, 2));
            var entity = World.Global.SpawnEntity(entityFactory, new IntVector2(0, 3));

            // We let the entity move up
            entity.Move(IntVector2.Up);

            // As a result, it gets the sliding modifier
            Assert.AreEqual(new IntVector2(0, 2), entity.GetTransform().position);

            // If we then push it to the right, it gets pushed successfully and loses the effect
            entity.BePushed(Push.Default(), IntVector2.Right); // < 1, 2 >

            // Now we take the entity back to the initial position
            // Moving back to the left should not apply the effect since there is nowhere to slide
            entity.Move(IntVector2.Left); // < 0, 2 >

            entity.Move(IntVector2.Down); // < 0, 3 >

            // Now we move up again
            entity.Move(IntVector2.Up);   // < 0, 2 >

            entity.GetActing().Activate();
        }
    }
}