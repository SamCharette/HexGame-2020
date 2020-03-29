using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinimaxPlayer.List;
using Omu.ValueInjecter;
using Players.Common;

namespace Tests.Players.Minimax.List
{
    [TestFixture()]
    public class PathfinderTests
    {
       

        [Test()]
        public void GetPathForPlayerTest()
        {
            var map = new ListMap(11);
            var player = new ListPlayer(1, 11, new Config());
            var redPlayer = new ListPlayer(2, 11, new Config());

            var pathfinder = new Pathfinder(map, player.Me);

          

            //map.TakeHex(PlayerType.Red, 5, 1);
            pathfinder = new Pathfinder(map, player.Me);
            var path = pathfinder.GetPathForPlayer();

            //Assert.AreEqual(11, path.Count);


            map.TakeHex(PlayerType.Red, 3, 2);
            map.TakeHex(PlayerType.Red, 2, 2);
            map.TakeHex(PlayerType.Red, 2, 0);
            map.TakeHex(PlayerType.Red, 2, 1);
            map.TakeHex(PlayerType.Red, 2, 3);
            map.TakeHex(PlayerType.Red, 2, 4);
            map.TakeHex(PlayerType.Red, 2, 5);

            var blueMap = new ListMap(map.Size);
            blueMap.InjectFrom<CloneInjection>(map);
            
            pathfinder = new Pathfinder(map, redPlayer.Me);
            path = pathfinder.GetPathForPlayer();
            TestContext.WriteLine(pathfinder.GetLog());
            path.ForEach(x => map.TakeHex(PlayerType.Red, x.Row, x.Column));
            TestContext.WriteLine(map.GetMapMatrix().ToString().Replace('0', '_'));

            //pathfinder = new Pathfinder(blueMap, player, true);
            //path = pathfinder.GetPathForPlayer();
            //path.ForEach(x => blueMap.TakeHex(PlayerType.Blue, x.Row, x.Column));
            //TestContext.WriteLine(pathfinder.GetLog());

            //TestContext.WriteLine(blueMap.GetMapMatrix().ToString().Replace('0', '_'));

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