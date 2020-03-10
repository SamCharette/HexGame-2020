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
            var path = player.FindPath(player.Memory.Top, player.Memory.Bottom);
            Assert.AreEqual(11, path.Count);
            path = player.FindPath(player.Memory.Board.FirstOrDefault(x => x.Row == 6 && x.Column ==6), player.Memory.Bottom);
            Assert.AreEqual(4, path.Count);
        }

        //[Test]
        //public void StartNode_ShouldHaveLessThanSizeDistanceLeft_WhenItHasNeighbours()
        //{
        //    var player = new ListPlayer(1, 11, new Config());
        //    var node = player.Memory.Board.FirstOrDefault(x => x.Row == 0 && x.Column == 3);
        //    player.Memory.TakeHex(PlayerType.Blue, 0, 3);
        //    //Assert.True(player.Memory.Top.IsNeighboursWith(node));
        //    //Assert.AreEqual(11, player.Memory.Top.RemainingDistance());
        //    //player.Memory.TakeHex(PlayerType.Blue, 1, 3);
        //    //Assert.AreEqual(10, player.Memory.Top.RemainingDistance());
        //    //player.Memory.TakeHex(PlayerType.Blue, 2, 2);
        //    //Assert.AreEqual(9, player.Memory.Top.RemainingDistance());
        //    //player.Memory.TakeHex(PlayerType.Blue, 5, 2);
        //    //Assert.AreEqual(9, player.Memory.Top.RemainingDistance());

        //}
    }
}
