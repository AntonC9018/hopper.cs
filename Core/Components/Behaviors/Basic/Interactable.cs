using System.Runtime.Serialization;
using Hopper.Utils.Chains;
using Hopper.Core.Items;
using Hopper.Core.Chains;

namespace Hopper.Core.Components.Basic
{
    // @Rethink This behavior should probably have no chains.
    [DataContract]
    [AutoActivation("BeInteractedWith")]
    public partial class Interactable : IBehavior
    {
        public class Context : ActorEvent
        {
            public IContent content;
        }

        // @Incomplete while generating the contents randomly, use throwaway pools while deserializing
        // since the content is going to overwritten afterwards while populating the object.
        // maybe include a flag to prevent that. OR use a more simplistic strategy for serialization.
        [Inject] public IContent m_content;

        // public void Init(IContentSpec spec)
        // {
        //     m_entity.InitEvent += () => m_content = spec.CreateContent(m_entity.World.m_pools);
        // }

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

        public static readonly ChainTemplateBuilder DefaultBuilder = new ChainTemplateBuilder()
                .AddTemplate<Event>(ChainName.Check)
                .AddTemplate<Event>(ChainName.Do)
                    .AddHandler(DieHandler)
                    .AddHandler(ReleaseHandler)
                .End();
    }
}