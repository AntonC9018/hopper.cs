using System.Collections.Generic;
using Hopper.Utils.Vector;
using Hopper.Utils;
using Hopper.Shared.Attributes;

namespace Hopper.Core.Components.Basic
{
    public partial class Sequential : IComponent
    {
        [Inject] public Sequence sequence;
        public ParticularAction CurrentAction => sequence.CurrentAction;

        // Tick.Chain.ChainPath(m_entity.Behaviors).AddHandler(
        //         e => sequence.TickAction(m_entity)
        //     );

        [Alias("GetMovs")] 
        public IEnumerable<IntVector2> GetMovs(Entity actor)
        {
            Assert.That(sequence.CurrentStep.movs != null);
            return sequence.CurrentStep.movs(actor.GetTransform());
        }

        public void ApplyCurrentAlgo(Acting.Context ctx)
        {
            sequence.CurrentStep.algo(ctx);
        }

        public static ParticularAction CalculateAction(Entity entity) 
            => entity.GetSequential().CurrentAction;
    }
}