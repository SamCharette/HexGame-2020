using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Omu.ValueInjecter;
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

            var map = new ListMap(6);
            var player = new ListPlayer(1, map.Size, new Config());
            map.TakeHex(PlayerType.Red, 3, 3);
            var inquisitor = new Inquisitor();
            var testMap = new ListMap(map.Size);
            testMap.InjectFrom(map);
            inquisitor.StartInquisition(testMap, player);
            

//            Assert.AreEqual(1, 2);
        }
    }
}
