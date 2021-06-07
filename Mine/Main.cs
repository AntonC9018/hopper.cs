
using Hopper.Core;
using Hopper.Core.Components.Basic;
using Hopper.Core.Mods;
using Hopper.Core.Stat;
using Hopper.Core.Targeting;
using Hopper.Core.WorldNS;
using Hopper.TestContent.BindingNS;
using Hopper.Utils.Vector;

namespace Mine
{
    public class Program
    {
        public static void Main()
        {
            Hopper.Core.Main.Init();
            Hopper.TestContent.Main.Init();

            var entityFactory = new EntityFactory();
            Transform.AddTo(entityFactory, Layers.REAL, TransformFlags.Default);
            Stats.AddTo(entityFactory, Registry.Global.Stats._map);
            Attackable.AddTo(entityFactory, Attackness.ALWAYS).DefaultPreset();
            Damageable.AddTo(entityFactory, new Health(1)).DefaultPreset();
            Displaceable.AddTo(entityFactory, Layers.WALL | Layers.REAL).DefaultPreset();
            Moving.AddTo(entityFactory).DefaultPreset();
            // Cannot be bound unless it has MoreChains
            MoreChains.AddTo(entityFactory, Registry.Global.MoreChains._map);
            

            var bindingFactory = new EntityFactory();
            Transform.AddTo(bindingFactory, Layers.REAL, TransformFlags.Default);
            Stats.AddTo(bindingFactory, Registry.Global.Stats._map);
            Binding.AddTo(bindingFactory, Layers.REAL, BoundEntityModifierDefault.Hookable).DefaultPreset();
            Damageable.AddTo(bindingFactory, new Health(1)).DefaultPreset();
            Attackable.AddTo(bindingFactory, Attackness.ALWAYS).DefaultPreset();

            World.Global = new World(3, 3);
            var spider = World.Global.SpawnEntity(bindingFactory, new IntVector2(1, 1));
            var entity = World.Global.SpawnEntity(entityFactory, new IntVector2(0, 0));
            spider.GetBinding().Activate(spider, new IntVector2(-1, -1));
        }
    }
}