using System.Collections.Generic;
using System.Runtime.Serialization;
using Core.Utils.Vector;

namespace Core
{
    [DataContract]
    public class Sequence
    {
        public Step[] stepData;
        public Entity actor;

        [DataMember] int currentStepIndex = 0;
        [DataMember] int currentRepeatCount = 0;

        public Action CurrentAction
        {
            get
            {
                return stepData[currentStepIndex].action?.Copy();
            }
        }

        public void TickAction()
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

        public List<IntVector2> GetMovs()
        {
            return stepData[currentStepIndex].GetMovs(actor);
        }
    }
}