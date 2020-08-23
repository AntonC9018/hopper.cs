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
        public virtual void _Add(File f, int sign)
        {
            // let's do it the dumbest way so that it works
            // maybe I'll figure out a better solution later
            var type = f.GetType();
            if (type != this.GetType())
            {
                throw new System.Exception("Can't add files of different types");
            }
            foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.Public))
            {
                var oldVal = (int)field.GetValue(this);
                var addVal = (int)field.GetValue(f);
                var newVal = oldVal + sign * addVal;
                field.SetValue(this, newVal);
            }
        }
        public File Copy()
        {
            return (File)this.MemberwiseClone();
        }
    }

    public class ArrayFile : File
    {
        public List<int> content = new List<int>();

        public override void _Add(File f, int sign)
        {
            // we assume it is the same type 
            var otherFile = (ArrayFile)f;
            var otherArray = otherFile.content;
            for (int i = 0; i < content.Count; i++)
            {
                content[i] += otherArray[i] * sign;
            }
        }

        public int this[int index]
        {
            get => content[index];
            set => content[index] = value;
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

    public class FS<T> where T : Directory, new()
    {
        public static readonly char s_separationChar = '/';
        public static string[] Split(string path)
        {
            return path.Split(s_separationChar);
        }

        public T m_baseDir;

        public T BaseDir
        {
            get => m_baseDir;
        }

        public FS()
        {
            m_baseDir = new T();
        }
        public FS(T d)
        {
            m_baseDir = d;
        }

        T GetDirectoryBySplitPath(IEnumerable<string> dirNames)
        {
            Directory dir = m_baseDir;
            foreach (var dirName in dirNames)
            {
                // getting a node should not require a virtual function
                // since it is always just nodes in an array
                dir = (Directory)dir.nodes[dirName];
            }
            return (T)dir;
        }

        public T GetDirectory(string path)
        {
            var dirName = Split(path);
            return GetDirectoryBySplitPath(dirName);
        }

        public Node GetNode(string path)
        {
            var dirNames = Split(path);
            var dirPath = dirNames.Take(dirNames.Length - 1);
            var node = (T)GetDirectoryBySplitPath(dirPath);
            var fileName = dirNames[dirNames.Length - 1];
            return node.nodes[fileName];
        }

        public File GetFile(string path)
        {
            var dirNames = Split(path);
            var dirPath = dirNames.Take(dirNames.Length - 1);
            var node = (T)GetDirectoryBySplitPath(dirPath);
            var fileName = dirNames[dirNames.Length - 1];
            return node.GetFile(fileName);
        }

        void Debug()
        {
            Debug(m_baseDir, 0);
        }

        void Debug(Directory dir, int indentLevel = 0)
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
    }
}
