namespace Hopper.Core.Predictions
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