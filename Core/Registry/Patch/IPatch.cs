namespace Hopper.Core.Registries
{
    ///<summary>Initializes Patchable objects and/or Patch Subareas on the Patch Area.</summary>
    public interface IPrePatch
    {
        void PrePatch(PatchArea patchArea);
    }

    ///<summary>Patches the Patch Subareas and/or the Patchable objects.<summary>
    public interface IPatch
    {
        void Patch(PatchArea patchArea);
    }

    /// <summary>Uses Patch Subareas to set up Patchable objects correctly</summary>
    public interface IPostPatch
    {
        void PostPatch(PatchArea patchArea);
    }
}