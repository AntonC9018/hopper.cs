namespace Hopper.Core.Stats.Basic
{
    public static class BasicStats
    {
        public static void Init(Registry registry)
        {
            SourceBase<Attack.Source>.InitFor(registry);
            Attack.BasicSource.AddFor(registry);
            Attack.Path.CreateDefaultFile(registry);
            Attack.Source.Resistance.Path.CreateDefaultFile(registry);

            Dig.Source.AddFor(registry);

            SourceBase<Push.Source>.InitFor(registry);
            Push.BasicSource.AddFor(registry);
            Push.Path.CreateDefaultFile(registry);
            Push.Source.Resistance.Path.CreateDefaultFile(registry);
        }
    }
}