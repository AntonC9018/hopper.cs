using System.Collections.Generic;
using Hopper.Core.Components.Basic;
using Hopper.Utils.Vector;

namespace Hopper.Core.Predictions
{
    public class Predictor
    {
        public World world;
        public Faction targetedFaction;

        public Predictor(World world, Faction targetedFaction)
        {
            this.world = world;
            this.targetedFaction = targetedFaction;
        }

        public IEnumerable<IntVector2> GetBadPositions()
        {
            var set = new HashSet<IntVector2>();
            foreach (var entities in world.State.Entities)
            {
                foreach (var entity in entities)
                {
                    if (!entity.Faction.IsOfFaction(targetedFaction) && entity.Behaviors.Has<Acting>())
                    {
                        var acting = entity.Behaviors.Get<Acting>();

                        if (acting.nextAction == null)
                        {
                            acting.CalculateNextAction();
                        }
                        if (acting.nextAction == null)
                        {
                            continue;
                        }

                        // TODO: Add support for good/bad predicted positions (currenlty, all are processed as one thing)
                        if (acting.nextAction is ParticularDirectedAction)
                        {
                            var action = (ParticularDirectedAction)acting.nextAction;
                            foreach (var direction in acting.GetPossibleDirections())
                            {
                                foreach (var pos in action.Predict(entity, direction))
                                {
                                    set.Add(pos);
                                }
                            }
                        }
                        else
                        {
                            var action = (ParticularUndirectedAction)acting.nextAction;
                            foreach (var pos in action.Predict(entity))
                            {
                                set.Add(pos);
                            }
                        }
                    }
                }
            }
            foreach (var vector in set)
            {
                yield return vector;
            }
        }
    }
}