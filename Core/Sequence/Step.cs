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

        public System.Func<Entity, Result> successFunction = null;
        public int repeat = 1;

        public Action action = null;
        public System.Action<Entity> enterFunction = null;
        public System.Action<Entity> exitFunction = null;
        public MovsFunc movs = null;
        public System.Action<Acting.Context> algo = null;

        public int CheckSuccessAndGetRelativeIndex(Entity e)
        {
            if (successFunction != null)
            {
                var result = successFunction(e);
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

            var acting = e.GetComponent(Acting.Index);

            if (acting != null)
            {
                if (acting._flags.HasFlag(Acting.Flags.ActionSucceeded))
                {
                    return relativeStepIndexSuccess;
                }
                return relativeStepIndexFail;
            }

            throw new System.Exception("Sequence expects Acting decorator unless a custom check success function is specified for each of the steps");

        }

        public bool IsRepeatLimitMet(int currentRepeatCount)
        {
            return currentRepeatCount >= repeat;
        }

        public void Enter(Entity e)
        {
            if (enterFunction != null)
                enterFunction(e);
        }

        public void Exit(Entity e)
        {
            if (exitFunction != null)
                exitFunction(e);
        }
    }
}