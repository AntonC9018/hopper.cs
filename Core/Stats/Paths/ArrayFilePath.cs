namespace Hopper.Core.Stats.Basic
{
    public static class ArrayFilePath<T>
    {
        public static ArrayFile GetDefaultFile(Registry registry)
        {
            return registry
                .GetCustomPatchRegistry<ArrayPatch<T>, Resistance_Kind<T>>()
                .DefaultFile;
        }
    }
}