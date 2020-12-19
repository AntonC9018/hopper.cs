namespace Hopper.Core.Items
{
    public interface ISuperPool
    {
        ISuperPool Copy();
        PoolItem GetNextItem(string path);
    }
}