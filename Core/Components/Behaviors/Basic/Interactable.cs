using System.Runtime.Serialization;
using Hopper.Utils.Chains;
using Hopper.Core.Items;
using Hopper.Core.Chains;

namespace Hopper.Core.Components.Basic
{
    // @Rethink This behavior should probably have no chains.
    [DataContract]
    public partial class Interactable : Behavior, IInitable<IContentSpec>
    {
        public class Event : ActorEvent
        {
            public IContent content;
        }

        // @Incomplete while generating the contents randomly, use throwaway pools while deserializing
        // since the content is going to overwritten afterwards while populating the object.
        // maybe include a flag to prevent that. OR use a more simplistic strategy for serialization.
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
        public static Handler<Event> DieHandler = new Handler<Event>
        {
            handler = (Event ev) => ev.actor.Die(),
            // @Incomplete: hardcode a reasonable priority value 
            priority = (int)PriorityRank.Medium
        };

        public static Handler<Event> ReleaseHandler = new Handler<Event>
        {
            handler = (Event ev) => ev.content?.Release(ev.actor),
            // @Incomplete: hardcode a reasonable priority value 
            priority = (int)PriorityRank.Medium
        };

        public static readonly ChainPaths<Interactable, Event> Check = new ChainPaths<Interactable, Event>(ChainName.Check);
        public static readonly ChainPaths<Interactable, Event> Do = new ChainPaths<Interactable, Event>(ChainName.Do);

        public static readonly ChainTemplateBuilder DefaultBuilder = new ChainTemplateBuilder()
                .AddTemplate<Event>(ChainName.Check)
                .AddTemplate<Event>(ChainName.Do)
                    .AddHandler(DieHandler)
                    .AddHandler(ReleaseHandler)
                .End();
        public static ConfigurableBehaviorFactory<Interactable, IContentSpec> Preset(IContentSpec spec) =>
            new ConfigurableBehaviorFactory<Interactable, IContentSpec>(DefaultBuilder, spec);

        static Interactable()
        {
            
        }
    }
}