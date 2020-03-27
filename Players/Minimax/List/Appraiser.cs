using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using MathNet.Numerics.LinearAlgebra;
using Players.Base;
using Players.Common;

namespace Players.Minimax.List
{
    /*
     * The sole duty of this class is to determine a score for
     * the current board state.
     *
     */
    public class Appraiser
    {
        public int ScoreFromBoard(ListMap map, ListPlayer player)
        {

            var opponent = player.Me == PlayerType.Blue ? PlayerType.Red : PlayerType.Blue;
            // player score
            var playerScore = PlayerScore(map, player.Me);
            // opponent score
            var opponentScore = PlayerScore(map, opponent);

            var scout = new Pathfinder(map, player.Me);
            var path = scout.GetPathForPlayer(); 
            return playerScore  - opponentScore;
        }

        private int PlayerScore(ListMap map, PlayerType player)
        {
            var scout = new Pathfinder(map, player);
            var path = scout.GetPathForPlayer();
            var isFullyAttached = path.Where(x => x.Owner == player).Any(x => x.IsAttachedToBothEnds());
            
            if (isFullyAttached)
            {
                return player == PlayerType.Blue ? 9999 : -9999;
            }

            var score = path.Count() - path.Count(x => x.Owner == PlayerType.White);
            return score;
        }

        private int PlayerScore_Bad(ListMap map, PlayerType player, Matrix<double> playerMatrix)
        {
            if (player == PlayerType.Blue)
            {
                var pathVector = playerMatrix.RowSums();
                var numberLeft = pathVector.AsEnumerable().Count(x => (int)x == 0);
                return map.Size - numberLeft;
            }
            else
            {
                var pathVector = playerMatrix.ColumnSums();
                var numberLeft = pathVector.AsEnumerable().Count(x => (int)x == 0);
                return map.Size - numberLeft;
            }
        }
    }
}
