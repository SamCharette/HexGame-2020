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
        public int ScoreFromBoard(ListMap map, PlayerType player)
        {

            var opponent = player == PlayerType.Blue ? PlayerType.Red : PlayerType.Blue;
            // player score
            var playerScore = PlayerScore(map, player, map.GetPlayerMatrix(player));
            // opponent score
            var opponentScore = PlayerScore(map, player, map.GetPlayerMatrix(opponent));

            return playerScore - opponentScore;
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
