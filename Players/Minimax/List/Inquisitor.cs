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

            var scout = new Pathfinder(map, isMaximizing ? player.Me : player.Opponent());
       
            var myPath =  scout.GetPathForPlayer();

            var possibleMoves = GetPossibleMoves(path, myPath, isMaximizing ? player.Me : player.Opponent(), map);
            if (isMaximizing)
            {
                foreach (var move in possibleMoves)
                {
                    var bestScore = -9999;
                    var newMap = map.GetCopyOf();
                    newMap.TakeHex(player.Me, move.Row, move.Column);

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
                    newMap.TakeHex(player.Opponent(), move.Row, move.Column);
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

        public IEnumerable<ListHex> GetPossibleMoves(List<ListHex> path, List<ListHex> myPath, PlayerType player, ListMap map)
        {
            var opponent = player == PlayerType.Blue ? PlayerType.Red : PlayerType.Blue;
            var lastOpponentMove = opponent == PlayerType.Blue ? map.LastBlueMove : map.LastRedMove;
            
            var possibleMoves = new HashSet<ListHex>();

            if (lastOpponentMove != null)
            {
                foreach (var move in lastOpponentMove.Neighbours)
                {
                    var hex = map.HexAt(move.ToTuple());
                    if (hex != null && hex.Owner == PlayerType.White)
                    {
                        hex.Priority+=3;
                        possibleMoves.Add(hex);
                    }
                }
            }

            foreach (var move in path)
            {
                var hex = map.HexAt(move.ToTuple());
                if (hex !=null && hex.Owner == PlayerType.White)
                {
                    hex.Priority+=2;
                    possibleMoves.Add(hex);
                }

            }

            foreach (var move in myPath)
            {
                var hex = map.HexAt(move.ToTuple());
                if (hex != null && hex.Owner == PlayerType.White)
                {
                    hex.Priority++;
                    possibleMoves.Add(hex);
                }
            }

            //foreach (var move in map.Board)
            //{
            //    if (move.Owner == PlayerType.White)
            //    {
            //        possibleMoves.Add(move);
            //    }
            //}

            var possibleMovesList = possibleMoves
                .OrderByDescending(x => x.Priority)
                .ThenBy(x => x.DistanceTo(lastOpponentMove))
                .ThenBy(x => x.RandomValue).Where(x => x.Owner == PlayerType.White).ToList();


            //Console.Write("Possible moves: ");
            //possibleMovesList.ForEach(x => Console.Write(x + " "));
            //Console.WriteLine("");

            return possibleMovesList;
        }
    }
}
