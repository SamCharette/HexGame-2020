using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Players.Common;
using Players.Minimax.List;

namespace Tests.Players.Minimax.List
{
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
        public void FindBestPath_ShouldReturnSizeLength_WhenBoardIsEmpty()
        {
            var player = new ListPlayer(1, 11, new Config());
            var path = player.StartLookingForBestPath(true, player.Memory);
            Assert.AreEqual(11, path.Count);
            player.Memory.TakeHex(PlayerType.Blue, 1, 5);
            path = player.StartLookingForBestPath(true, player.Memory);
            Assert.AreNotEqual(path, null);
            Assert.AreNotEqual(path.Count, 0);
            player.Memory.TakeHex(PlayerType.Red, 1, 6);
            path = player.StartLookingForBestPath(true, player.Memory);
            Assert.AreNotEqual(path, null);
            Assert.AreNotEqual(path.Count, 0);
        }

        [Test]
        public void Distance_ShouldBe3_WhenNodesAre3Apart()
        {
            var node1 = new ListNode(11, 1, 3);
            var node2 = new ListNode(11, 4, 1);
            Assert.AreEqual(3, node1.DistanceTo(node2));
        }

        [Test]
        public void StartNode_ShouldHaveLessThanSizeDistanceLeft_WhenItHasNeighbours()
        {
            var player = new ListPlayer(1, 11, new Config());
            var node = player.Memory.Board.FirstOrDefault(x => x.Row == 0 && x.Column == 3);
            player.Memory.TakeHex(PlayerType.Blue, 0, 3);
            Assert.True(player.Memory.Top.IsNeighboursWith(node));
            player.Memory.Top.Owner = PlayerType.Blue;
            player.Memory.Bottom.Owner = PlayerType.Blue;
            Assert.AreEqual(11, player.Memory.Top.GetDistanceToEnd());
            player.Memory.TakeHex(PlayerType.Blue, 1, 3);
            Assert.AreEqual(10, player.Memory.Top.GetDistanceToEnd());
            player.Memory.TakeHex(PlayerType.Blue, 2, 2);
            Assert.AreEqual(9, player.Memory.Top.GetDistanceToEnd());
            player.Memory.TakeHex(PlayerType.Blue, 5, 2);
            Assert.AreEqual(9, player.Memory.Top.GetDistanceToEnd());
            var nodeLastTaken = player.Memory.Board.FirstOrDefault(x => x.Row == 5 && x.Column == 2);
            Assert.AreEqual(5, nodeLastTaken.GetDistanceToEnd());
            Assert.AreEqual(4, nodeLastTaken.GetDistanceToTop());
            Assert.AreEqual(8, nodeLastTaken.GetDistanceToRight());
            Assert.AreEqual(1, nodeLastTaken.GetDistanceToLeft());
        }
    }
}
