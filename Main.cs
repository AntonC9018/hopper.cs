using System.Collections;
using System.Collections.Generic;
using Chains;
using Core;

// Hello World! program
namespace HelloWorld
{
    class Hello
    {
        static void Print(string str)
        {
            System.Console.WriteLine(str);
        }

        static void Main(string[] args)
        {
            StatTest();
        }

        static void StatTest()
        {
            StatManager.RegisterStat("hello", 0);
            StatManager.RegisterCategory("cat1", new List<string> { "hello" });
            var stats = new StatManager();
            int helloStat = stats.GetStatSafe("hello");
            var statCategory = stats.GetStatCategory("cat1");
            System.Console.WriteLine(helloStat);
            System.Console.WriteLine(statCategory["hello"]);
            var mul = new Multiplier
            {
                additiveStats = new Dictionary<string, int>
                {
                    { "hello", 2 }
                },
                multiplicativeStats = new Dictionary<string, int>
                {
                    { "hello", 1 }
                }
            };
            stats.AddMultiplier(mul);
            helloStat = stats.GetStat("hello");
            System.Console.WriteLine(helloStat);

        }


        static void ChainTest(string[] args)
        {
            var chain = new Chain();

            System.Action<EventBase> func1 = (EventBase ev) => { Print(((TestEvent)ev).test); };
            System.Action<EventBase> func2 = (EventBase ev) => Print("second function");


            var handle2 = chain.AddHandler(func2);
            var handle = chain.AddHandler(func1);


            var ev = new TestEvent();

            chain.Pass(ev);
            chain.RemoveHandler(handle2);
            chain.Pass(ev);
        }

        public class TestEntity : Entity
        {
        }

        static void TinkerTest()
        {
            System.Console.WriteLine("Main started");

            var testEntityFactory = new EntityFactory { entityClass = typeof(TestEntity) };

            testEntityFactory.AddBehavior(Attackable.s_factory);

            var template = testEntityFactory.chainTemplates["beAttacked"];
            template.AddHandler((EventBase e) => System.Console.WriteLine("Hello from template"));

            var testEntity = (TestEntity)testEntityFactory.Instantiate();

            var attackable = (Attackable)testEntity.m_behaviors[Attackable.s_factory.id];
            attackable.Activate(testEntity, null, null);

            var testTinker = new Tinker
            {
                m_chainDefinition = new ChainDefinition[]
                {
                    new ChainDefinition
                    {
                        name = "beAttacked",
                        handlers = new WeightedEventHandler[]
                        {
                            new WeightedEventHandler
                            {
                                m_handlerFunction = (EventBase e) => System.Console.WriteLine("hello from the added handler"),
                                m_priority = 10000
                            }
                        }
                    }
                }
            };

            testEntity.AddTinker(testTinker);
            attackable.Activate(testEntity, null, null);
            testEntity.RemoveTinker(testTinker);

            attackable.Activate(testEntity, null, null);
        }

    }

    class TestEvent : EventBase
    {
        public string test = "hello";
    }
}