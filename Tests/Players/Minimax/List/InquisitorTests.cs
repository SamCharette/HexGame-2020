using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Players.Common;
using Players.Minimax.List;

namespace Tests.Players.Minimax.List
{
    [TestFixture]
    public class InquisitorTests
    {

        [Test]
        public void Inquisitor_ShouldWork()
        {
            var player = new ListPlayer(1, 6, new Config());
            var map = new ListMap(6);
            var inquisitor = new Inquisitor(map, player);
           // inquisitor.StartInquisition();
            TestContext.WriteLine("Dafuq?");
            //Assert.AreEqual(0, inquisitor.GetScore());
        }
    }
}
