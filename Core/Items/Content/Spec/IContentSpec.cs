namespace Core.Items
{
    public interface IContentSpec
    {
        IContent CreateContent(PoolContainer pools);
    }
}