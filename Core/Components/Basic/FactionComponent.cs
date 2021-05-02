using Hopper.Core.Components;
using Hopper.Shared.Attributes;
using static Hopper.Core.Faction;

namespace Hopper.Core
{
    /// <summary>
    /// Entities of faction X can attack anything that does not intersect X.
    /// This, however, is not imposed and is used as a guide for the entities themselves.
    /// </summary>
    [Flags] public enum Faction
    {
        Player = 0b_0000_0001,
        Enemy = 0b_0000_0010,
        Environment = 0b_0000_0100,
        Any = ~0 // 0xFFFFFFFF or -1
    }

    // TODO: Generate automatically for flags! 
    public static class FactionExtensions
    {
        public static bool AreEitherSet(this Faction attackness, Faction flags)
        {
            return (attackness & flags) != 0;
        }
    }

    public partial class FactionComponent : IComponent
    {
        
        [Inject] public Faction faction;


        [Alias("IsPlayer")]
        public bool IsPlayer() => faction.HasFlag(Faction.Player);

        [Alias("CheckFaction")]
        public bool CheckFaction(Faction flags) => faction.AreEitherSet(flags); 
    }
}