using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
        private Tuple<int,int> _finalChoice;
   

        public Inquisitor()
        {

            _finalScore = -9999;
        }

        public int GetScore()
        {
            return _finalScore;
        }

        public Tuple<int,int> GetChoice()
        {
            return _finalChoice;
        }

        public void StartInquisition(ListMap searchMap, ListPlayer searchPlayer)
        {
            var mapToSearch = searchMap.GetCopyOf();
            var searchScout = new Pathfinder(mapToSearch, 
                searchPlayer.Me,
                searchPlayer.CostToMoveToClaimedNode,
                searchPlayer.CostToMoveToUnclaimedNode,
                searchPlayer.CostPerNodeTillEnd);

            _finalChoice = null;
            _finalScore = -9999;

            ThinkAboutTheNextMove(
                searchPlayer,
                mapToSearch,
                searchScout.GetPathForPlayer(),
                null,
                searchPlayer.CurrentLevels,
                -9999,
                9999,
                true);

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
                return judge.ScoreFromBoard(map, player);
            }
            
            if (currentMove != null)
            {
                map.TakeHex(isMaximizing ? player.Me : player.Opponent(), currentMove.Row, currentMove.Column);
            }

       

            var scout = new Pathfinder(map, isMaximizing ? player.Me : player.Opponent());
       
            var myPath =  scout.GetPathForPlayer();

            var possibleMoves = GetPossibleMoves(path, myPath);
            if (isMaximizing)
            {
                foreach (var move in possibleMoves)
                {
                    var bestScore = -9999;
                    var newMap = map.GetCopyOf();
                    bestScore = Math.Max(bestScore, 
                    ThinkAboutTheNextMove(
                        player, 
                        newMap, 
                        myPath, 
                        move, 
                        depth - 1, 
                        alpha, 
                        beta, 
                        false));

                    if (bestScore > alpha)
                    {
                        _finalChoice = move.ToTuple();
                        alpha = bestScore;
                    }
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
                    var worstScore = 9999;
                    var newMap = map.GetCopyOf();
                    worstScore = Math.Min(worstScore, ThinkAboutTheNextMove(
                        player, 
                        newMap, 
                        myPath, 
                        move, 
                        depth - 1, 
                        alpha, 
                        beta, 
                        true));
                    beta = Math.Min(worstScore, beta);
                    if (beta <= alpha)
                    {
                        break;
                    }
                }

                return beta;
            }
        }

        private IEnumerable<ListHex> GetPossibleMoves(List<ListHex> path, List<ListHex> myPath)
        {
            var possibleMoves = new List<ListHex>();
            var combined = path.Where(myPath.Contains).OrderBy(x => x.RandomValue).ToList();
            combined.ForEach(x => possibleMoves.Add(x));
            var myMoves = myPath.Where(x => !combined.Contains(x)).OrderBy(x => x.RandomValue).ToList();
            var theirMoves = path.Where(x => !combined.Contains(x)).OrderBy(x => x.RandomValue).ToList();
            theirMoves.ForEach(x => possibleMoves.Add(x));
            myMoves.ForEach(x => possibleMoves.Add(x));

            return possibleMoves.Where(x => x.Owner == PlayerType.White).ToList();
        }
    }
}
