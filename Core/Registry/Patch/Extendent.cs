namespace Hopper.Core.Registry
{
    /*
        Extendents are kinds that also work as pathches.
    */
    public interface IExtendent : IKind, IPatch
    {
    }

    public class Extendent<T> : IExtendent where T : IExtendent
    {
        public int m_id;
        public int Id => m_id;

        public void RegisterSelf(ModRegistry registry)
        {
            System.Console.WriteLine(typeof(T));
            m_id = registry.Add<T>((T)(IExtendent)this);
        }

        public void Patch(PatchArea patchArea)
        {
            patchArea.GetPatchSubRegistry<T>().Add(m_id, (T)(IExtendent)this);
        }
    }
}