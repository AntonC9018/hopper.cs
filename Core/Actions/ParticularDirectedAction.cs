using System.Collections.Generic;
using System.Linq;
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

        public override bool Do(Entity entity)
        {
            return action.function(entity, direction);
        }

        public IEnumerable<IntVector2> Predict(Entity entity, IntVector2 direction)
        {
            if (action.predict != null)
            {
                return action.predict(entity, direction);
            }
            return Enumerable.Empty<IntVector2>();
        }
    }
}