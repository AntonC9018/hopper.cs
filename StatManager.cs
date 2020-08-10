using System.Collections.Generic;
using Chains;
using Handle = MyLinkedList.MyListNode<Chains.WeightedEventHandler>;

namespace Core
{
    public class Multiplier
    {
        static IdGenerator s_idGenerator = new IdGenerator();
        public readonly int id = s_idGenerator.GetNextId();
        public Dictionary<string, int> additiveStats
            = new Dictionary<string, int>();
        public Dictionary<string, int> multiplicativeStats
            = new Dictionary<string, int>();
        public Dictionary<string, WeightedEventHandler> handlers
            = new Dictionary<string, WeightedEventHandler>();
    }

    public class StatCache
    {
        public int raw;
        public int additive = 0;
        public int multiplicative = 1;
        public Chain chain = new Chain();
    }

    public class StatManager
    {
        static Dictionary<string, int> s_defaultStats
            = new Dictionary<string, int>();

        static Dictionary<string, List<string>> s_categories
            = new Dictionary<string, List<string>>();
        public HashSet<string> m_isCategoryLoaded
            = new HashSet<string>();
        public Dictionary<string, StatCache> m_statCaches
            = new Dictionary<string, StatCache>();

        public StatManager()
        { }

        public StatManager(Dictionary<string, int> rawStats)
        {
            foreach (var (name, value) in rawStats)
            {
                m_statCaches[name] = new StatCache
                {
                    raw = value
                };
            }
        }

        public void RecalculateCache(string name)
        {
            // TODO: should sum and cache multipliers
            foreach (var (id, multiplier) in m_multipliers)
            {
                if (multiplier.additiveStats.ContainsKey(name))
                    m_statCaches[name].additive += multiplier.additiveStats[name];

                if (multiplier.multiplicativeStats.ContainsKey(name))
                    m_statCaches[name].multiplicative += multiplier.multiplicativeStats[name];

                if (multiplier.handlers.ContainsKey(name))
                    m_statCaches[name].chain.AddHandler(multiplier.handlers[name]);
            }
        }

        public void LazyLoadStat(string name)
        {
            if (!m_statCaches.ContainsKey(name))
            {
                m_statCaches[name] = new StatCache
                {
                    raw = s_defaultStats[name]
                };
            }
        }

        public int CalculateStat(string name)
        {
            var cache = m_statCaches[name];
            var stat = cache.raw;
            stat += cache.additive;
            stat = (int)(stat * cache.multiplicative);
            return stat;
        }

        // public int PassThroughStatChain(string name, int currentStat)
        // {
        //     if (m_statChains.ContainsKey(name))
        //     {
        //         // TODO: add a flexible event here, include a reference to entity
        //         var ev = new EventBase();

        //         return m_statChains[name];
        //     }
        // }

        public int GetStatSafe(string name)
        {
            LazyLoadStat(name);
            return CalculateStat(name);
        }

        public int GetStat(string name)
        {
            return CalculateStat(name);
        }

        public Dictionary<string, int> GetStatGroup(string[] names)
        {
            Dictionary<string, int> stats = new Dictionary<string, int>(names.Length);
            foreach (var name in names)
            {
                stats.Add(name, GetStatSafe(name));
            }
            return stats;
        }

        public Dictionary<string, int> GetStatCategory(string categoryName)
        {
            var names = s_categories[categoryName];
            if (m_isCategoryLoaded.Contains(categoryName))
            {
                Dictionary<string, int> stats = new Dictionary<string, int>(names.Count);
                foreach (var name in names)
                {
                    stats.Add(name, GetStat(name));
                }
                return stats;
            }
            m_isCategoryLoaded.Add(categoryName);
            return GetStatGroup(names.ToArray());
        }

        public void ResetStatsInCategory(string categoryName, int value)
        {
            foreach (string name in s_categories[categoryName])
            {
                m_statCaches[name] = new StatCache
                {
                    raw = value
                };
            }
        }

        public static void RegisterStat(string name, int defaultValue)
        {
            s_defaultStats[name] = defaultValue;
        }

        public static void RegisterCategory(string categoryName, List<string> names)
        {
            s_categories.Add(categoryName, names);
        }

        public static void RegisterStatInCategory(string categoryName, string name)
        {
            s_categories[categoryName].Add(name);
        }

        Dictionary<int, Multiplier> m_multipliers
            = new Dictionary<int, Multiplier>();
        Dictionary<int, Dictionary<string, Handle>> m_handles
            = new Dictionary<int, Dictionary<string, Handle>>();

        public void AddMultiplier(Multiplier multiplier)
        {
            foreach (var (name, value) in multiplier.additiveStats)
            {
                LazyLoadStat(name);
                m_statCaches[name].additive += value;
            }
            foreach (var (name, value) in multiplier.multiplicativeStats)
            {
                LazyLoadStat(name);
                m_statCaches[name].multiplicative += value;
            }
            foreach (var (name, handler) in multiplier.handlers)
            {
                LazyLoadStat(name);
                Handle handle = m_statCaches[name].chain.AddHandler(handler);
                m_handles[multiplier.id][name] = handle;
            }
            m_multipliers[multiplier.id] = multiplier;
        }
        public void RemoveMultiplier(Multiplier multiplier)
        {
            foreach (var (name, value) in multiplier.additiveStats)
            {
                m_statCaches[name].additive -= value;
            }
            foreach (var (name, value) in multiplier.multiplicativeStats)
            {
                m_statCaches[name].multiplicative -= value;
            }
            foreach (var (name, handle) in m_handles[multiplier.id])
            {
                m_statCaches[name].chain.RemoveHandler(handle);
            }
            m_multipliers.Remove(multiplier.id);
        }
    }
}