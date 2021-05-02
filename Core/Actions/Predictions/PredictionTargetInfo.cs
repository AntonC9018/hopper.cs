namespace Hopper.Core.Predictions
{
    public struct PredictionTargetInfo
    {
        public Layer layer;
        public Faction faction;

        public PredictionTargetInfo(Layer layer, Faction faction)
        {
            this.layer = layer;
            this.faction = faction;
        }
    }
}