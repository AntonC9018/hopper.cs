using System.Collections.Generic;
using System.Linq;
using Hopper.Core.Components.Basic;
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

        public ParticularUndirectedAction(UndirectedDo function, UndirectedPredict predict = null)
        {
            action.function = function;
            action.predict = predict;
        }

        public override bool Do(Acting acting)
        {
            return action.function(acting);
        }

        public IEnumerable<IntVector2> Predict(Acting acting)
        {
            if (action.predict != null)
            {
                return action.predict(acting);
            }
            return Enumerable.Empty<IntVector2>();
        }
    }
}