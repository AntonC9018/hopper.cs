using System.Collections.Generic;
using Hopper.Utils.Chains;
using Hopper.Utils.Vector;

namespace Hopper.Core
{
    public class CellMovementTriggerGrid : Dictionary<IntVector2, LinearChain<CellMovementContext>>
    {
        public CellMovementTriggerGrid() : base()
        {
        }

        public void Subscribe(IntVector2 position, System.Action<CellMovementContext> handler)
        {
            LinearChain<CellMovementContext> chain;
            if (!TryGetValue(position, out chain))
            {
                chain = new LinearChain<CellMovementContext>();
            }
            chain.Add(handler);
        }

        public void Trigger(Transform transform)
        {
            if (TryGetValue(transform.position, out var chain))
            {
                var context = new CellMovementContext(transform); 
                chain.PassWithoutStop(context);
            }
        }

        public void Reset()
        {
            foreach (var chain in Values)
            {
                chain.Clear();
            }
        }
    }
}