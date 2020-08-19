using System.Collections;
using System.Collections.Generic;
using Chains;
using Core;

// Hello World! program
namespace Test
{
    class Hello
    {
        static void Print(string str)
        {
            System.Console.WriteLine(str);
        }

        static void Main_(string[] args)
        {
            EventTest();
            // ClassTest();
            // StatTest();
            // ChainTest();
            // TinkerTest();
        }

        class EventHolderObj
        {
            public event System.Action ev;
            public void Call()
            {
                ev?.Invoke();
            }
        }

        class EventTestLambda
        {
            string testStr;
            public EventTestLambda(EventHolderObj eventHolder, string t)
            {
                testStr = t;
                eventHolder.ev += () => System.Console.WriteLine(testStr);
            }

            public void Detach(EventHolderObj eventHolder)
            {
                eventHolder.ev -= () => System.Console.WriteLine(testStr);
            }

            ~EventTestLambda()
            {
                System.Console.WriteLine("Destroyed EventTestLambda");
            }
        }

        class EventTestMemberFunc
        {
            string testStr = "MemberFunc";
            int id;

            void Func()
            {
                System.Console.WriteLine($"{testStr}. Id: {id}");
            }

            public EventTestMemberFunc(EventHolderObj eventHolder, int i)
            {
                id = i;
                eventHolder.ev += Func;
            }

            public void Detach(EventHolderObj eventHolder)
            {
                eventHolder.ev -= Func;
            }

            ~EventTestMemberFunc()
            {
                System.Console.WriteLine("Destroyed EventTestMemberFunc");
            }
        }

        class EventTestA
        {
            ~EventTestA()
            {
                System.Console.WriteLine("Destroyed EventTestA");
            }
        }

        static void InnerEventTest(EventHolderObj e)
        {
            {
                EventTestLambda l = new EventTestLambda(e, "test");
                e.Call();
                l.Detach(e);
            }
            e.Call();

            {
                EventTestMemberFunc m = new EventTestMemberFunc(e, 0);
                EventTestMemberFunc m2 = new EventTestMemberFunc(e, 1);
                e.Call();
                m2.Detach(e);
            }
            e.Call();
        }

        static void EventTest()
        {
            EventHolderObj e = new EventHolderObj();
            InnerEventTest(e);
            e.Call();
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
            e.Call();
        }


        abstract class A
        {
            public virtual void a(int i = 0) { System.Console.WriteLine($"Called A's a({i})"); }
        }

        class B : A
        {
            public override void a(int i) { System.Console.WriteLine($"Called B's a({i})"); }
        }

        static void ClassTest()
        {
            A b = new B();
            b.a();
            b.a(1);
            B bee = (B)b;
            bee.a(1);
            bee.a(1);
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


        static void ChainTest()
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

            var testEntityFactory = new EntityFactory(typeof(TestEntity));

            testEntityFactory.AddBehavior(Attackable.s_factory);

            // var template = testEntityFactory.m_chainTemplates["beAttacked"];
            // template.AddHandler((EventBase e) => System.Console.WriteLine("Hello from template"));

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
                                handlerFunction = (EventBase e) => System.Console.WriteLine("hello from the added handler"),
                                priority = 10000
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