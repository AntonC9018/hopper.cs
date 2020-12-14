namespace Hopper.Core.Stats.Basic
{
    public class SourceBase<T> : IKind, IPatch where T : SourceBase<T>
    {
        public int resistance { get; set; }
        private int m_id;
        public int Id => m_id;

        public void RegisterSelf(ModSubRegistry registry)
        {
            m_id = registry.Add<T>((T)this);
        }

        public void Patch(Repository repository)
        {
            repository.GetPatchSubRegistry<T>().Add(m_id, (T)this);
        }
    }
}