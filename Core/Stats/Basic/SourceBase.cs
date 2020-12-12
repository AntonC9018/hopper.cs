namespace Hopper.Core.Stats.Basic
{
    public class SourceBase<T> where T : class
    {
        public int resistance { get; set; }

        public static void InitOn(Registry registry)
        {
            var r = new ArrayPatch<T>();
            registry.AddCustomPatchRegistry<ArrayPatch<T>, Resistance_Kind<T>>(r);
        }

        public void RegisterOn(Registry registry)
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