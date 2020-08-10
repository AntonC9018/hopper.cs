using System.Collections.Generic;
using Chains;

namespace Core
{
    public class Multiplier { }

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

        Dictionary<int, Multiplier> m_multipliers;

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
        }

        public void LazyLoadStat(string name)
        {
            if (!m_statCaches.ContainsKey(name))
            {
                m_statCaches[name] = new StatCache
                {
                    raw = s_defaultStats[name]
                };
                RecalculateCache(name);
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
    }
}