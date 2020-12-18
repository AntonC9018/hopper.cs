using System.Collections.Generic;
using Hopper.Core.Items;
using Hopper.Core.Registry;
using Hopper.Utils.Vector;

namespace Hopper.Core
{
    public delegate void BringIntoGrid();

    public class World : IHaveId
    {
        public GridManager Grid { get; private set; }
        public WorldStateManager State { get; private set; }
        public PoolContainer m_pools = new PoolContainer();
        public Dictionary<int, IWorldEvent> m_events;
        public PatchArea m_currentRepository;

        public static readonly int NumPhases = System.Enum.GetNames(typeof(Phase)).Length;
        public static readonly int NumLayers = System.Enum.GetNames(typeof(Layer)).Length;

        public int Id => m_id;
        private int m_id;

        // For now, do this for the sake of tests and debugging
        // The world currently does not really need an id
        public World(int width, int height)
        {
            PhaseLayerExtensions.ThrowIfPhasesAreWrong();
            Grid = new GridManager(width, height);
            State = new WorldStateManager();
            m_events = new Dictionary<int, IWorldEvent>();
        }

        public World(int width, int height, PatchArea patchArea)
        {
            PhaseLayerExtensions.ThrowIfPhasesAreWrong();
            Grid = new GridManager(width, height);
            State = new WorldStateManager();
            m_events = new Dictionary<int, IWorldEvent>();
            m_id = 1;
            m_currentRepository = patchArea;
        }

        public void Loop()
        {
            State.Loop();
        }

        public int GetNextTimeFrame()
        {
            return State.GetNextTimeFrame();
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

        private T SpawnEntityNoEvent<T>(
            IFactory<T> EntityFactory, IntVector2 pos, IntVector2 orientation) where T : Entity
        {
            var entity = EntityFactory.Instantiate();
            State.RegisterEntity(entity, EntityFactory);
            entity.Init(pos, orientation, this);
            entity.ResetInGrid();
            return entity;
        }

        public T SpawnHangingEntity<T>(
            IFactory<T> EntityFactory, IntVector2 pos, IntVector2 orientation) where T : Entity
        {
            var entity = EntityFactory.Instantiate();
            State.RegisterEntity(entity, EntityFactory);
            entity.Init(pos, orientation, this);
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

        // public void SpawnParticle(int id)
        // {
        //     SpawnParticleEvent?.Invoke(id);
        // }

        public void InitializeWorldEvents()
        {
            foreach (var worldEvent in m_currentRepository.GetPatchSubRegistry<IWorldEvent>().patches.Values)
            {
                m_events.Add(worldEvent.Id, worldEvent.GetCopy());
            };
        }
    }
}