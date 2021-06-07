using Hopper.Core;
using Hopper.Core.Components;
using Hopper.Core.Components.Basic;
using Hopper.Shared.Attributes;

namespace Hopper.TestContent
{
    public partial class WillExplodeEntityModifier : IComponent
    {
        [Inject] public int countDown;

        [Export(Chain = "Ticking.Do", Dynamic = true)]
        public void CountDown(Entity actor)
        {
            if (--countDown <= 0)
            {
                actor.TryDie();
            }
        }

        [Export(Chain = "+Entity.Death", Dynamic = true)]
        public void Explode(Entity actor)
        {
            Explosion.ExplodeBy(actor);
            Remove(actor);
        }

        public void Preset(Entity entity)
        {
            CountDownHandlerWrapper.HookTo(entity);
            ExplodeHandlerWrapper.HookTo(entity);
        }

        public void Remove(Entity actor)
        {
            actor.RemoveComponent(Index);
            CountDownHandlerWrapper.UnhookFrom(actor);
            ExplodeHandlerWrapper.UnhookFrom(actor);
        }
    }
}