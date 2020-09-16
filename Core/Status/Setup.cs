using Core.FS;

namespace Core
{
    public static class StatusSetup
    {
        public static MapFile ResFile;

        static StatusSetup()
        {
            Directory baseDir = StatManager.DefaultFS.BaseDir;
            ResFile = new MapFile();
            baseDir.AddFile("status_res", ResFile);
        }
    }
}