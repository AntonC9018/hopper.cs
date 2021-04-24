namespace Hopper.Core
{

    public struct Pools
    {
        public PoolRegistry Entity;
        public PoolRegistry Item;

        public void Init()
        {
            Entity.Init();
            Item.Init();
        }

        public void StartRuntime()
        {
            Entity.StartRuntime();
            Item.StartRuntime();
        }

        public void StopRuntime()
        {
            Entity.StopRuntime();
            Item.StopRuntime();
        }
    }
}