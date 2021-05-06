using System;
using System.Collections.Generic;
using Hopper.Core;
using Hopper.Core.ActingNS;
using Hopper.Core.Components.Basic;
using Hopper.Core.Items;
using Hopper.Core.Retouchers;
using Hopper.Core.Stat;
using Hopper.Core.Targeting;
using Hopper.Core.WorldNS;
using Hopper.TestContent;
using Hopper.TestContent.BindingNS;
using Hopper.TestContent.PinningNS;
using Hopper.TestContent.SimpleMobs;
using Hopper.Utils;
using Hopper.Utils.Vector;
using static Hopper.Utils.Vector.IntVector2;

namespace Hopper.Mine
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Hopper.Core.Main.Init();
            Hopper.TestContent.Main.Init();


            var entityFactory = new EntityFactory();
            Transform.AddTo(entityFactory, Layer.REAL);
            Stats.AddTo(entityFactory, Registry.Global._defaultStats);
            Stats.AddInitTo(entityFactory);
            Displaceable.AddTo(entityFactory, ExtendedLayer.BLOCK).DefaultPreset();
            Moving.AddTo(entityFactory).DefaultPreset();
            Pushable.AddTo(entityFactory).DefaultPreset();
            Ticking.AddTo(entityFactory);
            Acting.AddTo(entityFactory, null, Algos.SimpleAlgo, Order.Entity)
                .DefaultPreset(entityFactory);

            World.Global = new World(3, 3);
            var world = World.Global;

            var entity = World.Global.SpawnEntity(entityFactory, Zero);
            var water = World.Global.SpawnEntity(Water.Factory, Zero + Right);
            entity.Move(Right);

            entity.GetActing().ActivateWith(Moving.Action.Compile(Right));
            entity.GetActing().Activate();

        }
    }
}