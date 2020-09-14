using Chains;
using System.Collections.Generic;
using Core.FS;
using Utils.Vector;

namespace Core.Behaviors
{
    public class Template : Behavior
    {

        public class Event : CommonEvent
        {
        }

        public class Params : ActivationParams
        {
        }


        public bool Activate(Action action, Params pars)
        {
            var ev = new Event
            {
                actor = m_entity,
                action = action,
                // push = pars.push
            };
            return CheckDoCycle<Event>(ev);
        }

        static void HandlerFunction1(Event ev)
        {
        }

        static void HandlerFunction2(Event ev)
        {
        }

        public static ChainPaths<Template, Event> Check;
        public static ChainPaths<Template, Event> Do;
        static Template()
        {
            Check = new ChainPaths<Template, Event>(ChainName.Check);
            Do = new ChainPaths<Template, Event>(ChainName.Do);

            var builder = new ChainTemplateBuilder()

                .AddTemplate<Event>(ChainName.Check)
                .AddHandler(HandlerFunction1, PriorityRanks.High)

                .AddTemplate<Event>(ChainName.Do)
                .AddHandler(HandlerFunction2)
                //.AddHandler(Utils.AddHistoryEvent(History.EventCode.pushed_do));

                .End();

            BehaviorFactory<Template>.s_builder = builder;

            // SetupStats();
        }
    }
}