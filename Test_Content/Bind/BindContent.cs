using Hopper.Core;

namespace Hopper.Test_Content.Bind
{
    public class BindContent
    {
        public BindStatus NoMove;
        public Retoucher NoMoveRetoucher;
        public EntityFactory<Spider> SpiderFactory;

        public BindContent()
        {
            NoMove = BindStatuses.CreateStopMoveBindStatus();
            NoMoveRetoucher = BindRetouchers.CreateBindRetoucher(NoMove);
            SpiderFactory = Spider.CreateFactory(this);
        }

        public void RegisterSelf(Registry registry)
        {
            NoMove.RegisterSelf(registry);
            NoMoveRetoucher.RegisterSelf(registry);
            SpiderFactory.RegisterSelf(registry);
        }
    }
}