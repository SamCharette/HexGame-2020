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
   

        public Inquisitor(ListMap map, ListPlayer player)
        {
            _playerSearchingFor = player;
            _map.InjectFrom<CloneInjection>(map);
            _scout = new Pathfinder(map, player);
        }

        public void StartInquisition()
        {
            
        }

        public int ThinkAboutTheNextMove(
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
                return judge.ScoreFromBoard(map);
            }

            var myPath =  _scout.GetPathForPlayer();

            var possibleMoves = myPath.Where(x => x.Owner == PlayerType.White).ToList();
            if (isMaximizing)
            {
                foreach (var move in possibleMoves)
                {
                    alpha = Math.Max(alpha, ThinkAboutTheNextMove(map, myPath, move, depth - 1, alpha, beta, false));
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
                    beta = Math.Min(beta, ThinkAboutTheNextMove(map, myPath, move, depth - 1, alpha, beta, true));
                    
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
