using System.Collections.Generic;
using Chains;

namespace Core
{
    public class StatManager
    {
        static Dictionary<string, int> s_defaultStats
            = new Dictionary<string, int>();

        static List<List<string>> s_categories
            = new List<List<string>>();

        public Dictionary<string, int> m_rawStats;
        public bool[] isCategoryLoaded = new bool[s_categories.Count];
        Dictionary<string, int> m_cachedAdditiveStats
            = new Dictionary<string, int>();
        Dictionary<string, float> m_cachedMultiplicativeStats
            = new Dictionary<string, float>();
        Dictionary<string, Chain> m_statChains
            = new Dictionary<string, Chain>();

        public StatManager(Dictionary<string, int> rawStats)
        {
            m_rawStats = rawStats;
        }

        public int GetRawStatSafe(string name)
        {
            if (m_rawStats.ContainsKey(name))
            {
                return m_rawStats[name];
            }
            m_rawStats[name] = s_defaultStats[name];
            return m_rawStats[name];
        }

        public int GetAdditiveStat(string name)
        {
            if (m_cachedAdditiveStats.ContainsKey(name))
            {
                return m_cachedAdditiveStats[name];
            }
            m_cachedAdditiveStats[name] = 0;
            return 0;
        }

        public float GetMultiplicativeStat(string name)
        {
            if (m_cachedMultiplicativeStats.ContainsKey(name))
            {
                return m_cachedMultiplicativeStats[name];
            }
            m_cachedMultiplicativeStats[name] = 1;
            return 1;
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
            var stat = GetRawStatSafe(name);
            stat += GetAdditiveStat(name);
            stat = (int)(stat * GetMultiplicativeStat(name));
            // TODO: pass through chain
            // stat = PassThroughStatChain(name, stat)
            return stat;
        }

        public int GetStat(string name)
        {
            var stat = m_rawStats[name];
            stat += m_cachedAdditiveStats[name];
            stat = (int)(stat * m_cachedMultiplicativeStats[name]);
            return stat;
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

        public Dictionary<string, int> GetStatCategory(int categoryIndex)
        {
            var names = s_categories[categoryIndex];
            if (isCategoryLoaded[categoryIndex])
            {
                Dictionary<string, int> stats = new Dictionary<string, int>(names.Count);
                foreach (var name in names)
                {
                    stats.Add(name, GetStat(name));
                }
                return stats;
            }
            isCategoryLoaded[categoryIndex] = true;
            return GetStatGroup(names.ToArray());
        }

        public void ResetStatsInCategory(int categoryIndex, int value)
        {
            foreach (string name in s_categories[categoryIndex])
            {
                m_rawStats[name] = value;
            }
        }

        public void ResetStatsInCategory(int categoryIndex, Dictionary<string, int> values)
        {
            foreach (string name in s_categories[categoryIndex])
            {
                m_rawStats[name] = values[name];
            }
        }

        public void ResetStatsInCategory(int categoryIndex, Dictionary<string, int> values, int defaultValue)
        {
            foreach (string name in s_categories[categoryIndex])
            {
                if (values.ContainsKey(name))
                {
                    m_rawStats[name] = values[name];
                }
                else
                {
                    m_rawStats[name] = defaultValue;
                }
            }
        }

        public static void RegisterStat(string name, int defaultValue)
        {
            s_defaultStats[name] = defaultValue;
        }

        public static int RegisterCategory(List<string> name)
        {
            s_categories.Add(name);
            return s_categories.Count - 1;
        }

        public static void AddStatToCategory(int categoryIndex, string name)
        {
            s_categories[categoryIndex].Add(name);
        }
    }
}