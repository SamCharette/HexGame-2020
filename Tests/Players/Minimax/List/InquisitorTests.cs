using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Data;
using MinimaxPlayer.Minimax.List;
using NUnit.Framework;
using Omu.ValueInjecter;
using Players;

namespace Tests.Players.Minimax.List
{
    [TestFixture]
    public class InquisitorTests
    {

        [Test]
        public void Inquisitor_ShouldWork()
        {

            var map = new ListMap(6);
            var player = new MinimaxPlayer.Minimax.MinimaxPlayer(1, map.Size, new Config());
            map.TakeHex(PlayerType.Red, 3, 3);
            var inquisitor = new Inquisitor();
            var testMap = new ListMap(map.Size);
            testMap.InjectFrom(map);
            inquisitor.StartInquisition(testMap, player);
            

//            Assert.AreEqual(1, 2);
        }

        [Test]
        public void GetPossibleMoves_ShouldGivePriorities_Appropriately()
        {
            var map = new ListMap(6);
            var player = new MinimaxPlayer.Minimax.MinimaxPlayer(1, map.Size, new Config());
            var player2 = new MinimaxPlayer.Minimax.MinimaxPlayer(2, map.Size, new Config());
            map.TakeHex(PlayerType.Blue, 0, 3);
            map.TakeHex(PlayerType.Red, 3, 3);
            map.TakeHex(PlayerType.Blue, 2, 3);
            map.TakeHex(PlayerType.Red, 3, 2);
            map.TakeHex(PlayerType.Blue, 1, 3);
            map.TakeHex(PlayerType.Red, 3, 1);

            var scout = new Pathfinder(map, player.Me);
            var redScout = new Pathfinder(map, player2.Me);

            var path = scout.GetPathForPlayer();
            var redPath = redScout.GetPathForPlayer();

            var inquisitor = new Inquisitor();
            var possibleMovesForBlue = inquisitor.GetPossibleMoves(redPath, path, player.Me, map);

            TestContext.WriteLine("Blue =========");
            foreach (var move in path.Where(x => x.Owner == PlayerType.White))
            {
                TestContext.WriteLine(move);
            }
            TestContext.WriteLine("Red ========");
            foreach (var move in redPath.Where(x => x.Owner == PlayerType.White))
            {
                TestContext.WriteLine(move);
            }
            TestContext.WriteLine("Result =========");
            foreach (var move in possibleMovesForBlue)
            {
                TestContext.WriteLine(move + " " + move.Priority);
            }


        }
    }
}
