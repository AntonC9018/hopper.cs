using Hopper.Utils.Chains;
using NUnit.Framework;

namespace Hopper.Tests
{
    public class ChainTemplate
    {
        private Recorder recorder;
        private ControlledHandler handler_Hello;
        private ControlledHandler handler_World;
        private ChainTemplate<EventBase> chainTemplate;
        private EventBase ev;

        public ChainTemplate()
        {
            recorder = new Recorder();
            handler_Hello = new ControlledHandler("Hello", recorder);
            handler_World = new ControlledHandler("World", recorder);
        }

        [SetUp]
        public void Setup()
        {
            recorder.recordedSequence = "";
            chainTemplate = new ChainTemplate<EventBase>();
            ev = new EventBase();
        }

        [Test]
        public void EmptyChainTemplate_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => chainTemplate.Init());
        }

        [Test]
        public void IsHandlerAdded()
        {
            chainTemplate.AddHandler(handler_Hello.Function);
            var chain = (Chain<EventBase>)chainTemplate.Init();
            chain.Pass(ev);
            Assert.That(recorder.recordedSequence == "Hello");
        }

        [Test]
        public void RepeatedInits_ProduceSameChain()
        {
            chainTemplate.AddHandler(handler_Hello.Function);

            var chain = (Chain<EventBase>)chainTemplate.Init();
            chain.Pass(ev);
            Assert.That(recorder.recordedSequence == "Hello");

            recorder.recordedSequence = "";

            var chain2 = (Chain<EventBase>)chainTemplate.Init();
            chain2.Pass(ev);
            Assert.That(recorder.recordedSequence == "Hello");
        }

        [Test]
        public void HandlerPriority_WorksCorrectly()
        {
            chainTemplate.AddHandler(handler_World.Function, PriorityRanks.Low);
            chainTemplate.AddHandler(handler_Hello.Function, PriorityRanks.High);
            var chain = (Chain<EventBase>)chainTemplate.Init();
            chain.Pass(ev);
            Assert.That(recorder.recordedSequence == "HelloWorld");
        }

        [Test]
        public void OrderOfHandlers_IsCorrect_ForRepeatedInits()
        {
            chainTemplate.AddHandler(handler_World.Function, PriorityRanks.Low);
            chainTemplate.AddHandler(handler_Hello.Function, PriorityRanks.High);

            var chain = (Chain<EventBase>)chainTemplate.Init();
            chain.Pass(ev);
            Assert.That(recorder.recordedSequence == "HelloWorld");

            recorder.recordedSequence = "";

            var chain2 = (Chain<EventBase>)chainTemplate.Init();
            chain2.Pass(ev);
            Assert.That(recorder.recordedSequence == "HelloWorld");
        }
    }
}