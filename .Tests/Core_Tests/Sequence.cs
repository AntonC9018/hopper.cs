using NUnit.Framework;
using Hopper.Core;
using System.Collections.Generic;
using Hopper.Core.ActingNS;

namespace Hopper.Tests
{
    public class Sequence_Test
    {
        public Step step_Hello;
        public Step step_World;
        public enum HW { Hello_Enter, Hello_Exit, World_Enter, World_Exit };
        public List<HW> events;

        public Sequence_Test()
        {
            events = new List<HW>();
        }

        [SetUp]
        public void Setup()
        {
            ResetEvents();
            step_Hello = new Step
            {
                enterFunction   = _ => events.Add(HW.Hello_Enter),
                exitFunction    = _ => events.Add(HW.Hello_Exit),
                successFunction = _ => new Result { success = true }
            };
            step_World = new Step
            {
                enterFunction   = _ => events.Add(HW.World_Enter),
                exitFunction    = _ => events.Add(HW.World_Exit),
                successFunction = _ => new Result { success = true }
            };
        }

        public void AssertEventSequence(params HW[] expected)
        {
            Assert.AreEqual(expected.Length, events.Count);

            for (int i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i], events[i]);
            }
        }

        public void ResetEvents() 
        {
            events.Clear();
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
            AssertEventSequence(HW.Hello_Exit, HW.Hello_Enter);
        }

        [Test]
        public void Sequence_SelectsStepsCorrectly()
        {
            var sequence = new Sequence(step_Hello, step_World);

            sequence.TickAction(null); // hello -> world
            AssertEventSequence(HW.Hello_Exit, HW.World_Enter);
            ResetEvents();
            
            sequence.TickAction(null); // world -> hello
            AssertEventSequence(HW.World_Exit, HW.Hello_Enter);
        }

        [Test]
        public void Step_IsRepeated()
        {
            step_Hello.repeat = 2; // repeat once more before continuing

            var sequence = new Sequence(step_Hello, step_World);

            sequence.TickAction(null); // stays at hello
            AssertEventSequence();

            sequence.TickAction(null); // hello -> world
            AssertEventSequence(HW.Hello_Exit, HW.World_Enter);
        }

        [Test]
        public void Step_LoopsBackwardCorrectly()
        {
            step_Hello.relativeStepIndexSuccess = -1;
            var sequence = new Sequence(step_Hello, step_Hello, step_World);

            sequence.TickAction(null); // | <- hello world <- |
            AssertEventSequence(HW.Hello_Exit, HW.World_Enter);
        }

        [Test]
        public void Step_RelativeIndexZero_KeepsTheStep()
        {
            step_Hello.relativeStepIndexSuccess = 0;
            var sequence = new Sequence(step_Hello, step_World);

            sequence.TickAction(null); // stays at hello
            AssertEventSequence();
        }
    }
}