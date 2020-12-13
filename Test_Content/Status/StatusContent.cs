using Hopper.Core;
using Hopper.Core.Retouchers;
using Hopper.Test_Content.Status.Freeze;

namespace Hopper.Test_Content.Status
{
    public class StatusContent
    {
        public Retoucher MoveCapturedRetoucher;
        public EntityFactory<IceCube> IceCubeFactory;
        public FreezeStatus FreezeStatus;


        public StatusContent(CoreRetouchers retouchers)
        {
            MoveCapturedRetoucher = IceCube.CreateMoveCapturedRetoucher();
            IceCubeFactory = IceCube.CreateFactory(MoveCapturedRetoucher);
            FreezeStatus = new FreezeStatus(1, IceCubeFactory);
        }

        public void RegisterSelf(Registry registry)
        {
            MoveCapturedRetoucher.RegisterSelf(registry);
            FreezeStatus.RegisterSelf(registry);
            IceCubeFactory.RegisterSelf(registry);
        }
    }
}