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
        private Pathfinder _scout;
        private ListPlayer _playerSearchingFor;
        private Thread _workerThread;
        private int _finalScore;
        private ListHex _finalChoice;
   

        public Inquisitor(ListMap map, ListPlayer player)
        {
            _playerSearchingFor = player;
            _map = new ListMap(map.Size);
            _map = Mapper.Map<ListMap>(map);
            //_scout = new Pathfinder(map, player);
            //_workerThread = new Thread(start: new ParameterizedThreadStart(StartLooking));
        }

        public int GetScore()
        {
            return _finalScore;
        }

        public ListHex GetChoice()
        {
            return _finalChoice;
        }

        public void StartInquisition()
        {
            var args = new InquisitionParameters
            {
                map = _map,
                path = _scout.GetPathForPlayer(),
                currentMove = null,
                depth = _playerSearchingFor.MaxLevels,
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
                args.map, 
                args.path, 
                args.currentMove, 
                args.depth, 
                args.alpha, 
                args.beta,
                args.isMaximizing);
        }

        private int ThinkAboutTheNextMove(
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

            _scout.SetMap(map);
            var myPath =  _scout.GetPathForPlayer();

            var possibleMoves = myPath.Where(x => x.Owner == PlayerType.White).ToList();
            if (isMaximizing)
            {
                foreach (var move in possibleMoves)
                {
                    var newMap = Mapper.Map<ListMap>(map);
                    alpha = Math.Max(alpha, ThinkAboutTheNextMove(newMap, myPath, move, depth - 1, alpha, beta, false));
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
                    beta = Math.Min(beta, ThinkAboutTheNextMove(newMap, myPath, move, depth - 1, alpha, beta, true));
                    
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
