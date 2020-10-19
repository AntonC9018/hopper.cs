using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Core.FS
{

    public class FS<D> where D : Directory, new()
    {
        public static readonly char s_separationChar = '/';
        public static string[] Split(string path)
        {
            return path.Split(s_separationChar);
        }

        public D m_baseDir;

        public D BaseDir
        {
            get => m_baseDir;
        }

        public FS()
        {
            m_baseDir = new D();
        }
        public FS(D d)
        {
            m_baseDir = d;
        }

        protected D GetDirectoryBySplitPath(IEnumerable<string> dirNames)
        {
            Directory dir = m_baseDir;
            foreach (var dirName in dirNames)
            {
                // getting a node should not require a virtual function
                // since it is always just nodes in an array
                dir = (Directory)dir.nodes[dirName];
            }
            return (D)dir;
        }

        protected D GetDirectoryBySplitPathLazy(IEnumerable<string> dirNames)
        {
            Directory dir = m_baseDir;
            foreach (var dirName in dirNames)
            {
                // getting a node should not require a virtual function
                // since it is always just nodes in an array
                if (!dir.nodes.ContainsKey(dirName))
                {
                    dir.nodes.Add(dirName, new D());
                }
                dir = (Directory)dir.nodes[dirName];
            }
            return (D)dir;
        }

        protected D GetDirectory(string path)
        {
            var dirName = Split(path);
            return GetDirectoryBySplitPath(dirName);
        }

        protected Node GetNode(string path)
        {
            var dirNames = Split(path);
            var dirPath = dirNames.Take(dirNames.Length - 1);
            var node = GetDirectoryBySplitPath(dirPath);
            var fileName = dirNames[dirNames.Length - 1];
            return node.nodes[fileName];
        }

        public File GetFileLazy(string path, File initialValue)
        {
            var dirNames = Split(path);
            var dirPath = dirNames.Take(dirNames.Length - 1);
            var node = GetDirectoryBySplitPathLazy(dirPath);
            var fileName = dirNames[dirNames.Length - 1];
            if (!node.nodes.ContainsKey(fileName))
            {
                System.Console.WriteLine($"The system doesn't contain {path}");
                node.nodes.Add(fileName, CopyFileNode(initialValue));
            }
            return node.GetFile(fileName);
        }

        protected File GetFile(string path)
        {
            var dirNames = Split(path);
            var dirPath = dirNames.Take(dirNames.Length - 1);
            var node = GetDirectoryBySplitPath(dirPath);
            var fileName = dirNames[dirNames.Length - 1];
            return node.GetFile(fileName);
        }

        public void Debug()
        {
            Debug(m_baseDir, 0);
        }

        public void Debug(Directory dir, int indentLevel = 0)
        {
            foreach (var kvp in dir.nodes)
            {
                System.Console.WriteLine(kvp.Key.PadLeft(indentLevel));
                if (kvp.Value is Directory)
                {
                    Debug((Directory)kvp.Value, indentLevel + 4);
                }
            }
        }

        protected void CopyDirectoryStructure(Directory from, D to)
        {
            foreach (var kvp in from.nodes)
            {
                if (kvp.Value is Directory)
                {
                    var subdir = new D();
                    to.nodes.Add(kvp.Key, subdir);
                    CopyDirectoryStructure((Directory)kvp.Value, subdir);
                }
                else
                {
                    var copy = CopyFileNode((File)kvp.Value);
                    to.nodes.Add(kvp.Key, copy);
                }
            }
        }

        protected virtual File CopyFileNode(File node)
        {
            return ((File)node).Copy();
        }
    }
}
