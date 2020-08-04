using System.Collections;
using System.Collections.Generic;
using Chains;
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
            var chain = new Chain<TestEvent>();

            System.Action<TestEvent> func1 = (TestEvent ev) => Print(ev.test);
            System.Action<TestEvent> func2 = (TestEvent ev) => Print("second function");


            var handle2 = chain.AddHandler(func2);
            var handle = chain.AddHandler(func1);


            var ev = new TestEvent();

            chain.Pass(ev);
            chain.RemoveHandler(handle2);
            chain.Pass(ev);
        }

    }

    class TestEvent : EventBase
    {
        public string test = "hello";
    }
}