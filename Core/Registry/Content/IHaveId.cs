namespace Hopper.Core.Registries
{
    // This is not necessarily content. This is anything that has id's.
    // INSTANCES currently have this type (World and Entity) 
    public interface IHaveId
    {
        int Id { get; }
    }
}