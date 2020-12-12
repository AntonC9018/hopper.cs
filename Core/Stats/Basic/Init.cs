namespace Hopper.Core.Stats.Basic
{
    public static class BasicStats
    {
        public static void Init(Registry registry)
        {
            SourceBase<Attack.Source>.InitOn(registry);
            Attack.BasicSource.RegisterOn(registry);
            Attack.Path.SetDefaultFile(registry);
            Attack.Source.Resistance.Path.SetDefaultFile(registry);

            Dig.Source.RegisterOn(registry);

            SourceBase<Push.Source>.InitOn(registry);
            Push.BasicSource.RegisterOn(registry);
            Push.Path.SetDefaultFile(registry);
            Push.Source.Resistance.Path.SetDefaultFile(registry);

            SourceBase<IStatus>.InitOn(registry);
        }
    }
}