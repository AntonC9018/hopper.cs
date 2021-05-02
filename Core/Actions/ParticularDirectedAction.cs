using System.Collections.Generic;
using System.Linq;
using Hopper.Core.Components.Basic;
using Hopper.Core.Predictions;
using Hopper.Utils.Vector;

namespace Hopper.Core
{
    public class ParticularDirectedAction : ParticularAction
    {
        public IntVector2 direction;
        private DirectedAction action;

        public ParticularDirectedAction(DirectedAction action)
        {
            this.action = action;
        }

        public override bool Do(Entity actor)
        {
            return action.function(actor, direction);
        }

        public IEnumerable<IntVector2> Predict(Entity actor, IntVector2 direction,  PredictionTargetInfo info)
        {
            if (action.predict != null)
            {
                return action.predict(actor, direction, info);
            }
            return Enumerable.Empty<IntVector2>();
        }
    }
}