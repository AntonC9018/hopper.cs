namespace Hopper.Core.Stats.Basic
{
    public static class BasicStats
    {
        public static void Init(Registry registry)
        {
            SourceBase<Attack.Source>.InitOn(registry);
            Attack.BasicSource.RegisterOn(registry);

            Dig.Source.RegisterOn(registry);

            SourceBase<Push.Source>.InitOn(registry);
            Push.BasicSource.RegisterOn(registry);

            SourceBase<IStatus>.InitOn(registry);

            registry.RunPatchingEvent += reg =>
            {
                Attack.Path.PatchDefaultFile(reg);
                Attack.Source.Resistance.Path.PatchDefaultFile(reg);
                Push.Path.PatchDefaultFile(reg);
                Push.Source.Resistance.Path.PatchDefaultFile(reg);
                Status.Resistance.Path.PatchDefaultFile(reg);
            };
        }
    }
}