using Hopper.Core;
using Hopper.Core.Retouchers;

namespace Hopper.Test_Content.Trap
{
    public class TrapContent
    {
        public EntityFactory<BounceTrap> BounceTrapFactory;

        public TrapContent()
        {
            BounceTrapFactory = BounceTrap.CreateFactory();
        }

        public void RegisterSelf(Registry registry)
        {
            Bounce.Source.RegisterOn(registry);
            BounceTrapFactory.RegisterSelf(registry);
        }
    }
}