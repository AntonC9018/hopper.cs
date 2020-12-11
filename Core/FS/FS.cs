using System.Collections.Generic;
using System.Linq;

namespace Hopper.Core.FS
{
    // T is the base type of file this FS can hold
    public class FS<T> where T : File
    {
        protected static readonly char s_separationChar = '/';
        protected virtual string[] Split(string path)
        {
            return path.Split(s_separationChar);
        }

        private Directory m_baseDir;

        public Directory BaseDir
        {
            get => m_baseDir;
        }

        public FS()
        {
            m_baseDir = new Directory();
        }
        public FS(Directory baseDir)
        {
            m_baseDir = baseDir;
        }

        protected Directory GetDirectoryBySplitPath(IEnumerable<string> dirNames)
        {
            Directory dir = m_baseDir;
            foreach (var dirName in dirNames)
            {
                // getting a node should not require a virtual function
                // since it is always just nodes in an array
                dir = (Directory)dir.nodes[dirName];
            }
            return (Directory)dir;
        }

        protected Directory GetDirectoryBySplitPathLazy(IEnumerable<string> dirNames)
        {
            Directory dir = m_baseDir;
            foreach (var dirName in dirNames)
            {
                // getting a node should not require a virtual function
                // since it is always just nodes in an array
                if (!dir.nodes.ContainsKey(dirName))
                {
                    dir.nodes.Add(dirName, new Directory());
                }
                dir = (Directory)dir.nodes[dirName];
            }
            return (Directory)dir;
        }

        public List<Node> GetNodes(string path)
        {
            var splitPath = Split(path);
            List<Node> currentNodes = new List<Node> { m_baseDir };
            List<Node> buffer = new List<Node>();
            foreach (var segment in splitPath)
            {
                var temp = buffer;
                buffer = currentNodes;
                currentNodes = temp;
                currentNodes.Clear();

                foreach (var node in buffer)
                {
                    foreach (var n in ExpandPath(segment, (Directory)node))
                    {
                        currentNodes.Add(n);
                    }
                }
            }
            return currentNodes;
        }

        public List<T> GetFiles(string path) => GetNodes(path).ConvertAll(e => (T)e);

        // A copy of the initial value will be created for each of the nodes
        public List<Node> GetNodesLazy(string path, T defaultValue)
        {
            var splitPath = Split(path);
            List<Node> currentNodes = new List<Node> { m_baseDir };
            List<Node> buffer = new List<Node>();

            for (int i = 0; i < splitPath.Length; i++)
            {
                var temp = buffer;
                buffer = currentNodes;
                currentNodes = temp;
                currentNodes.Clear();

                var substitute = i == splitPath.Length - 1 ? defaultValue : null;

                foreach (var node in buffer)
                {
                    foreach (var n in ExpandPathLazy(splitPath[i], (Directory)node, substitute))
                    {
                        currentNodes.Add(n);
                    }
                }
            }

            return currentNodes;
        }

        public List<T> GetFilesLazy(string path, T defaultValue)
            => GetNodesLazy(path, defaultValue).ConvertAll(e => (T)e);

        protected IEnumerable<Node> ExpandPath(string pathItem, Directory currentDir)
        {
            if (pathItem == "*")
            {
                foreach (var item in currentDir.nodes.Values)
                {
                    yield return item;
                }
            }
            else
            {
                yield return currentDir.nodes[pathItem];
            }
        }

        protected IEnumerable<Node> ExpandPathLazy(string pathItem, Directory currentDir, T substitute)
        {
            if (pathItem == "*")
            {
                foreach (var item in currentDir.nodes.Values)
                {
                    yield return item;
                }
            }
            else
            {
                Node sub;
                if (substitute == null)
                {
                    sub = new Directory();
                }
                else
                {
                    sub = substitute.Copy();
                }

                if (!currentDir.nodes.ContainsKey(pathItem))
                {
                    currentDir.nodes.Add(pathItem, sub);
                }
                yield return currentDir.nodes[pathItem];
            }
        }

        public List<T> GetAllFiles()
        {
            List<Node> currentNodes = new List<Node>() { m_baseDir };
            List<Node> buffer = new List<Node>();
            List<T> result = new List<T>();

            while (currentNodes.Count > 0)
            {
                var temp = buffer;
                buffer = currentNodes;
                currentNodes = temp;
                currentNodes.Clear();

                foreach (var node in buffer)
                {
                    if (node is T)
                    {
                        result.Add((T)node);
                    }
                    else
                    {
                        foreach (var item in ((Directory)node).nodes.Values)
                        {
                            currentNodes.Add(item);
                        }
                    }
                }
            }
            return result;
        }

        public Directory GetDirectory(string path)
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

        // A copy of the initial value will be created
        public T GetFileLazy(string path, T initialValue)
        {
            var dirNames = Split(path);
            var dirPath = dirNames.Take(dirNames.Length - 1);
            var node = GetDirectoryBySplitPathLazy(dirPath);
            var fileName = dirNames[dirNames.Length - 1];
            if (!node.nodes.ContainsKey(fileName))
            {
                System.Console.WriteLine($"Lazy loading file {path}");
                node.nodes.Add(fileName, initialValue.Copy());
            }
            return (T)node.nodes[fileName];
        }

        public T GetFile(string path)
        {
            var dirNames = Split(path);
            var dirPath = dirNames.Take(dirNames.Length - 1);
            var node = GetDirectoryBySplitPath(dirPath);
            var fileName = dirNames[dirNames.Length - 1];
            return (T)node.nodes[fileName];
        }

        public void Debug() => Debug(m_baseDir, 2);

        public void Debug(Directory dir, int indentLevel)
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
    }
}
