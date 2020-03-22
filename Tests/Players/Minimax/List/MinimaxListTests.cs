using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using Players.Common;
using Players.Minimax.List;

namespace Tests.Players.Minimax.List
{
    [TestFixture]
    public class MinimaxListTests
    {
        [SetUp]
        public void Setup()
        {

        }

        [TearDown]
        public void TearDown()
        {

        }


        [Test]
        public void Attaching_ShouldWork_AsYouAttach()
        {
            var hex = new ListHex(11, 5, 5);
            Assert.AreEqual(6, hex.Neighbours.Count);
            var hex2 = new ListHex(11, 0, 1);
            Assert.AreEqual(4, hex2.Neighbours.Count);
        }
    }
}
