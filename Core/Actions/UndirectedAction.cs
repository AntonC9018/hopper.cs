namespace Hopper.Core
{
    public class UndirectedAction : Action
    {
        public UndirectedDo function;
        public PredictUndirected predict;

        public ParticularUndirectedAction ToUndirectedParticular()
        {
            return new ParticularUndirectedAction(this);
        }

        public override ParticularAction ToParticular()
        {
            return ToUndirectedParticular();
        }
    }
}