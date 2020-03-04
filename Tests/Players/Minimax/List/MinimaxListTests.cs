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
        }

        [Test]
        public void FindBestPath_ShouldReturnSmallerSizeLength_WhenBoardHasHexes()
        {
            var player = new ListPlayer(1, 11, new Config());
            var node = player.Memory.Board.FirstOrDefault(x => x.Row == 5 && x.Column == 3);
            node.Owner = PlayerType.Blue;
            node = player.Memory.Board.FirstOrDefault(x => x.Row == 8 && x.Column == 3);
            node.Owner = PlayerType.Blue;
            var path = player.StartLookingForBestPath(true, player.Memory);
            Assert.AreEqual(9, path.Count(x => x.Owner == PlayerType.White));
        }
    }
}
