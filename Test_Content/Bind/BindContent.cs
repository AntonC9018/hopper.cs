using Hopper.Core;

namespace Hopper.Test_Content.Bind
{
    public class BindContent : ISubMod
    {
        public void RegisterSelf(ModSubRegistry registry)
        {
            BindStatuses.StopMove.RegisterSelf(registry);
            BindRetouchers.StopMoveRetoucher.RegisterSelf(registry);
            Spider.Factory.RegisterSelf(registry);
        }

        public void Patch(Repository repository)
        {
            BindStatuses.StopMove.Patch(repository);
        }

        public void AfterPatch(Repository repository)
        {
            Spider.Factory.AfterPatch(repository);
        }
    }
}