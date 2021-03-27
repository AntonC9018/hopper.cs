using Hopper.Core.Components;

namespace Hopper.Core
{
    public partial class FactionComponent : IComponent
    {
        [Flags] public Faction faction;

        public bool IsPlayer => faction.IsOfFaction(Faction.Player);
    }
}