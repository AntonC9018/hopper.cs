using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Core.FS
{
    public class Node
    {
    }

    public class File : Node
    {
        public virtual File Copy()
        {
            return (File)this.MemberwiseClone();
        }
    }
    public class Directory : Node
    {
        public Dictionary<string, Node> nodes =
            new Dictionary<string, Node>();

        public virtual File GetFile(string name)
        {
            return (File)nodes[name];
        }

        public void AddFile(string name, File file)
        {
            nodes.Add(name, file);
        }

        public void AddDirectory(string name, Directory directory)
        {
            nodes.Add(name, directory);
        }
    }

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

        D GetDirectoryBySplitPath(IEnumerable<string> dirNames)
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

        public D GetDirectory(string path)
        {
            var dirName = Split(path);
            return GetDirectoryBySplitPath(dirName);
        }

        public Node GetNode(string path)
        {
            var dirNames = Split(path);
            var dirPath = dirNames.Take(dirNames.Length - 1);
            var node = GetDirectoryBySplitPath(dirPath);
            var fileName = dirNames[dirNames.Length - 1];
            return node.nodes[fileName];
        }

        public File GetFile(string path)
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
            foreach (var (key, value) in dir.nodes)
            {
                System.Console.WriteLine(key.PadLeft(indentLevel));
                if (value is Directory)
                {
                    Debug((Directory)value, indentLevel + 4);
                }
            }
        }

        // TODO: lazy load
        protected void CopyDirectoryStructure(Directory from, D to)
        {
            foreach (var (name, node) in from.nodes)
            {
                if (node is Directory)
                {
                    var subdir = new D();
                    to.nodes.Add(name, subdir);
                    CopyDirectoryStructure((Directory)node, subdir);
                }
                else
                {
                    var copy = CopyFileNode((File)node);
                    to.nodes.Add(name, copy);
                }
            }
        }

        protected virtual File CopyFileNode(File node)
        {
            return ((File)node).Copy();
        }
    }
}
