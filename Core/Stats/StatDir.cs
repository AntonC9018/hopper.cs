using Core.FS;

namespace Core.Stats
{
    public class StatDir : Directory
    {
        public override File GetFile(string fileName)
        {
            var node = (StatFileContainer)nodes[fileName];
            var ev = new StatEvent { file = (File)node.file.Copy() };
            node.chain.Pass(ev);
            return ev.file;
        }
    }
}