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
            TestContext.WriteLine("===========1===============");
            TestContext.WriteLine("===========================");
            var map = new ListMap(6);
            TestContext.WriteLine("============2==============");
            TestContext.WriteLine("===========================");
            var player = new ListPlayer(1, map.Size, new Config());
            TestContext.WriteLine("=============3=============");
            TestContext.WriteLine("===========================");
            map.TakeHex(PlayerType.Red, 3, 3);
            TestContext.WriteLine("============4==============");
            TestContext.WriteLine("===========================");
            var inquisitor = new Inquisitor();
            TestContext.WriteLine("============5==============");
            TestContext.WriteLine("===========================");

            var testMap = new ListMap(map.Size);
            TestContext.WriteLine("============6==============");
            TestContext.WriteLine("===========================");
            testMap.InjectFrom(map);
            TestContext.WriteLine("============7==============");
            TestContext.WriteLine("===========================");

            inquisitor.StartInquisition(testMap, player);
            TestContext.WriteLine("============8==============");
            TestContext.WriteLine("===========================");

//            Assert.AreEqual(1, 2);
        }
    }
}
