using Chains;
using System.Collections.Generic;
using Core.FS;
using Vector;

namespace Core.Behaviors
{
    public class Pushable : IBehavior
    {
        public static List<string> s_indexSourceNameMap = new List<string>();

        public static int RegisterPushSource(string name, int defaultResValue = 1)
        {
            var sourceResFile = (ArrayFile)StatManager.s_defaultFS.GetFile("pushed/source_res");
            sourceResFile.content.Add(defaultResValue);

            s_indexSourceNameMap.Add(name);
            return s_indexSourceNameMap.Count - 1;
        }

        static Pushable()
        {
            Directory baseDir = StatManager.s_defaultFS.BaseDir;

            Directory pushDir = new Directory();
            StatFile sourceResFile = new ArrayFile();
            StatFile resFile = new Resistance
            {
                pierce = 1
            };

            baseDir.AddDirectory("pushed", pushDir);
            pushDir.AddFile("source_res", sourceResFile);
            pushDir.AddFile("res", resFile);

            RegisterPushSource("default", 1);
        }

        public class Resistance : StatFile
        {
            public int pierce = 0;
        }

        public class Event : CommonEvent
        {
            public Entity entity;
            public Attacking.Push push;
            public Resistance resistance;
        }

        public class Params : ActivationParams
        {
            public Attacking.Push push;
        }

        Chain<Event> chain_checkPushed;
        Chain<Event> chain_bePushed;

        public Pushable(Entity entity)
        {
            chain_checkPushed = (Chain<Event>)entity.m_chains["pushed:check"];
            chain_bePushed = (Chain<Event>)entity.m_chains["pushed:do"];
        }

        public bool Activate(Entity actor, Action action, ActivationParams pars = null)
        {
            var ev = new Event
            {
                actor = actor,
                action = action,
                push = ((Params)pars).push
            };

            chain_checkPushed.Pass(ev);

            if (!ev.propagate)
                return false;

            chain_bePushed.Pass(ev);
            return true;
        }

        static void SetResistance(Event ev)
        {
            ev.resistance = (Resistance)ev.actor.m_statManager.GetFile("pushed/res");
        }

        static void ResistSource(Event ev)
        {
            var sourceRes = (ArrayFile)ev.actor.m_statManager.GetFile("pushed/source_res");
            if (sourceRes[ev.push.source] > ev.push.power)
            {
                ev.push.distance = 0;
            }
        }

        static void Armor(Event ev)
        {

            if (ev.push.pierce <= ev.resistance.pierce)
            {
                ev.propagate = false;
            }
        }

        static void BePushed(Event ev)
        {
            // TODO: set up properly
            var move = new Displaceable.Move();
            var pars = new Displaceable.Params { move = move };
            ev.entity.beh_Displaceable.Activate(ev.actor, ev.action, pars);
        }

        public static BehaviorFactory<Pushable> s_factory = new BehaviorFactory<Pushable>(
            new IChainDef[]
            {
                new ChainDef<Event>
                {
                    name = "pushed:check",
                    handlers = new EvHandler<Event>[]
                    {
                        new EvHandler<Event> (
                            SetResistance,
                            PRIORITY_RANKS.HIGH
                        ),
                        new EvHandler<Event> (
                            ResistSource,
                            PRIORITY_RANKS.LOW
                        ),
                        new EvHandler<Event> (
                            Armor,
                            PRIORITY_RANKS.LOW
                        )
                    }
                },
                new ChainDef<Event>
                {
                    name = "pushed:do",
                    handlers = new EvHandler<Event>[]
                    {
                        new EvHandler<Event>(
                            BePushed
                        ),
                        new EvHandler<Event>(
                            Utils.AddHistoryEvent(History.EventCode.pushed_do)
                        )
                    }
                }
            }
        );
    }
}