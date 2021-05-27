using Hopper.Core.ActingNS;
using Hopper.Core.WorldNS;

namespace Hopper.Core.ActingNS
{
    public readonly struct PredictionTargetInfo
    {
        public readonly Layer layer;
        public readonly Faction faction;

        public PredictionTargetInfo(Layer layer, Faction faction)
        {
            this.layer = layer;
            this.faction = faction;
        }
    }
}