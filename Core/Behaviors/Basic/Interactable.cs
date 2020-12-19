using System.Runtime.Serialization;
using Hopper.Utils.Chains;
using Hopper.Core.Items;
using Hopper.Core.Chains;

namespace Hopper.Core.Behaviors.Basic
{
    [DataContract]
    public partial class Interactable : Behavior, IInitable<IContentSpec>
    {
        public class Event : ActorEvent
        {
            public IContent content;
        }

        // TODO: while generating the contents randomly, use throwaway pools while deserializing
        // since the content is going to overwritten afterwards while populating the object.
        // maybe include a flag to prevent that.
        public IContent m_content;

        public void Init(IContentSpec spec)
        {
            m_entity.InitEvent += () => m_content = spec.CreateContent(m_entity.World.m_pools);
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

        public static readonly ChainTemplateBuilder DefaultBuilder;
        public static ConfigurableBehaviorFactory<Interactable, IContentSpec> Preset(IContentSpec spec) =>
            new ConfigurableBehaviorFactory<Interactable, IContentSpec>(DefaultBuilder, spec);

        static Interactable()
        {
            Check = new ChainPaths<Interactable, Event>(ChainName.Check);
            Do = new ChainPaths<Interactable, Event>(ChainName.Do);

            DefaultBuilder = new ChainTemplateBuilder()

                .AddTemplate<Event>(ChainName.Check)

                .AddTemplate<Event>(ChainName.Do)
                .AddHandler(Die, PriorityRank.Medium)
                .AddHandler(Release, PriorityRank.Medium)
                .End();
        }
    }
}