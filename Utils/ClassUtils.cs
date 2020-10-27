namespace Core.Utils
{
    public static class ClassUtils
    {
        public static void AssureStaticallyConstructed(System.Type type)
        {
            System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(type.TypeHandle);
        }
    }
}