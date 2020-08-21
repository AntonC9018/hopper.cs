using System.Collections.Generic;
using System.Linq;
using Chains;
using Handle = MyLinkedList.MyListNode<Chains.WeightedEventHandler>;

namespace Core
{

    public static class Path
    {
        public static readonly char s_separationChar = '/';

        public static string[] Split(string path)
        {
            return path.Split(s_separationChar);
        }


    }

    public class Directory<File>
    {
        public Dictionary<string, Directory<File>> directories =
            new Dictionary<string, Directory<File>>();
        public Dictionary<string, File> files =
            new Dictionary<string, File>();
    }

    public class StatDir : Directory<int>
    {
        public class Event : EventBase
        {
            public Dictionary<string, int> stats;
        }

        public Chain chain = new Chain();
        public Dictionary<string, int> cache;

        public Dictionary<string, int> GetStats()
        {
            var stats = new Dictionary<string, int>(cache);
            chain.Pass(new Event { stats = stats });
            return stats;
        }

    }

    public class StatManager
    {
        public static Directory<int> s_defaultStatsDir = new Directory<int>();
        public StatDir m_baseDir;


        public StatManager()
        {
            m_baseDir = new StatDir();
            CopyDirectoryStructure(s_defaultStatsDir, m_baseDir);
        }

        void CopyDirectoryStructure(Directory<int> from, StatDir to)
        {
            to.cache = new Dictionary<string, int>(from.files);
            foreach (var (name, dir) in from.directories)
            {
                var subdir = new StatDir();
                to.directories.Add(name, subdir);
                CopyDirectoryStructure(dir, subdir);
            }
        }

        StatDir GetDirectoryByDirNames(string[] dirNames)
        {
            var dir = m_baseDir;
            foreach (var dirName in dirNames)
            {
                dir = (StatDir)dir.directories[dirName];
            }
            return dir;
        }

        Dictionary<string, int> GetStatsByDirNames(string[] dirNames)
        {
            return GetDirectoryByDirNames(dirNames).GetStats();
        }

        int GetStatByDirNames(string[] dirNames)
        {
            var len = dirNames.Length;
            var dirPath = (string[])dirNames.Take(len - 1);
            var statName = dirNames[len - 1];
            var stats = GetStatsByDirNames(dirPath);
            return stats[statName];
        }

        public StatDir GetDirectory(string path)
        {
            return GetDirectoryByDirNames(Path.Split(path));
        }

        public Dictionary<string, int> GetStats(string path)
        {
            return GetStatsByDirNames(Path.Split(path));
        }

        public int GetStat(string path)
        {
            return GetStatByDirNames(Path.Split(path));
        }
    }
}