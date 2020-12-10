namespace Hopper.Core.Items
{
    public static class Pool
    {
        public static SuperPool<NormalSubPool> CreateNormal()
        {
            return new SuperPool<NormalSubPool>();
        }
        public static SuperPool<EndlessSubPool> CreateEndless()
        {
            return new SuperPool<EndlessSubPool>();
        }
    }
}