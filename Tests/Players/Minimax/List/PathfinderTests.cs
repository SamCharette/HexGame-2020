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
            var map = new ListMap(11);
            var player = new ListPlayer(1, 11, new Config());
            var pathfinder = new Pathfinder(map, player);
            var path = pathfinder.GetPathForPlayer();
            PrintPath(path);
            Assert.AreEqual(11, path.Count);
            
            map.TakeHex(PlayerType.Red, 5, 0);
            pathfinder = new Pathfinder(map, player);
            path = pathfinder.GetPathForPlayer();
            PrintPath(path);
            //Assert.AreEqual(11, path.Count);

            map.TakeHex(PlayerType.Red, 10, 0);
            pathfinder = new Pathfinder(map, player);
            path = pathfinder.GetPathForPlayer();
            PrintPath(path);
            //Assert.AreEqual(11, path.Count);

            map.TakeHex(PlayerType.Red, 9, 10);
            pathfinder = new Pathfinder(map, player);
            path = pathfinder.GetPathForPlayer();
            PrintPath(path);
            //Assert.AreEqual(11, path.Count);
        }

        private void PrintPath(List<ListHex> path)
        {
            TestContext.WriteLine("================");
            path.ForEach(x => TestContext.WriteLine(x.ToString() + " " + x.F()));

        }
    }
}