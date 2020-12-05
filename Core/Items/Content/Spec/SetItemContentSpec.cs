namespace Hopper.Core.Items
{
    public class SetItemContentSpec : IContentSpec
    {
        private IItem m_item;

        public SetItemContentSpec(IItem item)
        {
            this.m_item = item;
        }

        public IContent CreateContent(PoolContainer pools)
        {
            return new ItemContent(m_item);
        }
    }
}