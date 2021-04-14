using System.Collections.Generic;
using Hopper.Core.Items;
using Hopper.Utils.Vector;

namespace Hopper.Core
{
    public delegate void BringIntoGrid();

    public class World
    {
        public GridManager grid;
        public WorldStateManager State;

        public static readonly int NumPhases = System.Enum.GetNames(typeof(Phase)).Length;
        public static readonly int NumLayers = System.Enum.GetNames(typeof(Layer)).Length;

        // For now, do this for the sake of tests and debugging
        // The world currently does not really need an id
        public World(int width, int height)
        {
            PhaseLayerExtensions.ThrowIfPhasesAreWrong();
            grid = new GridManager(width, height);
            State = new WorldStateManager();
        }

        public void Loop()
        {
            State.Loop();
        }

        public int GetNextTimeFrame()
        {
            return State.NextTimeFrame();
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

        private Entity SpawnEntityNoEvent(EntityFactory factory, IntVector2 pos, IntVector2 orientation)
        {
            var entity = factory.Instantiate();
            entity.id = Registry.Global.RegisterRuntimeEntity(entity);
            var transform = entity.InitTransform(pos, orientation);
            grid.GetCellAt(pos).m_transforms.Add(transform);
            return entity;
        }

        public Entity SpawnHangingEntity(EntityFactory factory, IntVector2 pos, IntVector2 orientation)
        {
            var entity = factory.Instantiate();
            entity.id = Registry.Global.RegisterRuntimeEntity(entity);
            entity.InitTransform(pos, orientation);
            State.AddEntity(entity);
            SpawnEntityEvent?.Invoke(entity);
            return entity;
        }

        public T SpawnEntity<T>(
            IFactory<T> EntityFactory, IntVector2 pos, IntVector2 orientation) where T : Entity
        {
            System.Console.WriteLine($"Creating entity of factory id : {EntityFactory.Id}");
            var entity = SpawnEntityNoEvent(EntityFactory, pos, orientation);
            State.AddEntity(entity);
            SpawnEntityEvent?.Invoke(entity);
            return entity;
        }

        public T SpawnEntity<T>(
            IFactory<T> EntityFactory, IntVector2 pos) where T : Entity
        {
            return SpawnEntity(EntityFactory, pos, IntVector2.Zero);
        }


        public T SpawnPlayer<T>(
            IFactory<T> EntityFactory, IntVector2 pos, IntVector2 orientation) where T : Entity
        {
            var entity = SpawnEntityNoEvent(EntityFactory, pos, orientation);
            State.AddPlayer(entity);
            SpawnEntityEvent?.Invoke(entity);
            return entity;
        }

        public T SpawnPlayer<T>(
            IFactory<T> EntityFactory, IntVector2 pos) where T : Entity
        {
            return SpawnPlayer(EntityFactory, pos, IntVector2.Zero);
        }


        public DroppedItem SpawnDroppedItem(
            IItem item, IntVector2 pos, IntVector2 orientation)
        {
            var entity = SpawnEntityNoEvent(DroppedItem.Factory, pos, orientation);
            entity.Item = item;
            SpawnEntityEvent?.Invoke(entity);
            return entity;
        }

        public DroppedItem SpawnDroppedItem(IItem item, IntVector2 pos)
        {
            return SpawnDroppedItem(item, pos, IntVector2.Zero);
        }

        public void InitializeWorldEvents()
        {
            foreach (var worldEvent in m_currentRepository.GetPatchSubRegistry<IWorldEvent>().patches.Values)
            {
                m_events.Add(worldEvent.Id, worldEvent.GetCopy());
            };
        }
    }
}