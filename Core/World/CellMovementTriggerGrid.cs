using System.Collections.Generic;
using Hopper.Utils.Chains;
using Hopper.Utils.Vector;

namespace Hopper.Core
{
    public struct CellMovementTriggerGrid
    {
        private Dictionary<IntVector2, LinearChain<CellMovementContext>> _triggers;

        public void Init()
        {
            _triggers = new Dictionary<IntVector2, LinearChain<CellMovementContext>>();
        }

        public void Subscribe(IntVector2 position, System.Action<CellMovementContext> handler)
        {
            LinearChain<CellMovementContext> chain;
            if (!_triggers.TryGetValue(position, out chain))
            {
                chain = new LinearChain<CellMovementContext>();
            }
            chain.Add(handler);
        }

        public void Trigger(Transform transform)
        {
            if (_triggers.TryGetValue(transform.position, out var chain))
            {
                var context = new CellMovementContext(transform); 
                chain.PassWithoutStop(context);
            }
        }

        public void Reset()
        {
            foreach (var chain in _triggers.Values)
            {
                chain.Clear();
            }
        }
    }


    public struct PermanentCellMovementTriggerGrid
    {
        private Dictionary<IntVector2, PermanentChain<CellMovementContext>> _triggers;

        public void Init()
        {
            _triggers = new Dictionary<IntVector2, PermanentChain<CellMovementContext>>();
        }

        public void Subscribe(IntVector2 position, System.Func<CellMovementContext, bool> handler)
        {
            PermanentChain<CellMovementContext> chain;
            if (!_triggers.TryGetValue(position, out chain))
            {
                chain = new PermanentChain<CellMovementContext>();
            }
            chain.Add(handler);
        }

        public void Trigger(Transform transform)
        {
            if (_triggers.TryGetValue(transform.position, out var chain))
            {
                var context = new CellMovementContext(transform); 
                chain.PassAndFilter(context);
            }
        }

        public void ClearAll()
        {
            _triggers.Clear();
        }
    }
}