namespace Hopper.Core.Registry
{
    public class Extendent<T> : IKind, IPatch where T : Extendent<T>
    {
        public int m_id;
        public int Id => m_id;

        public void RegisterSelf(ModSubRegistry registry)
        {
            m_id = registry.Add<Extendent<T>>(this);
        }

        public void Patch(Repository repository)
        {
            repository.GetPatchSubRegistry<Extendent<T>>().Add(m_id, this);
        }
    }
}