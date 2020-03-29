using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using MinimaxPlayer.List;
using NUnit.Framework;
using Omu.ValueInjecter;
using Players.Common;

namespace Tests.Players.Minimax.List
{
    [TestFixture]
    public class ListMapTests
    {
        private int Size;
        private MinimaxPlayer.List.ListMap map;

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
            Assert.AreEqual(PlayerType.Blue, map.HexAt(1,3).Owner);
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

        [Test]
        public void TakeHex_ShouldAttachAllMatrices_WhenAttachingToMultiples()
        {
            map = new ListMap(11);
            map.TakeHex(PlayerType.Red, 1, 1);
            map.TakeHex(PlayerType.Red, 1, 2);
            map.TakeHex(PlayerType.Red, 1, 3);
            map.TakeHex(PlayerType.Red, 1, 4);
            map.TakeHex(PlayerType.Red, 1, 5);

            map.TakeHex(PlayerType.Red, 1, 7);
            map.TakeHex(PlayerType.Red, 1, 8);
            map.TakeHex(PlayerType.Red, 1, 9);
            map.TakeHex(PlayerType.Red, 1, 10);

            Assert.AreEqual(112, map.Board.Count(x => x.Owner == PlayerType.White));

            Assert.AreSame(map.HexAt(1,1).Attached, map.HexAt(1,4).Attached);

            map.TakeHex(PlayerType.Red, 1, 6);
            Assert.AreSame(map.HexAt(1, 1).Attached, map.HexAt(1, 10).Attached);
            Assert.IsTrue(map.HexAt(1, 1).IsAttachedToRight);
            Assert.IsFalse(map.HexAt(1, 1).IsAttachedToLeft);
            map.TakeHex(PlayerType.Red, 1, 0);
            Assert.IsTrue(map.HexAt(1, 1).IsAttachedToLeft);
            Assert.IsTrue(map.HexAt(1, 1).IsAttachedToBothEnds());
        }

        [Test]
        public void ValueInjecter_ShouldProperlyClone_AFullMap()
        {
            map = new ListMap(11);
            map.TakeHex(PlayerType.Red, 1, 1);
            map.TakeHex(PlayerType.Red, 1, 2);
            map.TakeHex(PlayerType.Red, 1, 3);
            map.TakeHex(PlayerType.Red, 1, 4);
            map.TakeHex(PlayerType.Red, 1, 5);

            map.TakeHex(PlayerType.Blue, 1, 7);
            map.TakeHex(PlayerType.Blue, 1, 8);
            map.TakeHex(PlayerType.Blue, 1, 9);
            map.TakeHex(PlayerType.Blue, 1, 10);

            var newMap = new ListMap(map.Size);
            newMap.InjectFrom<CloneInjection>(map);
            Assert.AreEqual(5, newMap.Board.Count(x => x.Owner == PlayerType.Red));
            Assert.AreEqual(4, newMap.Board.Count(x => x.Owner == PlayerType.Blue));
            Assert.AreEqual(map.HexAt(1,1).ToTuple(), newMap.HexAt(1,1).ToTuple());
            Assert.AreEqual(map.HexAt(1,1).Attached, map.HexAt(1,1).Attached);
            newMap.TakeHex(PlayerType.Blue, 1, 6);
            Assert.AreNotEqual(map.Board.Count(x => x.Owner == PlayerType.Blue), newMap.Board.Count(x => x.Owner == PlayerType.Blue));
        }

        [Test]
        public void GetPlayerMatrix_ShouldBeAbleToMakeAMatrix_WhenCalledUpon()
        {
            map = new ListMap(11);
            map.TakeHex(PlayerType.Red, 1, 1);
            map.TakeHex(PlayerType.Red, 1, 2);
            map.TakeHex(PlayerType.Red, 1, 3);
            map.TakeHex(PlayerType.Red, 1, 4);
            map.TakeHex(PlayerType.Red, 1, 5);

            map.TakeHex(PlayerType.Blue, 1, 7);
            map.TakeHex(PlayerType.Blue, 1, 8);
            map.TakeHex(PlayerType.Blue, 1, 9);
            map.TakeHex(PlayerType.Blue, 1, 10);

            var blueMap = new ListMap(map.Size);
            blueMap.InjectFrom<CloneInjection>(map);

            var matrixForPlayer1 = blueMap.GetPlayerMatrix(PlayerType.Blue);
            TestContext.WriteLine("Player 1 matrix");
            TestContext.WriteLine(matrixForPlayer1.ToString());

            var matrixForPlayer2 = blueMap.GetPlayerMatrix(PlayerType.Red);
            TestContext.WriteLine("Player 2 matrix");
            TestContext.WriteLine(matrixForPlayer2.ToString());

            Assert.AreEqual(matrixForPlayer1, blueMap.HexAt(1,7).Attached);
        }
    }
}
