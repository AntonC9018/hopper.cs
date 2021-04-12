using Hopper.Core.Components;
using Hopper.Shared.Attributes;

namespace Hopper.Core
{
    public partial class FactionComponent : IComponent
    {
        [Flags] [Inject] public Faction faction;

        public bool IsPlayer => faction.IsOfFaction(Faction.Player);
    }
}