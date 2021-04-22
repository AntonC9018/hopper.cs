using System.Collections.Generic;
using Hopper.Core.Components.Basic;
using Hopper.Utils.Vector;

namespace Hopper.Core
{
    public delegate void InsideChangeFunction(Entity entity);
    public delegate Result SuccessCheckFunction(Entity entity);

    public class Result
    {
        public int? index = null;
        public bool success = false;
    }

    public class Step
    {
        public int relativeStepIndexSuccess = 1;
        public int relativeStepIndexFail = 0;

        public System.Func<Acting, Result> successFunction = null;
        public int repeat = 1;

        public Action action = null;
        public System.Action<Acting> enterFunction = null;
        public System.Action<Acting> exitFunction = null;
        public MovsFunc movs = null;
        public System.Action<Acting.Context> algo = null;

        public int CheckSuccessAndGetRelativeIndex(Acting acting)
        {
            if (successFunction != null)
            {
                var result = successFunction(acting);
                if (result.index != null)
                {
                    return (int)result.index;
                }
                if (result.success)
                {
                    return relativeStepIndexSuccess;
                }
                return relativeStepIndexFail;
            }

            if (acting._flags.HasFlag(Acting.Flags.ActionSucceeded))
            {
                return relativeStepIndexSuccess;
            }
            return relativeStepIndexFail;

        }

        public bool IsRepeatLimitMet(int currentRepeatCount)
        {
            return currentRepeatCount >= repeat;
        }

        public void Enter(Acting acting)
        {
            if (enterFunction != null)
                enterFunction(acting);
        }

        public void Exit(Acting e)
        {
            if (exitFunction != null)
                exitFunction(e);
        }
    }
}