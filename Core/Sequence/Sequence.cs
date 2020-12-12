using System.Collections.Generic;
using System.Runtime.Serialization;
using Hopper.Core.Behaviors.Basic;
using Hopper.Utils;
using Hopper.Utils.Vector;

namespace Hopper.Core
{
    [DataContract]
    public class Sequence : ISequence
    {
        private Step[] stepData;

        public Sequence(params Step[] stepData)
        {
            Assert.AreNotEqual(0, stepData.Length, "The step data must include at least one step");
            this.stepData = stepData;
        }

        [DataMember] private int currentStepIndex = 0;
        [DataMember] private int currentRepeatCount = 0;

        public Action CurrentAction
        {
            get
            {
                return stepData[currentStepIndex].action?.Copy();
            }
        }

        public void TickAction(Entity actor)
        {
            currentRepeatCount++;
            Step currentStep = stepData[currentStepIndex];

            if (!currentStep.IsRepeatLimitMet(currentRepeatCount))
                return;

            int relativeIndex = currentStep.CheckSuccessAndGetRelativeIndex(actor);

            if (relativeIndex != 0)
            {
                currentStepIndex += relativeIndex + stepData.Length;
                currentStepIndex %= stepData.Length;
                currentRepeatCount = 0;
                currentStep.Exit(actor);
                stepData[currentStepIndex].Enter(actor);
            }
        }

        public List<IntVector2> GetMovs(Entity actor)
        {
            return stepData[currentStepIndex].GetMovs(actor);
        }

        public void ApplyCurrentAlgo(Acting.Event ev)
        {
            stepData[currentStepIndex].algo(ev);
        }
    }
}