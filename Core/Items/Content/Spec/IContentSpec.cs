namespace Hopper.Core.Items
{
    public interface IContentSpec
    {
        IContent CreateContent(PoolContainer pools, KindRegistry registry);
    }
}