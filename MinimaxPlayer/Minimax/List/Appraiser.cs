using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using Players;

namespace MinimaxPlayer.Minimax.List
{
    /*
     * The sole duty of this class is to determine a score for
     * the current board state.
     *
     */
    public class Appraiser
    {
        public int ScoreFromBoard(ListMap map, MinimaxPlayer player)
        {

            var opponent = player.Me == PlayerType.Blue ? PlayerType.Red : PlayerType.Blue;
            // player score
            var playerScore = PlayerScore(map, player.Me);
            // opponent score
            var opponentScore = PlayerScore(map, opponent);

            return playerScore  - opponentScore;
        }

        public int ScoreFromBoard(MinimaxPlayer player, List<ListHex> playerPath, List<ListHex> opponentPath)
        {
            var opponent = player.Me == PlayerType.Blue ? PlayerType.Red : PlayerType.Blue;
            // player score
            var playerScore = PlayerScore(playerPath);
            // opponent score
            var opponentScore = PlayerScore(opponentPath);

            return playerScore - opponentScore;
        }

        private int PlayerScore(ListMap map, PlayerType player)
        {
            var scout = new Pathfinder(map, player);
            var path = scout.GetPathForPlayer();
            var isFullyAttached = path.Where(x => x.Owner == player).Any(x => x.IsAttachedToBothEnds());
            
            if (isFullyAttached)
            {
                Console.WriteLine((string) ("Winning move found! " + player));
                return player == PlayerType.Blue ? 5000 : -5000;
            }

            return PlayerScore(path);
            
        }

        private int PlayerScore(List<ListHex> path)
        {
            var score = path.Count() - path.Count(x => x.Owner == PlayerType.White);
            return score;
        }

        private int PlayerScore_Bad(ListMap map, PlayerType player, Matrix<double> playerMatrix)
        {
            if (player == PlayerType.Blue)
            {
                var pathVector = playerMatrix.RowSums();
                var numberLeft = Enumerable.AsEnumerable<double>(pathVector).Count(x => (int)x == 0);
                return map.Size - numberLeft;
            }
            else
            {
                var pathVector = playerMatrix.ColumnSums();
                var numberLeft = Enumerable.AsEnumerable<double>(pathVector).Count(x => (int)x == 0);
                return map.Size - numberLeft;
            }
        }
    }
}
