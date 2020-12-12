using NUnit.Framework;
using Hopper.Core.Items;
using Hopper.Core;

namespace Hopper.Tests
{
    public class Sequence_Test
    {
        public Recorder recorder;
        public Step step_Hello;
        public Step step_World;

        public Sequence_Test()
        {
            recorder = new Recorder();
        }

        [SetUp]
        public void Setup()
        {
            recorder.recordedSequence = "";
            step_Hello = new Step
            {
                enterFunction = _ => recorder.Record("Enter_Hello"),
                exitFunction = _ => recorder.Record("Exit_Hello"),
                successFunction = _ => new Result { success = true }
            };
            step_World = new Step
            {
                enterFunction = _ => recorder.Record("Enter_World"),
                exitFunction = _ => recorder.Record("Exit_World"),
                successFunction = _ => new Result { success = true }
            };
        }

        [Test]
        public void Sequence_MustHave_AtLeastOneStep()
        {
            Assert.Throws<Hopper.Utils.Exception>(() => new Sequence(new Step[0]));
        }

        [Test]
        public void Sequence_LoopsBackTo_TheOnlyStep()
        {
            var sequence = new Sequence(step_Hello);
            sequence.TickAction(null);
            Assert.AreEqual("Exit_Hello" + "Enter_Hello", recorder.recordedSequence);
        }

        [Test]
        public void Sequence_SelectsStepsCorrectly()
        {
            var sequence = new Sequence(step_Hello, step_World);

            sequence.TickAction(null); // hello -> world
            Assert.AreEqual("Exit_Hello" + "Enter_World", recorder.recordedSequence);
            recorder.recordedSequence = "";

            sequence.TickAction(null); // world -> hello
            Assert.AreEqual("Exit_World" + "Enter_Hello", recorder.recordedSequence);
        }

        [Test]
        public void Step_IsRepeated()
        {
            step_Hello.repeat = 2; // repeat once more before continuing

            var sequence = new Sequence(step_Hello, step_World);

            sequence.TickAction(null); // stays at hello
            Assert.AreEqual("", recorder.recordedSequence);

            sequence.TickAction(null); // hello -> world
            Assert.AreEqual("Exit_Hello" + "Enter_World", recorder.recordedSequence);
        }

        [Test]
        public void Step_LoopsBackwardCorrectly()
        {
            step_Hello.relativeStepIndexSuccess = -1;
            var sequence = new Sequence(step_Hello, step_Hello, step_World);

            sequence.TickAction(null); // | <- hello world <- |
            Assert.AreEqual("Exit_Hello" + "Enter_World", recorder.recordedSequence);
        }

        [Test]
        public void Step_RelativeIndexZero_KeepsTheStep()
        {
            step_Hello.relativeStepIndexSuccess = 0;
            var sequence = new Sequence(step_Hello, step_World);

            sequence.TickAction(null); // stays at hello
            Assert.AreEqual("", recorder.recordedSequence);
        }
    }
}