using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Hopper.Utils.Vector;
using Hopper.Utils;

namespace Hopper.Core.Components.Basic
{
    public class Sequential : IComponent
    {
        [Inject] public Sequence sequence;
        public ParticularAction CurrentAction => sequence.CurrentAction;

        // Tick.Chain.ChainPath(m_entity.Behaviors).AddHandler(
        //         e => sequence.TickAction(m_entity)
        //     );

        [Alias("GetMovs")] 
        public List<IntVector2> GetMovs(Entity actor)
        {
            Assert.That(sequence.CurrentStep.movs != null);
            return sequence.CurrentStep.movs(actor);
        }

        public void ApplyCurrentAlgo(Acting.Context ctx)
        {
            sequence.CurrentStep.algo(ctx);
        }
    }
}