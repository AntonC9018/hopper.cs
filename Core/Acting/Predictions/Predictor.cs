using System.Collections.Generic;
using Hopper.Core.ActingNS;
using Hopper.Core.WorldNS;
using Hopper.Utils;
using Hopper.Utils.Vector;

namespace Hopper.Core.ActingNS
{
    public class Predictor
    {
        public World world;
        public PredictionTargetInfo predictionInfo;

        public Predictor(World world, Layers targetedLayer, Faction targetedFaction)
        {
            this.world = world;
            this.predictionInfo = new PredictionTargetInfo(targetedLayer, targetedFaction);
        }

        public HashSet<IntVector2> GetBadPositions()
        {
            var set = new HashSet<IntVector2>();

            foreach (var actings in world.State._allActings)
            foreach (var acting in actings)
            {
                if (!acting._nextAction.HasAction())
                {
                    acting.CalculateAndSetAction();
                }

                // TODO: Add support for good/bad predicted positions (currenlty, all are processed as one thing)
                if (acting._nextAction._storedAction is IPredictable action)
                if (action is IUndirectedPredictable undirectedPredictable)
                {
                    set.AddRange(undirectedPredictable.Predict(acting.actor, predictionInfo));
                }
                else
                {
                    foreach (var direction in acting.GetPossibleDirections())
                        set.AddRange(action.Predict(acting.actor, direction, predictionInfo));
                }
            }
            return set;
        }
    }
}