using Hopper.Utils.Chains;
using NUnit.Framework;

namespace Hopper.Tests
{
    public class Chains
    {
        private Recorder recorder;
        private ControlledHandler handler_Hello;
        private ControlledHandler handler_World;
        private Chain<EventBase> chain;
        private EventBase ev;

        public Chains()
        {
            recorder = new Recorder();
            handler_Hello = new ControlledHandler("Hello", recorder);
            handler_World = new ControlledHandler("World", recorder);
        }

        [SetUp]
        public void Setup()
        {
            recorder.recordedSequence = "";
            chain = new Chain<EventBase>();
            ev = new EventBase();
        }

        [Test]
        public void EmptyChain_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => chain.Pass(ev));
        }

        [Test]
        public void EmptyChain_AddingHandlerDefault_ReturnsHandle()
        {
            var handle = chain.AddHandler(handler_Hello.Function);
            Assert.IsNotNull(handle);
        }

        [Test]
        public void IsAddedHandlerCalled()
        {
            var handle = chain.AddHandler(handler_Hello.Function);
            // the handler will be called
            chain.Pass(ev);
            // assert it were called
            Assert.AreEqual("Hello", recorder.recordedSequence);
        }

        [Test]
        public void DoesAddedHandlerStay_AfterPass()
        {
            var handle = chain.AddHandler(handler_Hello.Function);
            chain.Pass(ev);
            chain.Pass(ev);
            Assert.AreEqual("HelloHello", recorder.recordedSequence);
        }

        [Test]
        public void IsAddedHandlerRemovedByHandle_BeforeCall()
        {
            var handle = chain.AddHandler(handler_Hello.Function);
            chain.RemoveHandler(handle);
            chain.Pass(ev);
            Assert.AreEqual("", recorder.recordedSequence);
        }

        [Test]
        public void IsAddedHandlerRemovedByHandle_AfterCall()
        {
            var handle = chain.AddHandler(handler_Hello.Function);
            chain.Pass(ev);
            chain.RemoveHandler(handle);
            chain.Pass(ev);
            Assert.AreEqual("Hello", recorder.recordedSequence);
        }

        [Test]
        public void AreHandlers_AddedInOrder_CalledInTheSameOrder()
        {
            var handle_Hello = chain.AddHandler(handler_Hello.Function);
            var handle_World = chain.AddHandler(handler_World.Function);
            chain.Pass(ev);
            Assert.AreEqual("HelloWorld", recorder.recordedSequence);
        }

        [Test]
        public void AreHandlers_CalledByPriority()
        {
            var handle_Hello = chain.AddHandler(handler_Hello.Function, PriorityRank.High);
            var handle_World = chain.AddHandler(handler_World.Function, PriorityRank.Medium);
            chain.Pass(ev);
            Assert.AreEqual("HelloWorld", recorder.recordedSequence);
        }

        [Test]
        public void AreHandlers_CalledByPriority_IndependentOfOrderTheyWereAdded()
        {
            var handle_World = chain.AddHandler(handler_World.Function, PriorityRank.Medium);
            var handle_Hello = chain.AddHandler(handler_Hello.Function, PriorityRank.High);
            chain.Pass(ev);
            Assert.AreEqual("HelloWorld", recorder.recordedSequence);
        }

        [Test]
        public void IsPropagateVerified()
        {
            var handler_Stop = new EvHandler<EventBase>(ev => ev.propagate = false, PriorityRank.Medium);

            var handle_Hello = chain.AddHandler(handler_Hello.Function, PriorityRank.High);
            var handle_World = chain.AddHandler(handler_World.Function, PriorityRank.Low);
            var handle_Stop = chain.AddHandler(handler_Stop);

            // use the default stop function, which checks `propagation`
            chain.Pass(ev);

            Assert.AreEqual("Hello", recorder.recordedSequence);
        }

        [Test]
        public void EventHandler_IsNotModified()
        {
            var handler_test = new EvHandler<EventBase>(ev => { }, PriorityRank.Medium);
            Assert.AreEqual((int)PriorityRank.Medium, handler_test.Priority);
            chain.AddHandler(handler_test);
            chain.Pass(ev);
            Assert.AreEqual((int)PriorityRank.Medium, handler_test.Priority);
        }

        [Test]
        public void IsStopFunctionExecuted()
        {
            int counter = 0;
            var handle_Hello = chain.AddHandler(handler_Hello.Function);

            // counter was checked at 1, now has value 1, handler is called
            chain.Pass(ev, ev => ++counter == 2);
            Assert.That(counter == 1, "Counter incremented");
            Assert.AreEqual("Hello", recorder.recordedSequence, "First call successful");

            // counter was checked at 2, now has value 2, handler is not called
            chain.Pass(ev, ev => ++counter == 2);
            Assert.That(counter == 2, "Counter incremented");
            Assert.AreEqual("Hello", recorder.recordedSequence, "Second call not successful, as expected");

            // counter was checked at 3, handler is called
            chain.Pass(ev, ev => ++counter == 2);
            Assert.That(counter == 3, "Counter incremented");
            Assert.AreEqual("HelloHello", recorder.recordedSequence, "Third call successful");
        }



        // [Test]
        // public void RemovingHandlerTwiceThrows()
        // {
        // }
    }
}