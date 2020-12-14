using System.Runtime.Serialization;
using Hopper.Utils.Chains;
using Hopper.Core.Items;
using Hopper.Core.Chains;

namespace Hopper.Core.Behaviors.Basic
{
    [DataContract]
    public partial class Interactable : Behavior
    {
        public class Config
        {
            public IContentSpec contentSpec;

            public Config(IContentSpec contentSpec)
            {
                this.contentSpec = contentSpec;
            }
        }

        public class Event : ActorEvent
        {
            public IContent content;
        }

        // TODO: while generating the contents randomly, use throwaway pools while deserializing
        // since the content is going to overwritten afterwards while populating the object.
        // maybe include a flag to prevent that.
        public IContent m_content;

        private void Init(Config config)
        {
            if (config != null)
            {
                // TODO:
                // m_entity.InitEvent +=
                //     () => m_content = config.contentSpec.CreateContent(
                //         m_entity.World.m_pools, m_entity.World.m_currentRepository);
            }
        }

        public bool Activate()
        {
            var ev = new Event
            {
                actor = m_entity,
                content = m_content
            };
            return CheckDoCycle<Event>(ev);
        }

        // for now, let the default response be `die` 
        private static void Die(Event ev) => ev.actor.Die();
        private static void Release(Event ev) => ev.content?.Release(ev.actor);

        public static readonly ChainPaths<Interactable, Event> Check;
        public static readonly ChainPaths<Interactable, Event> Do;

        static Interactable()
        {
            Check = new ChainPaths<Interactable, Event>(ChainName.Check);
            Do = new ChainPaths<Interactable, Event>(ChainName.Do);

            var builder = new ChainTemplateBuilder()

                .AddTemplate<Event>(ChainName.Check)

                .AddTemplate<Event>(ChainName.Do)
                .AddHandler(Die, PriorityRanks.Medium)
                .AddHandler(Release, PriorityRanks.Medium)
                .End();

            BehaviorFactory<Interactable>.s_builder = builder;
        }
    }
}