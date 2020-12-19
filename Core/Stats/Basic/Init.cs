using System;
using Hopper.Core.Registries;

namespace Hopper.Core.Stats.Basic
{
    public static class BasicStats
    {
        public static void PrePatch(PatchArea patchArea)
        {
            Attack.Source.Resistance.InitPatchSubRegistry(patchArea);
            Push.Source.Resistance.InitPatchSubRegistry(patchArea);
            Status.Source.Resistance.InitPatchSubRegistry(patchArea);
        }

        public static void Patch(PatchArea patchArea)
        {
            Dig.Source.Patch(patchArea);
            Push.BasicSource.Patch(patchArea);
        }

        public static void PostPatch(PatchArea patchArea)
        {
            Attack.Source.Resistance.CreateDefaultFile(patchArea);
            Push.Source.Resistance.CreateDefaultFile(patchArea);
            Status.Source.Resistance.CreateDefaultFile(patchArea);
        }

    }
}