using System.Collections.Generic;
using Core.Utils.Vector;
using Chains;
using Core;
using Core.Items;
using System.Linq;
using Core.Targeting;
using Core.Behaviors;
using Test;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Core.Generation;
using Core.Stats;
using Core.Stats.Basic;
using Core.History;

// Hello World! program
namespace Hopper
{
    class Hello
    {
        static void Main(string[] args)
        {
            // Serialize();
            // Demo();
            // Generate();
            // Release();
            // Explode();
            // Bounce();
            // GoldTest();
            // GhostTest();
            // SlideTest();
            // ShieldTest();
        }

        public static void ShieldTest()
        {
            var shield = new ModularItem(3, ShieldModule.CreateFront(2));
            var enemyFactory = new EntityFactory<Entity>().AddBehavior<Attackable>();
            var playerFactory = new EntityFactory<Entity>()
                .AddBehavior<Attacking>()
                .AddBehavior<Acting>(new Acting.Config(Algos.SimpleAlgo));
            var world = new World(5, 5);
            var player = world.SpawnPlayer(playerFactory, new IntVector2(1, 1));
            player.Reorient(new IntVector2(1, 0));
            var enemy = world.SpawnEntity(enemyFactory, new IntVector2(2, 1));
            enemy.Reorient(new IntVector2(-1, 0));
            shield.BeEquipped(enemy);
            var attackAction = new BehaviorAction<Attacking>();
            player.Behaviors.Get<Acting>().NextAction = attackAction.WithDir(new IntVector2(1, 0));
            world.Loop();
            player.Stats.GetRaw(Attack.Path).pierce += 2;
            player.Behaviors.Get<Acting>().NextAction = attackAction.WithDir(new IntVector2(1, 0));
            world.Loop();
        }

        public static void SlideTest()
        {
            var world = new World(5, 5);
            var p_fact = new EntityFactory<Entity>()
                .AddBehavior<Acting>(new Acting.Config(Algos.SimpleAlgo))
                .AddBehavior<Attacking>()
                .AddBehavior<Moving>()
                .AddBehavior<Displaceable>()
                .AddBehavior<Statused>()
                .Retouch(Core.Retouchers.Reorient.OnDisplace)
                .Retouch(Core.Retouchers.Skip.EmptyAttack);

            var player = world.SpawnPlayer(p_fact, new IntVector2(1, 1));
            var ice_fact = IceFloor.CreateFactory();
            var ice1 = world.SpawnEntity(ice_fact, new IntVector2(2, 1));
            var ice2 = world.SpawnEntity(ice_fact, new IntVector2(3, 1));
            var action = new CompositeAction(
                new BehaviorAction<Attacking>(),
                new BehaviorAction<Moving>()
            );
            void SetAction(IntVector2 vec) => player.Behaviors.Get<Acting>().NextAction
                = action.Copy().WithDir(vec);
            void Print()
            {
                System.Console.WriteLine($"Is status applied? {SlideStatus.Status.IsApplied(player)}");
                System.Console.WriteLine($"Position: {player.Pos}");
            }

            SetAction(IntVector2.Right);
            world.Loop();
            Print();

            SetAction(IntVector2.Down);
            world.Loop();
            Print();

            SetAction(IntVector2.Down);
            world.Loop();
            Print();

            SetAction(IntVector2.Down);
            world.Loop();
            Print();
        }

        public static void GhostTest()
        {
            var world = new World(5, 5);
            var p_fact = new EntityFactory<Entity>()
                .AddBehavior<Acting>(new Acting.Config(Algos.SimpleAlgo))
                .AddBehavior<Attacking>();
            var player = world.SpawnPlayer(p_fact, new IntVector2(1, 1));
            player.Stats.GetRaw(Attack.Path).damage = 10;

            var ghost = world.SpawnEntity(Ghost.CreateFactory(), player.Pos + IntVector2.Right);
            player.Behaviors.Get<Acting>().NextAction =
                new BehaviorAction<Attacking>().WithDir(IntVector2.Right);

            world.Loop();

            System.Console.WriteLine($"Player's new position {player.Pos}");
        }

        public static void GoldTest()
        {
            var world = new World(5, 5);
            var p_fact = Player.CreateFactory().Retouch(Gold.PickUpRetoucher);
            var player = world.SpawnPlayer(p_fact, new IntVector2(1, 1));

            var gold = Gold.Drop(player.Pos + IntVector2.Right, 1, world);
            var gold2 = Gold.Drop(player.Pos + IntVector2.Right, 1, world);

            System.Console.WriteLine($"Gold1 == Gold2? {gold == gold2}");
            System.Console.WriteLine($"Gold amount: {gold.Amount}");

            player.Behaviors.Get<Acting>().NextAction =
                new BehaviorAction<Moving>().WithDir(IntVector2.Right);
            world.Loop();

            System.Console.WriteLine($"Is gold dead? {gold.IsDead}");
            System.Console.WriteLine($"Gold amount: {gold.Amount}");
        }

        public static void Bounce()
        {
            var world = new World(5, 5);
            var player = world.SpawnPlayer(Player.CreateFactory(), new IntVector2(1, 1));
            var trap = world.SpawnEntity(BounceTrap.Factory, new IntVector2(1, 2));
            trap.Reorient(new IntVector2(1, 0));
            player.Behaviors.Get<Acting>().NextAction =
                new BehaviorAction<Moving>().WithDir(new IntVector2(0, 1));
            world.Loop();
            System.Console.WriteLine($"Player is at {player.Pos}");
        }

        public static void Explode()
        {
            // var defStat = new StatManager();
            // defStat.GetRaw(Attack.Resistance.Path.String, new Attack.Resistance { pierce = 10 });
            var world = new World(5, 5);
            var player = world.SpawnPlayer(Player.CreateFactory(), new IntVector2(1, 1));
            Explosion.Explode(new IntVector2(2, 2), 1, world);
            // player.Stats.DefaultStats = defStat;
            // player.Stats.GetRaw(Attack.Source.Resistance.Path).content[Explosion.Source.Id] = 10;
            // player.Stats.GetRaw(Attack.Resistance.Path).pierce = 10;
        }

        public class TestContent : IContent
        {
            public static EntityFactory<Entity> factory = new EntityFactory<Entity>();

            public void Release(Entity entity)
            {
                System.Console.WriteLine("spawned");
                entity.World.SpawnEntity(factory, entity.Pos);
            }
        }

        static void Release()
        {
            var content = new TestContent();
            var interactableEntityFactory = new EntityFactory<Entity>()
                .AddBehavior<Interactable>();
            var world = new World(1, 1);
            var entity = world.SpawnEntity(interactableEntityFactory, new IntVector2(0, 0));
            var interactable = entity.Behaviors.Get<Interactable>();
            interactable.m_content = content;
            interactable.Activate();
            var releasedContent = entity.Cell.GetFirstEntity();
            System.Console.WriteLine($"Entity = content entity? - {entity == releasedContent}");
        }

        static void Generate()
        {
            Generator generator = new Generator(50, 50, new Generator.Options());
            generator.graph.Print();
            generator.AddRoom(new IntVector2(5, 5));
            generator.graph.Print();
            generator.AddRoom(new IntVector2(5, 5));
            generator.graph.Print();
            generator.AddRoom(new IntVector2(5, 5));
            generator.graph.Print();
            generator.AddRoom(new IntVector2(5, 5));
            generator.graph.Print();
            generator.AddRoom(new IntVector2(5, 5));
            generator.graph.Print();
            generator.Generate();
        }

        static void Serialize()
        {
            var playerFactory = new EntityFactory<Player>();
            playerFactory.AddBehavior<Attackable>();
            playerFactory.AddBehavior<Attacking>();
            playerFactory.AddBehavior<Displaceable>();
            playerFactory.AddBehavior<Moving>();
            playerFactory.AddBehavior<Pushable>();
            playerFactory.AddBehavior<Statused>();
            playerFactory.AddBehavior<Sequential>(new Sequential.Config(new Step[0]));
            System.Console.WriteLine("Set up playerFactory");

            var player = playerFactory.Instantiate();
            World world = new World(1, 1);
            player.Init(new IntVector2(1, 1), world);
            var item = new TinkerItem(new Tinker<TinkerData>(new ChainDef<EventBase>[] { }));
            var item2 = new TinkerItem(new Tinker<TinkerData>(new ChainDef<EventBase>[] { }), 1);
            var packed = IdMap.Items.PackModMap();
            ((Inventory)player.Inventory).AddContainer(3, new CircularItemContainer(5));
            player.Inventory.Equip(item);
            player.Inventory.Equip(item2);

            MemoryTraceWriter traceWriter = new MemoryTraceWriter();
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Converters.Add(new IHaveIdConverter<IItem>());
            settings.Converters.Add(new InventoryConverter());
            // settings.Converters.Add(new BehaviorConverter());
            settings.Converters.Add(new BehaviorControlConverter());
            settings.Converters.Add(new EntityConverter());
            settings.TraceWriter = traceWriter;
            string result = JsonConvert.SerializeObject(player, settings);
            System.Console.WriteLine(result);
            Entity entity = JsonConvert.DeserializeObject<Player>(result, settings);
            System.Console.WriteLine(entity);
            // System.Console.WriteLine(traceWriter.ToString());

        }

        static void Demo()
        {
            var ____ = Spider.Factory;
            var _____ = TestTinkerStuff.tinker;
            var ______ = TestStatusStuff.status;

            System.Console.WriteLine("\n ------ Definition + Instantiation Demo ------ \n");

            World world = new World
            {
                m_grid = new GridManager(5, 5),
                m_state = new WorldStateManager()
            };
            System.Console.WriteLine("Created world");

            // Statused.RegisterStatus(TestStatusTinkerStuff.status, 1);
            var packed = IdMap.Tinker.PackModMap();
            IdMap.Tinker.SetServerMap(packed);

            var playerFactory = new EntityFactory<Player>();
            playerFactory.AddBehavior<Attackable>();
            playerFactory.AddBehavior<Attacking>();
            playerFactory.AddBehavior<Displaceable>();
            playerFactory.AddBehavior<Moving>();
            playerFactory.AddBehavior<Pushable>();
            playerFactory.AddBehavior<Statused>();
            System.Console.WriteLine("Set up playerFactory");

            Acting.Config playerActingConf = new Acting.Config(Algos.SimpleAlgo, null);

            playerFactory.AddBehavior<Acting>(playerActingConf);
            playerFactory.Retouch(Core.Retouchers.Skip.EmptyAttack);

            // this one's for the equip demo
            playerFactory.Retouch(Core.Retouchers.Equip.OnDisplace);


            var enemyFactory = new EntityFactory<Entity>();
            enemyFactory.AddBehavior<Attackable>();
            enemyFactory.AddBehavior<Attacking>();
            enemyFactory.AddBehavior<Displaceable>();
            enemyFactory.AddBehavior<Moving>();
            enemyFactory.AddBehavior<Pushable>();


            Acting.Config enemyActingConf = new Acting.Config(Algos.EnemyAlgo);

            enemyFactory.AddBehavior<Acting>(enemyActingConf);


            var attackAction = new BehaviorAction<Attacking>();
            var moveAction = new BehaviorAction<Moving>();
            CompositeAction attackMoveAction = new CompositeAction(
                new Action[] { attackAction, moveAction }
            );

            Step[] stepData =
            {
                new Step { action = null },
                new Step { action = attackMoveAction, movs = Movs.Basic }
            };

            var sequenceConfig = new Sequential.Config(stepData);

            enemyFactory.AddBehavior<Sequential>(sequenceConfig);
            System.Console.WriteLine("Set up enemyFactory");

            Entity player = playerFactory.Instantiate();
            System.Console.WriteLine("Instantiated Player");

            Entity enemy = enemyFactory.Instantiate();
            System.Console.WriteLine("Instantiated Enemy");

            enemy.Init(new IntVector2(1, 2), world);
            world.m_state.AddEntity(enemy);
            world.m_grid.Reset(enemy, enemy.Pos);
            System.Console.WriteLine("Enemy set in world");

            player.Init(new IntVector2(1, 1), world);
            world.m_state.AddPlayer(player);
            world.m_grid.Reset(player, player.Pos);
            System.Console.WriteLine("Player set in world");

            var playerNextAction = attackMoveAction.Copy();
            playerNextAction.direction = new IntVector2(0, 1);
            player.Behaviors.Get<Acting>().NextAction = playerNextAction;
            System.Console.WriteLine("Set player action");
            System.Console.WriteLine("\n ------ Modifier Demo ------ \n");

            var mod = Modifier.Create(Attack.Path, new Attack { damage = 1 });//new StatModifier(Attack.Path, new Attack { damage = 1 });
            var attack = player.Stats.Get(Attack.Path);
            System.Console.WriteLine("Attack damage:{0}", attack.damage);
            System.Console.WriteLine("Attack pierce:{0}", attack.pierce);
            System.Console.WriteLine("Adding modifier");
            mod.AddSelf(player.Stats);
            attack = player.Stats.Get(Attack.Path);
            System.Console.WriteLine("Attack damage:{0}", attack.damage);
            System.Console.WriteLine("Attack pierce:{0}", attack.pierce);
            System.Console.WriteLine("Removing modifier");
            mod.RemoveSelf(player.Stats);
            attack = player.Stats.Get(Attack.Path);
            System.Console.WriteLine("Attack damage:{0}", attack.damage);
            System.Console.WriteLine("Attack pierce:{0}", attack.pierce);

            var mod2 = Modifier.Create(Attack.Path, new Chains.EvHandler<StatEvent<Attack>>(
                (StatEvent<Attack> eve) =>
                {
                    System.Console.WriteLine("Called handler");
                    eve.file.damage *= 3;
                })
            );
            System.Console.WriteLine("Adding modifier");
            mod2.AddSelf(player.Stats);
            attack = player.Stats.Get(Attack.Path);
            System.Console.WriteLine("Attack damage:{0}", attack.damage);
            System.Console.WriteLine("Attack pierce:{0}", attack.pierce);
            System.Console.WriteLine("Removing modifier");
            player.Stats.RemoveChainModifier(mod2);
            attack = player.Stats.Get(Attack.Path);
            System.Console.WriteLine("Attack damage:{0}", attack.damage);
            System.Console.WriteLine("Attack pierce:{0}", attack.pierce);

            System.Console.WriteLine("\n ------ Pools Demo ------ \n");

            PoolItem[] items = new[]
            {
                new PoolItem(0, 1),
                new PoolItem(1, 1),
                new PoolItem(2, 1),
                new PoolItem(3, 1),
                new PoolItem(4, 10)
            };

            var pool = Pool.CreateNormal<IItem>();

            pool.AddRange("zone1/weapons", items.Take(2));
            pool.AddRange("zone1/trinkets", items.Skip(2).Take(2));
            pool.Add("zone1/trinkets", items[4]);

            var poolCopy = pool.Copy();

            var it1 = pool.GetNextItem("zone1/weapons");
            System.Console.WriteLine($"Item Id = {it1.id}, q = {it1.quantity}");
            // var it2 = pool.GetNextItem("zone1/weapons");
            // System.Console.WriteLine($"Item Id = {it2.id}, q = {it2.quantity}");
            var it3 = poolCopy.GetNextItem("zone1/weapons");
            System.Console.WriteLine($"Item Id = {it3.id}, q = {it3.quantity}");
            var it4 = poolCopy.GetNextItem("zone1/weapons");
            System.Console.WriteLine($"Item Id = {it4.id}, q = {it4.quantity}");


            System.Console.WriteLine("\n ------ TargetProvider Demo ------ \n");
            var pattern = new Pattern
            (
                new Piece
                {
                    pos = new IntVector2(1, 0),
                    dir = new IntVector2(1, 0),
                    reach = null
                },
                new Piece
                {
                    pos = new IntVector2(2, 0),
                    dir = new IntVector2(1, 0),
                    reach = new List<int>()
                }
            );
            var weapon = TargetProvider.CreateAtk(pattern, Handlers.GeneralChain);
            System.Console.WriteLine($"Enemy is at {enemy.Pos}");
            var ev = Target.CreateEvent<AtkTarget>(player, playerNextAction.direction);
            var atk = player.Stats.Get(Attack.Path);
            var targets = weapon.GetParticularTargets(ev, new Attackable.Params(atk, player));
            foreach (var t in targets)
            {
                System.Console.WriteLine($"Entity at {t.targetEntity.Pos} has been considered a potential target");
            }

            System.Console.WriteLine("\n ------ Inventory Demo ------ \n");
            var inventory = (Inventory)player.Inventory;

            var cyclicContainer = new CircularItemContainer(1);
            // indeces 0 and 1 are reserved for weapon and shovel respectively
            inventory.AddContainer(4, cyclicContainer);

            var tinker = Tinker<TinkerData>.SingleHandlered<Attacking.Event>(
                Attacking.Check,
                e => System.Console.WriteLine("Hello from tinker applied by item")
            );
            var item = new TinkerItem(tinker, 4);

            // inventory.Equip(item) ->         // the starting point
            // item.BeEquipped(entity) ->       // it's interface method
            // tinker.Tink(entity)  ->          // adds the handlers
            // entity.Tinkers.SetStore()        // creates the tinker data
            inventory.Equip(item);

            world.Loop();
            System.Console.WriteLine("Looped");

            // creates excess as the size is 1
            inventory.Equip(item);
            // inventory.DropExcess() ->
            // item.BeUnequipped()    ->
            // tinker.Untink() -> entity.Tinkers.RemoveStore()
            // + world.CreateDroppedItem(id, pos)
            inventory.DropExcess();

            var entities = world.m_grid.GetCellAt(player.Pos).m_entities;
            System.Console.WriteLine($"There's {entities.Count} entities in the cell where the player is standing");

            System.Console.WriteLine("\n ------ History Demo ------ \n");
            var enemyUpdates = enemy.History.Updates;
            var playerUpdates = player.History.Updates;
            foreach (var updateInfo in enemyUpdates)
            {
                System.Console.WriteLine($"Enemy did {System.Enum.GetName(typeof(UpdateCode), updateInfo.updateCode)}. Position after: {updateInfo.stateAfter.pos}");
            }
            foreach (var updateInfo in playerUpdates)
            {
                System.Console.WriteLine($"Player did {System.Enum.GetName(typeof(UpdateCode), updateInfo.updateCode)}. Position after: {updateInfo.stateAfter.pos}");
            }

            System.Console.WriteLine("\n ------ Equip on Displace Demo ------ \n");
            // we don't need the entity for the next test
            enemy.Die();

            var playerMoveAction = moveAction.Copy();
            playerMoveAction.direction = IntVector2.Down;
            player.Behaviors.Get<Acting>().NextAction = playerMoveAction;

            var cyclicContainer2 = new CircularItemContainer(1);
            inventory.AddContainer(3, cyclicContainer);

            var chainDefs2 = new IChainDef[0];
            var tinker2 = new Tinker<TinkerData>(chainDefs2);
            var item2 = new TinkerItem(tinker2, 3);

            var droppedItem2 = world.SpawnDroppedItem(item2, player.Pos + IntVector2.Down);

            /*
            this only works because we did
            `playerFactory.AddRetoucher(Core.Retouchers.Equip.OnDisplace);`
            up top.
            */

            System.Console.WriteLine($"Player's position before moving: {player.Pos}");
            world.Loop();
            System.Console.WriteLine($"Player's new position: {player.Pos}");
            System.Console.WriteLine($"There's {world.m_grid.GetCellAt(player.Pos).m_entities.Count} entities in the cell where the player is standing");


            System.Console.WriteLine("\n ------ Tinker static reference Demo ------ \n");

            var tinker3 = TestTinkerStuff.tinker; // see the definition below
            tinker3.Tink(player);
            player.Behaviors.Get<Acting>().NextAction = playerMoveAction;
            world.Loop();
            tinker3.Untink(player);


            System.Console.WriteLine("\n ------ Status Demo ------ \n");

            // this has to be rethought for sure
            var status = TestStatusStuff.status;
            status.TryApply(
                player,
                new StatusData(),
                new StatusFile { power = 1, amount = 2 }
            );
            player.Behaviors.Get<Acting>().NextAction = playerMoveAction;
            world.Loop();
            player.Behaviors.Get<Acting>().NextAction = playerMoveAction;
            world.Loop();
            player.Behaviors.Get<Acting>().NextAction = playerMoveAction;
            world.Loop();

            System.Console.WriteLine("\n ------ Spider Demo ------ \n");
            player.ResetPosInGrid(new IntVector2(4, 4));
            var spider = world.SpawnEntity(Spider.Factory, new IntVector2(3, 3));
            world.Loop();
            System.Console.WriteLine($"Tinker is applied? {BindStatuses.NoMove.IsApplied(player)}");
            System.Console.WriteLine("Looped");
            System.Console.WriteLine($"Player's new position: {player.Pos}");
            System.Console.WriteLine($"Spider's new position: {spider.Pos}");

            player.Behaviors.Get<Acting>().NextAction = attackAction;
            world.Loop();
            System.Console.WriteLine("Looped");
            System.Console.WriteLine($"Player's new position: {player.Pos}");
            System.Console.WriteLine($"Spider's new position: {spider.Pos}");

            player.Behaviors.Get<Displaceable>().Activate(new IntVector2(-1, -1), new Move());
            world.Loop();
            System.Console.WriteLine("Looped");
            System.Console.WriteLine($"Player's new position: {player.Pos}");
            System.Console.WriteLine($"Spider's new position: {spider.Pos}");

            System.Console.WriteLine("Killing spider");
            spider.Die();
            world.Loop();
            System.Console.WriteLine($"Tinker is applied? {BindStatuses.NoMove.IsApplied(player)}");

            System.Console.WriteLine("\n ------ Input Demo ------ \n");
            // we also have the possibilty to add behaviors dynamically.
            var InputFactory = new BehaviorFactory<Controllable>();
            var input = (Controllable)InputFactory.Instantiate(player,
                new Controllable.Config { defaultAction = attackMoveAction });
            player.Behaviors.Add(typeof(Controllable), input);

            var outputAction0 = input.ConvertInputToAction(InputMapping.Up);
            System.Console.WriteLine($"Fed Up. Output: {outputAction0.direction}");
            var outputAction1 = input.ConvertInputToAction(InputMapping.Special_0);
            System.Console.WriteLine($"Fed Special_0. Output is null?: {outputAction1 == null}");
        }
    }

    public class TestTinkerData : TinkerData
    {
        public int i = 1;
    }

    public static class TestTinkerStuff
    {
        static void TestMethod1(Displaceable.Event actorEvent)
        {
            var data = tinker.GetStore(actorEvent);
            System.Console.WriteLine($"Tinker says that i = {data.i}");
        }
        public static Tinker<TestTinkerData> tinker = Tinker<TestTinkerData>
            .SingleHandlered<Displaceable.Event>(Displaceable.Do, TestMethod1);
    }

    public static class TestStatusStuff
    {
        static void TestMethod1(StandartEvent standartEvent)
        {
            var data = status.Tinker.GetStore(standartEvent);
            System.Console.WriteLine($"Tinker says that amount = {data.amount}");
        }
        public static Status<StatusData> status = new Status<StatusData>(
            new ChainDefBuilder()
                .AddDef<Acting.Event>(Acting.Check)
                .AddHandler(TestMethod1)
                .End().ToStatic(),
            null,
            0
        );
    }
}