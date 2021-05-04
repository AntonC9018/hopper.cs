using System.Linq;
using Hopper.Core;
using Hopper.Shared.Attributes;
using Hopper.Utils.Chains;
using Hopper.Utils.Vector;
using NUnit.Framework;

namespace Hopper.Tests
{
    public class ChainTests
    {
        public ChainTests()
        {
        }

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void PrioritiesAreAssignedCorrectly()
        {
            var assigner = new PriorityAssigner();
            assigner.Init();
            Assert.Less(assigner.Next(PriorityRank.High), assigner.Next(PriorityRank.Medium));

            var pr1 = assigner.Next(PriorityRank.Medium);
            var pr2 = assigner.Next(PriorityRank.Medium);
            Assert.AreNotEqual(pr1, pr2);
        }

        public class Context : ContextBase
        {
            public int counter;
        }

        [Test]
        public void Handler()
        {
            var ctx = new Context();
            var handler = new Handler<Context>(1, _ctx => _ctx.counter++);

            // The handler is added through the Add method
            var chain = new Chain<Context> { handler };
            Assert.That(chain.Contains(handler));

            // The handler remains after a pass
            // It also gets executed            
            chain.PassNoCondition(ctx);
            Assert.That(chain.Contains(handler));
            Assert.AreEqual(1, ctx.counter);

            // The second handler overrides the first handler
            // Since it is executed after that, setting the value to 5 
            var handler2 = new Handler<Context>(2, _ctx => _ctx.counter = 5);
            chain.Add(handler2);
            chain.PassNoCondition(ctx);
            Assert.That(chain.Contains(handler2));
            Assert.AreEqual(5, ctx.counter);

            // Removing a handler works
            // The previous handler is the only one executing, incrementing the value
            chain.Remove(handler2);
            Assert.False(chain.Contains(handler2));
            chain.PassNoCondition(ctx);
            Assert.AreEqual(6, ctx.counter);

            // If you were to stop propagation, the value will not be incremented
            var handler3 = new Handler<Context>(0, _ctx => _ctx.propagate = false);
            chain.Add(handler3);
            chain.Pass(ctx);
            Assert.AreEqual(6, ctx.counter);

            // Removing all handlers
            chain.Clear();

            // You may also remove a handler while a pass is being made
            // This is achieved by creating a copy of the array with handlers before iterating.
            // There are better ways to do it, which I'll propably get to later, 
            // if these copies become a problem.
            var handler5 = new Handler<Context>(5, _ctx => _ctx.counter = 9);
            var handler4 = new Handler<Context>(4, _ctx => chain.Remove(handler5));
            chain.AddMany(handler4, handler5);

            ctx.propagate = true;
            chain.Pass(ctx);
            Assert.AreEqual(9, ctx.counter);
            
            ctx.counter = 69;
            chain.Pass(ctx);
            Assert.AreEqual(69, ctx.counter);
        }
    }
}