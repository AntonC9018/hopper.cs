using Newtonsoft.Json;

namespace Hopper.Core.Items
{
    public interface IResizable
    {
        [JsonIgnore] int Size { get; set; }
    }

    public interface IResizableContainer : IResizable, IItemContainer
    {
    }
}