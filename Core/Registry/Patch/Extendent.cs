namespace Hopper.Core.Registries
{
    /** <summary>
        Extendents are kinds that also work as pathches.
        </summary>
    */
    public interface IExtendent : IKind, IPatch
    {
    }

    /// <inheritdoc cref="IExtendent"/>
    public class Extendent<T> : Kind<IExtendent>, IExtendent where T : IExtendent
    {
        public void Patch(PatchArea patchArea)
        {
            patchArea.GetPatchSubRegistry<T>().Add(m_id, (T)(IExtendent)this);
        }
    }
}