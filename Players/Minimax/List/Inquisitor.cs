using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Omu.ValueInjecter;
using Players.Common;

namespace Players.Minimax.List
{
    /*
     * This class is the threaded worker that will be given a move and
     * asked to look ahead to see if it can be a good one
     */
    public class Inquisitor
    {
        private ListMap _map;
        private Thread _workerThread;
        private int _finalScore;
        private ListHex _finalChoice;
   

        public Inquisitor()
        {
           
            _workerThread = new Thread(StartLooking);
        }

        public int GetScore()
        {
            return _finalScore;
        }

        public ListHex GetChoice()
        {
            return _finalChoice;
        }

        public void StartInquisition(ListMap searchMap, ListPlayer searchPlayer)
        {
            var mapToSearch = new ListMap(searchMap.Size);
            mapToSearch.InjectFrom(searchMap);
            var searchScout = new Pathfinder(mapToSearch, searchPlayer, true);
            var args = new InquisitionParameters
            {
                player = searchPlayer,
                map = mapToSearch,
                path = searchScout.GetPathForPlayer(),
                currentMove = null,
                depth = searchPlayer.MaxLevels,
                alpha = -9999,
                beta = 9999,
                isMaximizing = true
            };
            _workerThread.Start(args);
        }

        private void StartLooking(Object obj)
        {
            var args = (InquisitionParameters) obj;

            _finalScore = ThinkAboutTheNextMove(
                args.player,
                args.map, 
                args.path, 
                args.currentMove, 
                args.depth, 
                args.alpha, 
                args.beta,
                args.isMaximizing);
        }

        private int ThinkAboutTheNextMove(
            ListPlayer player,
          ListMap map,
          List<ListHex> path,
          ListHex currentMove,
          int depth,
          int alpha,
          int beta,
          bool isMaximizing)
        {
            var judge = new Appraiser();

            if (depth == 0 || map.Board.All(x => x.Owner != Common.PlayerType.White))
            {
                return judge.ScoreFromBoard(map, PlayerType.Blue);
            }

            var scout = new Pathfinder(map, player);
       
            var myPath =  scout.GetPathForPlayer();

            var possibleMoves = myPath.Where(x => x.Owner == PlayerType.White).ToList();
            if (isMaximizing)
            {
                foreach (var move in possibleMoves)
                {
                    var newMap = Mapper.Map<ListMap>(map);
                    alpha = Math.Max(alpha, ThinkAboutTheNextMove(player, newMap, myPath, move, depth - 1, alpha, beta, false));
                    if (beta <= alpha)
                    {
                        break;
                    }
                }

                return alpha;
            }
            else 
            {
                foreach (var move in possibleMoves)
                {
                    var newMap = Mapper.Map<ListMap>(map);
                    beta = Math.Min(beta, ThinkAboutTheNextMove(player, newMap, myPath, move, depth - 1, alpha, beta, true));
                    
                    if (beta <= alpha)
                    {
                        break;
                    }
                }

                return beta;
            }
        }
    }
}
