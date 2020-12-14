namespace Hopper.Core.Mods
{
    public interface IMod : ISubMod
    {
        int Offset { get; }
        string Name { get; }
    }
}