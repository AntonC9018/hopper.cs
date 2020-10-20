using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Utils;

namespace Core.FS
{
    public class FS<D, F>
        where D : Directory, new()
        where F : File
    {
        protected static readonly char s_separationChar = '/';
        protected virtual string[] Split(string path)
        {
            return path.Split(s_separationChar);
        }

        private D m_baseDir;

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
            D dir = m_baseDir;
            foreach (var dirName in dirNames)
            {
                // getting a node should not require a virtual function
                // since it is always just nodes in an array
                dir = (D)dir.nodes[dirName];
            }
            return (D)dir;
        }

        protected D GetDirectoryBySplitPathLazy(IEnumerable<string> dirNames)
        {
            D dir = m_baseDir;
            foreach (var dirName in dirNames)
            {
                // getting a node should not require a virtual function
                // since it is always just nodes in an array
                if (!dir.nodes.ContainsKey(dirName))
                {
                    dir.nodes.Add(dirName, new D());
                }
                dir = (D)dir.nodes[dirName];
            }
            return (D)dir;
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
                    foreach (var n in ExpandPath(segment, (D)node))
                    {
                        currentNodes.Add(n);
                    }
                }
            }
            return currentNodes;
        }

        public List<F> GetFiles(string path) => GetNodes(path).ConvertAll(e => (F)e);

        public List<Node> GetNodesLazy(string path, F defaultValue)
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
                    foreach (var n in ExpandPathLazy(splitPath[i], (D)node, substitute))
                    {
                        currentNodes.Add(n);
                    }
                }
            }

            return currentNodes;
        }

        public List<F> GetFilesLazy(string path, F defaultValue)
            => GetNodesLazy(path, defaultValue).ConvertAll(e => (F)e);

        protected IEnumerable<Node> ExpandPath(string pathItem, D currentDir)
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

        protected IEnumerable<Node> ExpandPathLazy(string pathItem, D currentDir, F substitute)
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
                    sub = new D();
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

        public List<F> GetAllFiles()
        {
            List<Node> currentNodes = new List<Node>() { m_baseDir };
            List<Node> buffer = new List<Node>();
            List<F> result = new List<F>();

            while (currentNodes.Count > 0)
            {
                var temp = buffer;
                buffer = currentNodes;
                currentNodes = temp;
                currentNodes.Clear();

                foreach (var node in buffer)
                {
                    if (node is F)
                    {
                        result.Add((F)node);
                    }
                    else
                    {
                        foreach (var item in ((D)node).nodes.Values)
                        {
                            currentNodes.Add(item);
                        }
                    }
                }
            }
            return result;
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

        public F GetFileLazy(string path, F initialValue)
        {
            var dirNames = Split(path);
            var dirPath = dirNames.Take(dirNames.Length - 1);
            var node = GetDirectoryBySplitPathLazy(dirPath);
            var fileName = dirNames[dirNames.Length - 1];
            if (!node.nodes.ContainsKey(fileName))
            {
                System.Console.WriteLine($"The system doesn't contain {path}");
                node.nodes.Add(fileName, initialValue.Copy());
            }
            return (F)node.nodes[fileName];
        }

        public F GetFile(string path)
        {
            var dirNames = Split(path);
            var dirPath = dirNames.Take(dirNames.Length - 1);
            var node = GetDirectoryBySplitPath(dirPath);
            var fileName = dirNames[dirNames.Length - 1];
            return (F)node.nodes[fileName];
        }

        public void Debug() => Debug(m_baseDir, 2);

        public void Debug(D dir, int indentLevel)
        {
            foreach (var kvp in dir.nodes)
            {
                System.Console.WriteLine(kvp.Key.PadLeft(indentLevel));
                if (kvp.Value is D)
                {
                    Debug((D)kvp.Value, indentLevel + 4);
                }
            }
        }

        public void CopyDirectoryStructure(D from, D to)
        {
            foreach (var kvp in from.nodes)
            {
                if (kvp.Value is D)
                {
                    var subdir = new D();
                    to.nodes.Add(kvp.Key, subdir);
                    CopyDirectoryStructure((D)kvp.Value, subdir);
                }
                else
                {
                    var copy = ((F)kvp.Value).Copy();
                    to.nodes.Add(kvp.Key, copy);
                }
            }
        }
    }
}
