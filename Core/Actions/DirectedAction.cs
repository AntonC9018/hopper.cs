using Hopper.Utils.Vector;

namespace Hopper.Core
{
    public class DirectedAction : Action
    {
        public DirectedDo function;
        public PredictDirected predict;

        public ParticularDirectedAction ToAction()
        {
            return new ParticularDirectedAction(this);
        }

        public ParticularDirectedAction ToDirectedParticular(IntVector2 direction)
        {
            var result = new ParticularDirectedAction(this);
            result.direction = direction;
            return result;
        }

        public override ParticularAction ToParticular()
        {
            return ToDirectedParticular(IntVector2.Zero);
        }
    }
}