using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Hopper.Core;
using Hopper.Core.Components;
using Hopper.Core.Components.Basic;
using Hopper.Shared.Attributes;
using Hopper.Utils.Vector;

namespace Hopper.TestContent.Floor
{
    // Some ideas:
    // 1. using ticking is kind of dumb for this: checking if the thing died or left each turn, meh
    // 2. subscribing to death of barriers idk
    // 3. destroy barriers on leave? but the leave is cleared every iteration. idk
    // TODO: this needs some thought
    public partial class BlockingTrapComponent : IComponent
    {
        [Inject] public Layer targetedLayer;
        public List<Entity> barriers = new List<Entity>();

        public bool Activate(Entity actor)
        {
            if (barriers.Count > 0)
            {
                return false;
            }

            var transform = actor.GetTransform();

            if (transform.GetAllUndirectedButSelfFromLayer(targetedLayer).Any())
            {
                foreach (var direction in IntVector2.OrthogonallyAdjacentToOrigin)
                {
                    var barrier = World.Global.SpawnEntity(RealBarrier.Factory, transform.position, direction);
                    barriers.Add(barrier);
                }
                return true;
            }

            return false;
        }

        public void RemoveBarriers()
        {
            foreach (var barrier in barriers)
            {
                if (!barrier.IsDead()) barrier.Die();
            }
            barriers.Clear();
        }
    }
}