namespace Hopper.Core
{
    /// <summary>
    /// Entities of faction X can attack anything that does not intersect X.
    /// This, however, is not imposed and is used as a guide for the entities themselves.
    /// </summary>
    public enum Faction
    {
        Player = 0b_0000_0001,
        Enemy = 0b_0000_0010,
        Environment = 0b_0000_0100
    }

    public static class FactionExtensions
    {
        public static bool IsOfFaction(this Entity entity, Faction faction)
        {
            return (entity.Faction & faction) != 0;
        }

        public static bool IsOfFaction(this Faction faction1, Faction faction2)
        {
            return (faction1 & faction2) != 0;
        }
    }
}