using System.Collections.Generic;
using Hopper.Core.Components.Basic;
using Hopper.Utils.Vector;

namespace Hopper.Core.Predictions
{
    public class Predictor
    {
        public World world;
        public PredictionTargetInfo predictionInfo;

        public Predictor(World world, Layer targetedLayer, Faction targetedFaction)
        {
            this.world = world;
            this.predictionInfo = new PredictionTargetInfo(targetedLayer, targetedFaction);
        }

        public HashSet<IntVector2> GetBadPositions()
        {
            var set = new HashSet<IntVector2>();

            foreach (var actings in world.state.actings)
            foreach (var acting in actings)
            {
                if (acting.nextAction == null)
                {
                    acting.CalculateNextAction();
                }
                if (acting.nextAction == null)
                {
                    continue;
                }

                // TODO: Add support for good/bad predicted positions (currenlty, all are processed as one thing)
                // TODO: require a layer as well as a faction, like I'm doing in the target provider
                if (acting.nextAction._storedAction is IPredictable action)
                foreach (var direction in acting.GetPossibleDirections())
                foreach (var pos in action.Predict(acting.actor, direction, predictionInfo))
                {
                    set.Add(pos);
                }
            }
            return set;
        }
    }
}