using System.Collections.Generic;
using System.Linq;
using Hopper.Core.Components.Basic;
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

        public override bool Do(Acting acting)
        {
            return action.function(acting, direction);
        }

        public IEnumerable<IntVector2> Predict(Acting acting, IntVector2 direction)
        {
            if (action.predict != null)
            {
                return action.predict(acting, direction);
            }
            return Enumerable.Empty<IntVector2>();
        }
    }
}