using System.Collections.Generic;
using Hopper.Core.ActingNS;
using Hopper.Core.Components.Basic;
using Hopper.Core.Items;
using Hopper.Utils.Vector;

namespace Hopper.Core.WorldNS
{
    public class World
    {
        public static World Global;

        public GridManager grid;
        public WorldStateManager state;

        public static readonly int NumOrders = System.Enum.GetValues(typeof(Order)).Length;
        public static readonly int NumLayers = System.Enum.GetValues(typeof(Layer)).Length;

        // For now, do this for the sake of tests and debugging
        // The world currently does not really need an id
        public World(int width, int height)
        {
            grid = new GridManager(width, height);
            state = new WorldStateManager();
        }

        public void Loop()
        {
            state.Loop();
            grid.ResetCellTriggers();
        }

        public int GetNextTimeFrame()
        {
            return state.NextTimeFrame();
        }

        public event System.Action<Entity> SpawnEntityEvent;

        // Spawning of particles. (A `Particle` being a `Scent` without a `Logent`)
        // 
        // So there's two ways to do this:
        //
        //  1. world is aware of particles at a basic level. The viewmodel subscribes to
        //     the spawnParticle event and reaches out to its dict of handlers, ignoring the
        //     event if no handler has been found for the specified event. The ids also need
        //     to be generated and stored, but the system is similar to that of entities.
        //  
        //  2. world is not responsible for particles. As a result, each `particle spawner`
        //     defines a static event, which has as arguments all the necessary metadata and
        //     the world object (since there may be multiple worlds at a time). The code
        //     defines custom handlers, called watchers, who manage what exactly happens. 
        //
        // For now, I'm opting for the second option 
        //
        // public event System.Action<int> SpawnParticleEvent;

        public Entity SpawnEntity(EntityFactory factory, IntVector2 pos, IntVector2 orientation)
        {
            System.Console.WriteLine($"Creating entity of factory id : {factory.id}");

            var entity = factory.Instantiate();
            entity.id = Registry.Global.RegisterRuntimeEntity(entity);

            if (entity.TryInitTransform(pos, orientation, out var transform))
            {
                grid.AddTransformNoEvent(transform);
                factory.InitInWorld(transform);
            }

            if (entity.TryInitActing(out var acting))
            {
                state.AddActor(acting);
            }

            if (entity.TryGetTicking(out var ticking))
            {
                ticking.Init(entity);
                state.AddTicking(ticking);
            }

            SpawnEntityEvent?.Invoke(entity);
            return entity;
        }

        public Entity SpawnEntity(EntityFactory factory, IntVector2 pos)
        {
            return SpawnEntity(factory, pos, IntVector2.Zero);
        }
    }
}