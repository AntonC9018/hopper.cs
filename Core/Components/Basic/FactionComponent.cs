using Hopper.Core.Components;
using Hopper.Shared.Attributes;

namespace Hopper.Core
{
    public partial class Faction : IComponent
    {
        /// <summary>
        /// Entities of faction X can attack anything that does not intersect X.
        /// This, however, is not imposed and is used as a guide for the entities themselves.
        /// </summary>
        [Flags] public enum Flags
        {
            Player = 0b_0000_0001,
            Enemy = 0b_0000_0010,
            Environment = 0b_0000_0100
        }
        [Flags] [Inject] public Flags faction;


        [Alias("IsPlayer")]
        public bool IsPlayer() => faction.HasFlag(Flags.Player);
    }
}