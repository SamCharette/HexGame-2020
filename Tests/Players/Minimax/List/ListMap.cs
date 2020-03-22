using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Players.Common;
using Players.Minimax.List;

namespace Tests.Players.Minimax.List
{
    [TestFixture]
    public class ListMapTests
    {
        private int Size;
        private ListMap map;

        [SetUp]
        public void Initialize()
        {
           
        }

        [TearDown]
        public void TearDown()
        {
           
        }

        [Test]
        public void Constructor_ShouldMakeAMap_Period()
        {
            map = new ListMap(11);
            Assert.AreEqual(121, map.Board.Count);
        }

        [Test]
        public void TakeHex_ShouldChangeTheOwnerProperty_WhenHexIsUnowned()
        {
            map = new ListMap(11);
            map.TakeHex(PlayerType.Blue, 1, 3);
            Assert.AreEqual(PlayerType.Blue, map.At(1,3).Owner);
            Assert.AreEqual(120, map.Board.Count(x => x.Owner == PlayerType.White));
        }

        [Test]
        public void TakeHex_ShouldFail_IfHexAskedForIsOutOfBounds()
        {
            map = new ListMap(11);
            var didItWork = map.TakeHex(PlayerType.Blue, -13, 5000);
            Assert.AreEqual(false, didItWork);
            Assert.AreEqual(121, map.Board.Count(x => x.Owner == PlayerType.White));
        }
    }
}
