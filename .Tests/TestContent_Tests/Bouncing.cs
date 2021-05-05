using NUnit.Framework;
using Hopper.Core;
using Hopper.Utils.Vector;
using System.Linq;
using Hopper.Core.Stat;
using Hopper.TestContent.Bind;
using Hopper.Core.Components.Basic;
using Hopper.Core.Targeting;

namespace Hopper.Tests.Test_Content
{
    public class Bouncing_Test
    {
        public readonly EntityFactory entityFactory;
        public readonly EntityFactory bindingFactory;

        public Bouncing_Test()
        {
            InitScript.Init();
        }

        [SetUp]
        public void Setup()
        {
            World.Global = new World(3, 3);
        }

        public GridManager Grid => World.Global.grid;

        [Test]
        public void Test_1()
        {
        }
    }
}