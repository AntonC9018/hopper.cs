using Hopper.Core.Registries;

namespace Hopper.Core
{
    public interface ISubMod : IPrePatch, IPatch, IPostPatch, IHasContent
    {
    }
}