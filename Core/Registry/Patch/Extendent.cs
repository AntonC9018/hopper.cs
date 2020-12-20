namespace Hopper.Core.Registries
{
    /*
        Extendents are kinds that also work as pathches.
    */
    public interface IExtendent : IKind, IPatch
    {
    }

    public class Extendent<T> : Kind<IExtendent>, IExtendent where T : IExtendent
    {
        public void Patch(PatchArea patchArea)
        {
            patchArea.GetPatchSubRegistry<T>().Add(m_id, (T)(IExtendent)this);
        }
    }
}