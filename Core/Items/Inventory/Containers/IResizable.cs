using Newtonsoft.Json;

namespace Hopper.Core.Items
{
    public interface IResizable<out T> where T : IItem
    {
        [JsonIgnore] int Size { get; set; }
        T this[int index] { get; }
    }

    public interface IResizableContainer<out T> : IResizable<T>, IItemContainer<T>
        where T : IItem
    {
    }
}