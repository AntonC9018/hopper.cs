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

            var entityFactory = new EntityFactory();
            Transform.AddTo(entityFactory, Layer.REAL);
            Stats.AddTo(entityFactory, Registry.Global._defaultStats);
            Attackable.AddTo(entityFactory, Attackness.ALWAYS).DefaultPreset();
            Damageable.AddTo(entityFactory, new Health(1)).DefaultPreset();
            Displaceable.AddTo(entityFactory, Layer.WALL | Layer.REAL).DefaultPreset();
            Moving.AddTo(entityFactory).DefaultPreset();
            entityFactory.InitInWorldFunc = e => e.entity.GetStats().Init();

            var bindingFactory = new EntityFactory();
            Transform.AddTo(bindingFactory, Layer.REAL);
            Stats.AddTo(bindingFactory, Registry.Global._defaultStats);
            Binding.AddTo(bindingFactory, Layer.REAL, BoundEntityModifier.DefaultHookable).DefaultPreset();
            Damageable.AddTo(bindingFactory, new Health(1)).DefaultPreset();
            Attackable.AddTo(bindingFactory, Attackness.ALWAYS).DefaultPreset();
            bindingFactory.InitInWorldFunc = e => e.entity.GetStats().Init();

            World.Global = new World(3, 3);

            var spider = World.Global.SpawnEntity(bindingFactory, new IntVector2(1, 1));
            var entity = World.Global.SpawnEntity(entityFactory, new IntVector2(0, 0));

            spider.GetBinding().Activate(spider, new IntVector2(-1, -1));
            entity.Die();
        }
    }
}