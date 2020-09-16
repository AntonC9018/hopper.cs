using Core.FS;

namespace Core
{
    public static class PushSetup
    {
        public static MapFile SourceResFile;
        static void SetupStats()
        {
            Directory baseDir = StatManager.DefaultFS.BaseDir;

            Directory pushDir = new Directory();
            SourceResFile = new MapFile();
            StatFile resFile = new Push.Resistance
            {
                pierce = 1
            };

            baseDir.AddDirectory("pushed", pushDir);
            pushDir.AddFile("source_res", SourceResFile);
            pushDir.AddFile("res", resFile);

            Push.BasicSource = new Push.Source();
        }
    }
}