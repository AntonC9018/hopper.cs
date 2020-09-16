using Core.FS;

namespace Core
{
    public static class AttackSetup
    {
        public static MapFile SourceResFile;

        static AttackSetup()
        {
            Directory baseDir = StatManager.DefaultFS.BaseDir;

            Directory attackDir = new Directory();

            SourceResFile = new MapFile();
            StatFile resFile = new Attack.Resistance
            {
                armor = 0,
                minDamage = 1,
                maxDamage = 10,
                pierce = 1
            };
            StatFile attackStatFile = new Attack
            {
                sourceId = 0,
                power = 1,
                damage = 1,
                pierce = 1
            };
            StatFile pushStatFile = new Push
            {
                source = 0,
                power = 1,
                distance = 1,
                pierce = 1
            };

            baseDir.AddDirectory("attacked", attackDir);
            attackDir.AddFile("source_res", SourceResFile);
            attackDir.AddFile("res", resFile);
            baseDir.AddFile("attack", attackStatFile);
            baseDir.AddFile("push", pushStatFile);

            Attack.BasicSource = new Attack.Source();
        }
    }
}