using Hopper.Core.History;
using NUnit.Framework;

namespace Hopper.Tests
{
    public class History_Tests
    {
        private History<string> history;
        private Trackable trackable_Hello;
        private Trackable trackable_World;
        private UpdateCode testCode;

        public class Trackable : ITrackable<string>
        {
            public string message;
            public string GetState() => message;
        }

        public History_Tests()
        {
            trackable_Hello = new Trackable { message = "Hello" };
            trackable_World = new Trackable { message = "World" };
            testCode = new UpdateCode("Test");
        }

        [SetUp]
        public void Setup()
        {
            history = new History<string>();
        }

        [Test]
        public void EmptyHistory_GetsInitialState()
        {
            Assert.That(history.Updates.Count == 0, "At start, no events exist");
            history.InitControlUpdate(trackable_Hello);
            Assert.That(history.Updates.Count == 1, "We have initialized the history with a control event with an initial state");
        }

        [Test]
        public void ControlUpdate_HasRightData()
        {
            history.InitControlUpdate(trackable_Hello);
            Assert.That(history.Updates[0].updateCode == UpdateCode.control);
            Assert.That(history.Updates[0].stateAfter == "Hello");
        }

        [Test]
        public void RightDataAreAdded()
        {
            history.Add(trackable_World.GetState(), testCode);

            Assert.That(history.Updates[0].updateCode == testCode);
            Assert.That(history.Updates[0].stateAfter == "World");
        }

        [Test]
        public void LastState_BecomesControlState_AfterClear()
        {
            history.InitControlUpdate(trackable_Hello);
            history.Add(trackable_World.GetState(), testCode);
            history.Clear();

            Assert.That(history.Updates[0].updateCode == UpdateCode.control);
            Assert.That(history.Updates[0].stateAfter == "World");
        }

        [Test]
        public void SearchBefore_WithoutAnyUpdates_Throws()
        {
            Assert.Throws<System.ArgumentOutOfRangeException>(() => history.GetStateBefore(testCode));
        }

        [Test]
        public void SearchBefore_WorksCorrectly()
        {
            history.InitControlUpdate(trackable_Hello);
            history.Add(trackable_World.GetState(), testCode);
            var state = history.GetStateBefore(testCode);
            Assert.That(state == "Hello");
        }
    }
}