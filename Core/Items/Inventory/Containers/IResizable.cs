using Newtonsoft.Json;

namespace Core.Items
{
    public interface IResizable
    {
        [JsonIgnore] int Size { get; set; }
    }

    public interface IResizableContainer : IResizable, IItemContainer
    {
    }
}