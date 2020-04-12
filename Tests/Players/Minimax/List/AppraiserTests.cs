using System;
using System.Collections.Generic;
using System.Text;
using Data;
using MinimaxPlayer.Minimax.List;
using NUnit.Framework;
using Players;

namespace Tests.Players.Minimax.List
{
    [TestFixture]
    public class AppraiserTests
    {
        [Test]
        public void ScoreBoard_ShouldWork()
        {            
            var map = new ListMap(11);

            var bluePlayer = new ListPlayer(1, map.Size, new Config());
            var redPlayer = new ListPlayer(2, map.Size, new Config());
            map.TakeHex(PlayerType.Red, 1, 1);
            map.TakeHex(PlayerType.Red, 1, 2);
            map.TakeHex(PlayerType.Red, 1, 3);
            map.TakeHex(PlayerType.Red, 1, 4);
            map.TakeHex(PlayerType.Red, 1, 5);

            map.TakeHex(PlayerType.Blue, 1, 7);
            map.TakeHex(PlayerType.Blue, 1, 8);
            map.TakeHex(PlayerType.Blue, 1, 9);
            map.TakeHex(PlayerType.Blue, 1, 10);

            var matrixForPlayer1 = map.GetPlayerMatrix(PlayerType.Blue);
            TestContext.WriteLine("Player 1 matrix");
            TestContext.WriteLine(matrixForPlayer1.ToString());

            var matrixForPlayer2 = map.GetPlayerMatrix(PlayerType.Red);
            TestContext.WriteLine("Player 2 matrix");
            TestContext.WriteLine(matrixForPlayer2.ToString());

            var appraiser = new Appraiser();
            
            var player1score = appraiser.ScoreFromBoard(map, bluePlayer);
            var player2score = appraiser.ScoreFromBoard(map, redPlayer);
            Assert.AreNotEqual(player1score, player2score);

            Assert.AreEqual(4, player2score);
            Assert.AreEqual(-4, player1score);
        }

    }
}
