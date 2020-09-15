using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Items;
using Utils;

namespace Core
{
    public interface IHaveId
    {
        int Id { get; }
    }

    public static class IdMap
    {
        // public
        public static bool IsInRuntimePhase { get; private set; } = false;

        public static RuntimeIdMap<Entity, IEntityFactory> Entity =
            new RuntimeIdMap<Entity, IEntityFactory>();

        public static SetupIdMap<ITinker> Tinker = new SetupIdMap<ITinker>();
        public static SetupIdMap<Retoucher> Retoucher = new SetupIdMap<Retoucher>();
        public static SetupIdMap<IEntityFactory> EntityFactory = new SetupIdMap<IEntityFactory>();
        public static SetupIdMap<IItem> Items = new SetupIdMap<IItem>();
        public static SetupIdMap<IStatus> Status = new SetupIdMap<IStatus>();

        // static IdMap()
        // {
        //     Items.SetGlobalMap(Items.PackModMap());
        //     Tinker.SetGlobalMap(Tinker.PackModMap());
        //     Status.SetGlobalMap(Status.PackModMap());
        //     Retoucher.SetGlobalMap(Retoucher.PackModMap());
        //     EntityFactory.SetGlobalMap(EntityFactory.PackModMap());
        // }
    }

    public class SetupIdMap
    {
        // contains all items listed by mods in the order they were intialized.
        // we assume initialization takes place in the same sequence on both 
        // client and server, which is required for ids to match intended ones.
        protected Dictionary<string, List<IHaveId>> m_modMap = new Dictionary<string, List<IHaveId>>();
        // represents the local map. Once ids are generated at client, they are set in stone here
        protected List<IHaveId> m_clientMap = new List<IHaveId>();
        // provides a mapping of Server ids -> Client ids and backwards
        // this is used only for processing data coming from server and serialization (and backwards)
        protected Map<int, int> m_serverToClientMap = new Map<int, int>();


        public int Add(IHaveId item, string modName = "Default")
        {
            if (!m_modMap.ContainsKey(modName))
            {
                m_modMap[modName] = new List<IHaveId>();
            }
            m_modMap[modName].Add(item);
            m_clientMap.Add(item);
            return m_clientMap.Count - 1;
        }

        public class Instruction
        {
            public string modName;
            public int listIndex;

            public Instruction(string modName, int listIndex)
            {
                this.modName = modName;
                this.listIndex = listIndex;
            }
        }

        public void SetServerMap(List<Instruction> instructions)
        {
            m_serverToClientMap.Clear();
            for (int id = 0; id < instructions.Count; id++)
            {
                var instruction = instructions[id];
                var elementWithId = m_modMap[instruction.modName][instruction.listIndex];
                m_serverToClientMap.Add(id, elementWithId.Id);
            }
        }

        public List<Instruction> PackModMap()
        {
            var result = new List<Instruction>();
            foreach (var kvp in m_modMap)
            {
                for (int i = 0; i < kvp.Value.Count; i++)
                    result.Add(new Instruction(kvp.Key, i));
            }
            return result;
        }
    }

    public class SetupIdMap<T> : SetupIdMap where T : IHaveId
    {
        public IEnumerable<T> AllItems
        {
            get
            {
                foreach (int clientId in m_serverToClientMap.Reverse.Keys)
                    yield return (T)m_clientMap[clientId];
            }
        }

        public int Add(T item, string modName = "Default")
        {
            return base.Add(item, modName);
        }

        public T Map(int id)
        {
            return (T)m_clientMap[id];
        }
    }


    public interface IIdProviderProxy
    {
        Task<int> GetCurrentId();
    }

    public class IdProviderProxy : IIdProviderProxy
    {
        private int m_currentId = 0;

        public Task<int> GetCurrentId()
        {
            var task = new Task<int>(() => m_currentId);
            return task;
        }
    }

    public class InstanceAndFactory
    {
        public IHaveId instance;
        public IHaveId factory;

        public InstanceAndFactory(IHaveId instance, IHaveId factory)
        {
            this.instance = instance;
            this.factory = factory;
        }
    }

    public class RuntimeIdMap
    {
        protected Dictionary<int, InstanceAndFactory> m_map = new Dictionary<int, InstanceAndFactory>();
        protected int m_currentId;

        public async void RestoreState()
        {

            m_map.Clear();
            // var currentId = await idProxy.GetCurrentId();
            // if (IdMap.IsInRuntimePhase)
            // {
            //     m_currentId = currentId;
            // }
        }

        public IHaveId Map(int id)
        {
            if (m_map.ContainsKey(id))
                return m_map[id].instance;
            return null;
        }

        public void Remove(int id)
        {
            m_map.Remove(id);
        }

        public int Add(IHaveId instance, IHaveId factory)
        {
            m_currentId++;
            m_map[m_currentId] = new InstanceAndFactory(instance, factory);
            return m_currentId;
        }

        // public int GenerateId()
        // {
        //     m_currentId++;
        //     return m_currentId;
        // }
    }

    public class RuntimeIdMap<T, FactoryT> : RuntimeIdMap
        where T : IHaveId
    {
        public int Add(T instance, FactoryT factory)
        {
            return base.Add(instance, (IHaveId)factory);
        }
    }
}