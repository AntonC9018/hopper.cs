using System.Runtime.Serialization;
using Hopper.Utils.Chains;
using Hopper.Core.Items;

namespace Hopper.Core.Components.Basic
{
    // @Rethink This behavior should probably have no chains.
    [DataContract]
    [AutoActivation("BeInteractedWith")]
    public partial class Interactable : IBehavior
    {
        public class Context : ActorContext
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
        [Export] public static void Die(Damageable damageable)
        {
            damageable.Die();
        }

        [Export] public static void ReleaseContent(Context ctx)
        {
            ctx.content?.Release(ctx.actor);
        }

        // Check {} Do { these 2 }
    }
}