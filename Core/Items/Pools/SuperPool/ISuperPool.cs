namespace Hopper.Core.Items
{
    public interface ISuperPool
    {
        PoolItem GetNextItem(string path);
    }
}