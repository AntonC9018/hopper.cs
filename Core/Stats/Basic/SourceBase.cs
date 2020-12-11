namespace Hopper.Core.Stats.Basic
{
    public class SourceBase<T> where T : class
    {
        public int resistance { get; set; }

        public void InitFor(Registry registry)
        {
            var r = new ArrayPatch<T>();
            registry.AddCustomPatchRegistry<ArrayPatch<T>, Resistance_Kind<T>>(r);
            registry.IdReferences[this] = r.patches.Count - 1;
            r.patches.Add(resistance);
        }
        public int GetId(Registry registry)
        {
            return registry.IdReferences[this];
        }
    }
}