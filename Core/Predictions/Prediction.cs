using System.Collections.Generic;
using Hopper.Core.Behaviors.Basic;
using Hopper.Utils.Vector;

namespace Hopper.Core.Predictions
{
    public class Prediction
    {
        public World world;
        public Faction targetedFaction;

        public Prediction(World world, Faction targetedFaction)
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

                        if (acting.NextAction == null)
                        {
                            acting.CalculateNextAction();
                        }
                        if (acting.NextAction == null)
                        {
                            continue;
                        }

                        // For now, just highlight the attacking. Ignore e.g. lasers.
                        // It should in principle be more scalable. 
                        // For this, the action should store an object that implements the IBadPrediction interface
                        // and we have to somehow know to retrieve this object out of there.
                        if (acting.NextAction.ContainsAction(typeof(BehaviorAction<Attacking>)))
                        {
                            var attacking = entity.Behaviors.Get<Attacking>();
                            foreach (var direction in acting.GetPossibleDirections())
                            {
                                foreach (var pos in attacking.GetBadPositions(direction))
                                {
                                    set.Add(pos);
                                }
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