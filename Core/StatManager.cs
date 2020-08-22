using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Chains;
using Handle = MyLinkedList.MyListNode<Chains.WeightedEventHandler>;

namespace Core
{

    public class Node
    {
    }

    public class File : Node
    {
        public virtual void _Add(File f)
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
                var newVal = oldVal + addVal;
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

        public override void _Add(File f)
        {
            // we assume it is the same type 
            var otherFile = (ArrayFile)f;
            var otherArray = otherFile.content;
            for (int i = 0; i < content.Count; i++)
            {
                content[i] += otherArray[i];
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

    public class StatNode : Node
    {
        public Chain chain;
        public File file;
    }

    public class StatEvent : EventBase
    {
        public File file;
    }

    public class StatDir : Directory
    {
        public override File GetFile(string fileName)
        {
            var node = (StatNode)nodes[fileName];
            var ev = new StatEvent { file = node.file };
            node.chain.Pass(ev);
            return ev.file;
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

    public class StatManager : FS<StatDir>
    {
        // contains either directories or files
        public static FS<Directory> s_defaultFS = new FS<Directory>();

        public StatManager()
        {
            CopyDirectoryStructure(s_defaultFS.m_baseDir, m_baseDir);
        }

        void CopyDirectoryStructure(Directory from, StatDir to)
        {
            foreach (var (name, node) in from.nodes)
            {
                if (node is Directory)
                {
                    var subdir = new StatDir();
                    to.nodes.Add(name, subdir);
                    CopyDirectoryStructure((Directory)node, subdir);
                }
                else if (node is File)
                {
                    var file = new StatNode();
                    file.chain = new Chain();
                    file.file = ((File)node).Copy();
                    to.nodes.Add(name, file);
                }
            }
        }
    }
}