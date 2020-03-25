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
            StartLooking(args);
            //_workerThread.Start(args);
        }

        private void StartLooking(Object obj)
        {
            var args = (InquisitionParameters) obj;
            Console.WriteLine("Starting to look");
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
                return judge.ScoreFromBoard(map, player);
            }

            var scout = new Pathfinder(map, player);
       
            var myPath =  scout.GetPathForPlayer();

            var possibleMoves = GetPossibleMoves(path, myPath);
            if (isMaximizing)
            {
                foreach (var move in possibleMoves)
                {
                    var newMap = Mapper.Map<ListMap>(map);
                    var newScore = ThinkAboutTheNextMove(player, newMap, myPath, move, depth - 1, alpha, beta, false);
                    if (newScore > alpha)
                    {
                        player.CurrentChoice = move.ToTuple();
                        alpha = newScore;
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

        private IEnumerable<ListHex> GetPossibleMoves(List<ListHex> path, List<ListHex> myPath)
        {
            var possibleMoves = new List<ListHex>();
            var combined = path.Where(myPath.Contains).OrderBy(x => x.RandomValue).ToList();
            combined.ForEach(x => possibleMoves.Add(x));
            var myMoves = myPath.Where(x => !combined.Contains(x)).OrderBy(x => x.RandomValue).ToList();
            var theirMoves = path.Where(x => !combined.Contains(x)).OrderBy(x => x.RandomValue).ToList();
            myMoves.ForEach(x => possibleMoves.Add(x));
            theirMoves.ForEach(x => possibleMoves.Add(x));

            return possibleMoves;
        }
    }
}
