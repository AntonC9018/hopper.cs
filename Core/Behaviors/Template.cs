using Chains;
using System.Collections.Generic;
using Core.FS;
using Vector;

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

        public static string s_checkChainName = "template:check";
        public static string s_doChainName = "template:do";

        public bool Activate(Action action, Params pars)
        {
            var ev = new Event
            {
                actor = m_entity,
                action = action,
                // push = pars.push
            };
            return CheckDoCycle<Event>(ev, s_checkChainName, s_doChainName);
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
            var builder = new ChainTemplateBuilder();

            var check = builder.AddTemplate<Event>(s_checkChainName);
            Check = new ChainPaths<Template, Event>(s_checkChainName);
            check.AddHandler(HandlerFunction1, PRIORITY_RANKS.HIGH);

            var _do = builder.AddTemplate<Event>(s_doChainName);
            Do = new ChainPaths<Template, Event>(s_checkChainName);
            _do.AddHandler(HandlerFunction2);
            // _do.AddHandler(Utils.AddHistoryEvent(History.EventCode.pushed_do));

            BehaviorFactory<Template>.s_builder = builder;

            // SetupStats();
        }
    }
}