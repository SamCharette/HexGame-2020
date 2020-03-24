using NUnit.Framework;
using Players.Minimax.List;
using System;
using System.Collections.Generic;
using System.Text;
using Players.Common;

namespace Players.Minimax.List
{
    [TestFixture()]
    public class PathfinderTests
    {
       

        [Test()]
        public void GetPathForPlayerTest()
        {
            var map = new ListMap(6);
            var player = new ListPlayer(1, 6, new Config());
            var pathfinder = new Pathfinder(map, player);
            var path = pathfinder.GetPathForPlayer();
            TestContext.WriteLine(pathfinder.GetLog());
            Assert.AreEqual(6, path.Count);
            
            //map.TakeHex(PlayerType.Red, 5, 0);
            pathfinder = new Pathfinder(map, player);
            path = pathfinder.GetPathForPlayer();
            
            //Assert.AreEqual(11, path.Count);

            //map.TakeHex(PlayerType.Red, 5, 1);
            pathfinder = new Pathfinder(map, player);
            path = pathfinder.GetPathForPlayer();

            //Assert.AreEqual(11, path.Count);


            map.TakeHex(PlayerType.Red, 2, 0);
            map.TakeHex(PlayerType.Red, 2, 1);
            map.TakeHex(PlayerType.Red, 2, 3);
            map.TakeHex(PlayerType.Red, 2, 4);
            map.TakeHex(PlayerType.Red, 2, 5);
            pathfinder = new Pathfinder(map, player, true);
            path = pathfinder.GetPathForPlayer();
            TestContext.WriteLine(pathfinder.GetLog());


            //Assert.AreEqual(11, path.Count);
        }

        private void PrintPath(PlayerType player, List<ListHex> path)
        {
            TestContext.WriteLine("=============== " + (player == PlayerType.Blue ? " Blue " : " Red ") + " ===============");
            TestContext.WriteLine("--------------- " + path.Count + " ---------------");
            PrintEntirePathAndValues(path);

        }

        private void PrintEntireMapAndAllRelevantValues(List<ListHex> board)
        {
            TestContext.WriteLine("======== Board ========");
            foreach (var hex in board)
            {
                TestContext.WriteLine(hex.ToLongString());

            }
        }

        private void PrintEntirePathAndValues(List<ListHex> path)
        {
            path.ForEach(x => TestContext.WriteLine(x.ToString() + " " + x.F()));
        }
    }
}