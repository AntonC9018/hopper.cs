using Hopper.Core;
using Hopper.Core.Retouchers;

namespace Hopper.Test_Content.Floor
{
    public class WallsContent
    {
        public EntityFactory<Barrier> BarrierFactory;

        public WallsContent()
        {
            BarrierFactory = Barrier.CreateFactory();
        }

        public void RegisterSelf(Registry registry)
        {
            BarrierFactory.RegisterSelf(registry);
        }
    }
}