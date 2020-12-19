using Hopper.Core.Registries;

namespace Hopper.Core.Items
{
    public interface IContentSpec
    {
        IContent CreateContent(Pools pools);
    }
}