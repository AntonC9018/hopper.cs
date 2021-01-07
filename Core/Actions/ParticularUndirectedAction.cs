using System.Collections.Generic;
using System.Linq;
using Hopper.Utils.Vector;

namespace Hopper.Core
{
    public class ParticularUndirectedAction : ParticularAction
    {
        private UndirectedAction action;

        public ParticularUndirectedAction(UndirectedAction action)
        {
            this.action = action;
        }

        public ParticularUndirectedAction(UndirectedDo function, PredictUndirected predict = null)
        {
            action.function = function;
            action.predict = predict;
        }

        public override bool Do(Entity entity)
        {
            return action.function(entity);
        }

        public IEnumerable<IntVector2> Predict(Entity entity)
        {
            if (action.predict != null)
            {
                return action.predict(entity);
            }
            return Enumerable.Empty<IntVector2>();
        }
    }
}