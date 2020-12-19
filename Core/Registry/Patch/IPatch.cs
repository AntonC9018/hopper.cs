namespace Hopper.Core.Registries
{
    // Initializes Patchable objects and/or Patch Subareas on the Patch Area
    public interface IPrePatch
    {
        void PrePatch(PatchArea patchArea);
    }

    // Patches the Patch Subareas and/or the Patchable objects
    public interface IPatch
    {
        void Patch(PatchArea patchArea);
    }

    // Uses Patch Subareas to set up Patchable objects correctly
    public interface IPostPatch
    {
        void PostPatch(PatchArea patchArea);
    }
}