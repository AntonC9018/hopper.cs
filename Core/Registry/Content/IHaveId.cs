namespace Hopper.Core.Registries
{
    /// <summary>
    /// This is not necessarily content. This is anything that has id's.
    /// INSTANCES currently have this type (World and Entity) 
    /// </summary>
    public interface IHaveId
    {
        int Id { get; }
    }
}