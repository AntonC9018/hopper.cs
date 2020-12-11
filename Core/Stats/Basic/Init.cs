namespace Hopper.Core.Stats.Basic
{
    public static class BasicStats
    {
        public static void Init(Registry registry)
        {
            Attack.BasicSource.InitFor(registry);
            Attack.Path.CreateDefaultFile(registry);
            Attack.Source.Resistance.Path.CreateDefaultFile(registry);

            Dig.Source.InitFor(registry);

            Push.BasicSource.InitFor(registry);
            Push.Path.CreateDefaultFile(registry);
            Push.Source.Resistance.Path.CreateDefaultFile(registry);
        }
    }
}