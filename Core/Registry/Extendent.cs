namespace Hopper.Core.Registry
{
    public interface IExtendent : IKind, IPatch
    {
    }

    public class Extendent<T> : IExtendent where T : IExtendent
    {
        public int m_id;
        public int Id => m_id;

        public void RegisterSelf(ModSubRegistry registry)
        {
            System.Console.WriteLine(typeof(T));
            m_id = registry.Add<T>((T)(IExtendent)this);
        }

        public void Patch(Repository repository)
        {
            repository.GetPatchSubRegistry<T>().Add(m_id, (T)(IExtendent)this);
        }
    }
}