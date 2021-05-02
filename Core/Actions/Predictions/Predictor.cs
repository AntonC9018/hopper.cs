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

        public HashSet<IntVector2> GetBadPositions()
        {
            var set = new HashSet<IntVector2>();
            foreach (var actings in world.state.actings)
            foreach (var acting in actings)
            {
                if (!acting.actor.TryCheckFaction(~targetedFaction, out bool result) && result)
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
                    if (acting.nextAction is ParticularDirectedAction)
                    {
                        var action = (ParticularDirectedAction)acting.nextAction;
                        foreach (var direction in acting.GetPossibleDirections())
                        {
                            foreach (var pos in action.Predict(acting.actor, direction))
                            {
                                set.Add(pos);
                            }
                        }
                    }
                    else
                    {
                        var action = (ParticularUndirectedAction)acting.nextAction;
                        foreach (var pos in action.Predict(acting.actor))
                        {
                            set.Add(pos);
                        }
                    }
                }
            }
            return set;
        }
    }
}