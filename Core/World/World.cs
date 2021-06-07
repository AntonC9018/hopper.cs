using System.Collections.Generic;
using Hopper.Core.ActingNS;
using Hopper.Core.Components;
using Hopper.Core.Components.Basic;
using Hopper.Core.Items;
using Hopper.Shared.Attributes;
using Hopper.Utils.Chains;
using Hopper.Utils.Vector;

namespace Hopper.Core.WorldNS
{
    [InstanceExport]
    public partial class World
    {
        public static World Global;
        [Chain("gSpawnEntity")] 
        public static readonly Index<Chain<Entity>> SpawnEntityIndex = new Index<Chain<Entity>>();
        [Chain("gStartLoop")]
        public static readonly Index<Chain<int>> StartLoopIndex = new Index<Chain<int>>();
        [Chain("gEndLoop")]
        public static readonly Index<Chain<int>> EndLoopIndex = new Index<Chain<int>>();

        public GridManager Grid { get; private set; }
        public WorldStateManager State { get; private set; }
        public MoreChains Chains { get; private set; }
        
        public static readonly int NumOrders = System.Enum.GetValues(typeof(Order)).Length;

        // For now, do this for the sake of tests and debugging
        // The world currently does not really need an id
        public World(int width, int height)
        {
            Grid   = new GridManager(width, height);
            State  = new WorldStateManager();
            Chains = new MoreChains(Registry.Global.GlobalChains._map);

            // Preload the chains
            Chains.GetLazy(SpawnEntityIndex);
            Chains.GetLazy(StartLoopIndex);
            Chains.GetLazy(EndLoopIndex);
        }

        public int _loopCount = 0;

        public void Loop()
        {
            Chains.Get(StartLoopIndex).Pass(_loopCount);
            
            State.Loop();
            Grid.ResetCellTriggers();

            Chains.Get(EndLoopIndex).Pass(_loopCount);
            _loopCount++;
        }

        public Entity SpawnEntity(EntityFactory factory, IntVector2 position, IntVector2 orientation)
        {
            System.Console.WriteLine($"Creating entity of factory id : {factory.id}");

            var entity = factory.Instantiate();
            entity.id = Registry.Global.RuntimeEntities.Add(entity);

            if (entity.TryInitTransform(position, orientation, out var transform))
            {
                Grid.AddTransformNoEvent(transform);
                factory.InitInWorld(transform);
            }

            if (entity.TryInitActing(out var acting))
            {
                State.AddActor(acting);
            }

            if (entity.TryGetTicking(out var ticking))
            {
                ticking.Init(entity);
                State.AddTicking(ticking);
            }

            Chains.Get(SpawnEntityIndex).Pass(entity);
            return entity;
        }

        public Entity SpawnEntity(EntityFactory factory, IntVector2 position)
        {
            return SpawnEntity(factory, position, IntVector2.Zero);
        }
    }
}