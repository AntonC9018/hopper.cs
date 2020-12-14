using System;

namespace Hopper.Core.Stats.Basic
{
    public static class BasicStats
    {
        public static void Patch(Repository repository)
        {
            Attack.Source.Resistance.InitPatchSubRegistry(repository);
            Push.Source.Resistance.InitPatchSubRegistry(repository);
            Status.Source.Resistance.InitPatchSubRegistry(repository);

            Dig.Source.Patch(repository);
            Push.BasicSource.Patch(repository);
        }

        public static void AfterPatch(Repository repository)
        {
            Attack.Source.Resistance.CreateDefaultFile(repository);
            Push.Source.Resistance.CreateDefaultFile(repository);
            Status.Source.Resistance.CreateDefaultFile(repository);
        }
    }
}