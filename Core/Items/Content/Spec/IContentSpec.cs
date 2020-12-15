using Hopper.Core.Registry;

namespace Hopper.Core.Items
{
    public interface IContentSpec
    {
        IContent CreateContent(PoolContainer pools, Registry.Registry registry);
    }
}