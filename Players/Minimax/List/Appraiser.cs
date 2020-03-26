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
            var playerScore = PlayerScore(map, player.Me, map.GetPlayerMatrix(player.Me));
            // opponent score
            var opponentScore = PlayerScore(map, opponent, map.GetPlayerMatrix(opponent));

            var bluePathfinder = new Pathfinder(map, player.Me);
            var path = bluePathfinder.GetPathForPlayer(); 
            return playerScore + path.Count(x => x.Owner == player.Me) - opponentScore;
        }

        private int PlayerScore(ListMap map, PlayerType player, Matrix<double> playerMatrix)
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
