using Core.FS;

namespace Core
{
    public static class DigSetup
    {
        static DigSetup()
        {
            Directory baseDir = StatManager.DefaultFS.BaseDir;
            StatFile digStatFile = new Dig
            {
                power = 0,
                damage = 1,
                pierce = 10
            };
            baseDir.AddFile("dig", digStatFile);
            System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(AttackSetup).TypeHandle);
            Dig.Source = new Attack.Source();
        }
    }
}