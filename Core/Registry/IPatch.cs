namespace Hopper.Core.Stats
{
    public interface IPatch
    {
        Registry Registry { get; }
        void PatchKindRegistry(int kindId);
    }
}