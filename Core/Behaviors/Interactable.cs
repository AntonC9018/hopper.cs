using System.Runtime.Serialization;
using Chains;
using Core.Items;

namespace Core.Behaviors
{
    [DataContract]
    public partial class Interactable : Behavior
    {
        public class Config : BehaviorConfig
        {
            public ContentConfig contentConfig;
        }

        public class Event : ActorEvent
        {
            public IContent content;
        }

        // TODO: while generating the contents randomly, use throwaway pools while deserializing
        // since the content is going to overwritten afterwards while populating the object.
        public IContent m_content;

        public override void Init(Entity entity, BehaviorConfig config)
        {
            if (config != null)
            {
                m_content = ContentProvider.DefaultProvider
                    .CreateContent(((Config)config).contentConfig);
            }
            base.Init(entity, config);
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
        static void Die(Event ev) => ev.actor.Die();
        static void Release(Event ev) => ev.content?.Release(ev.actor);

        public static ChainPaths<Interactable, Event> Check;
        public static ChainPaths<Interactable, Event> Do;

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