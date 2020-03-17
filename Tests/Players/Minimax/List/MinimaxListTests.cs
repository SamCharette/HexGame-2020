//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using NUnit.Framework;
//using NUnit.Framework.Constraints;
//using Players.Common;
//using Players.Minimax.List;

//namespace Tests.Players.Minimax.List
//{
//    public class MinimaxListTests 
//    {
//        [SetUp]
//        public void Setup()
//        {

//        }

//        [TearDown]
//        public void TearDown()
//        {

//        }

//        [Test]
//        public void FindBestPath_ShouldReturnSizeLength_WhenBoardIsEmpty()
//        {
//            var player = new ListPlayer(1, 11, new Config());
//            var path = player.FindPath(player.Memory.Top, player.Memory.Bottom, player.Me);
//            Assert.AreEqual(11, path.Count);
//            path = player.FindPath(player.Memory.Board.FirstOrDefault(x => x.Row == 6 && x.Column ==6), player.Memory.Bottom, player.Me);
//            Assert.AreEqual(4, path.Count);
//        }

//        [Test]
//        public void ScoreFromBoard_ShouldCorrectlyGiveScore_Always()
//        {
//            var player = new ListPlayer(1, 11, new Config());
//            player.Memory.TakeHex(player.Me, 0, 1);
//            player.Memory.TakeHex(player.Me, 1, 1);
//            Assert.AreEqual(2, player.ScoreFromBoard());
//        }

//        [Test]
//        public void ScoreFromBoard_ShouldCorrectlyGiveScore_Always2()
//        {
//            var player = new ListPlayer(1, 11, new Config());
//            player.Memory.TakeHex(player.Me, 0, 1);
//            player.Memory.TakeHex(player.Me, 1, 1);
//            player.Memory.TakeHex(player.Opponent(), 5, 0);
//            Assert.AreEqual(1, player.ScoreFromBoard());
//        }

//        //[Test]
//        //public void IsWinningMove_ShouldReturnTrue_WhenItFreakingIs()
//        //{
//        //    var player = new ListPlayer(1, 5, new Config());
//        //    player.Memory.TakeHex(player.Me, 3, 0);
//        //    player.Memory.TakeHex(player.Me, 3, 1);
//        //    player.Memory.TakeHex(player.Me, 3, 2);
//        //    player.Memory.TakeHex(player.Me, 3, 3);
//        //    player.Memory.TakeHex(player.Me, 3, 4);
//        //    var isAWinner = player.IsWinningMove(player.Me);
//        //    Assert.AreEqual(true, isAWinner);
//        //}
//        [Test]
//        public void Attaching_ShouldWork_AsYouAttach()
//        {
//            var player = new ListPlayer(1, 5, new Config());
//            var map = player.Memory;
            
//            map.TakeHex(player.Me, 0, 3);
//            map.TakeHex(player.Me, 1, 3);
//            var hex = map.FindHex(0, 3);
//            Assert.AreEqual(2, hex.Attached.Count);
//            map.TakeHex(player.Me, 2,3);
//            var hex2 = map.FindHex(2, 3);
//            Assert.AreEqual(3, hex2.Attached.Count());
//            map.TakeHex(player.Me, 3, 3);
//            map.TakeHex(player.Me, 4, 3);
//            var hex3 = map.FindHex(4, 3);

//            Assert.IsTrue(hex3.IsAttachedTo(hex));
//            Assert.IsTrue(hex3.IsAttachedTo(map.Bottom));
//            Assert.IsTrue(map.Bottom.IsAttachedTo(hex3));
//            Assert.IsTrue(map.Top.IsAttachedTo(hex));
//            Assert.IsTrue(hex2.IsAttachedTo(map.Bottom));
//            Assert.IsTrue(map.Bottom.IsAttachedTo(hex2));
//            Assert.IsTrue(map.Top.IsAttachedTo(map.Bottom));
//        }
//    }
//}
