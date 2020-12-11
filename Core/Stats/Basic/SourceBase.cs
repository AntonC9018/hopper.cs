namespace Hopper.Core.Stats.Basic
{
    public class SourceBase<T>
    {
        public int resistance { get; set; }

        public void InitFor(Registry registry)
        {
            var r = registry.GetCustomPatchRegistry<ArrayPatch<T>, Resistance_Kind<T>>();
            registry.IdReferences[this] = r.patches.Count - 1;
            r.patches.Add(resistance);
        }
        public int GetId(Registry registry)
        {
            return registry.IdReferences[this];
        }
    }
}