using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using Players;

namespace NegamaxPlayer
{
    /*
     * The sole duty of this class is to determine a score for
     * the current board state.
     *
     */
    public class Appraiser
    {
        public int ScoreFromBoard(Board map, int player)
        {

            var opponent = player == 1 ? -1 : 1;
            // player score
            var playerScore = PlayerScore(map, player);
            // opponent score
            var opponentScore = PlayerScore(map, opponent);

            return playerScore  - opponentScore;
        }


        private int PlayerScore(Board map, int player)
        {
            var scout = new Pathfinder(map, player);
            var path = scout.GetPathForPlayer();
 

            return PlayerScore(path);
            
        }

        private int PlayerScore(List<Hex> path)
        {
            var score = path.Count() - path.Count(x => x.Owner == 0);
            return score;
        }

       
    }
}
