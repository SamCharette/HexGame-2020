using System;
using System.Collections.Generic;
using System.Text;
using Players;

namespace NegamaxPlayer
{
    public class Negamax : Player
    {
        public Board Board { get; set; }
        public int MaxDepth { get; set; }

        public Negamax(int playerNumber, int boardSize, Config playerConfig) : base(playerNumber, boardSize, playerConfig)
        {
            Setup(boardSize);
        }

        public Negamax()
        {

        }

        public void Setup(int size)
        {
            Size = size;
            Board = new Board();
            Board.Setup(Size);
        }

        public override Tuple<int, int> SelectHex(Tuple<int, int> opponentMove)
        {
            return JustGetARandomHex().ToTuple();
        }

        private PlayerType Opponent()
        {
            return this.PlayerNumber == 1 ? Players.PlayerType.Red : Players.PlayerType.Blue;
        }

        private Hex StartSearch()
        {
            Hex currentChoice = null;
            var bestScoreSoFar = -9999;
            // Get possible moves
            // Order them
            var childNodes = new List<Hex>();
            // foreach move possible do negamax
            foreach (var node in childNodes)
            {
                var moveValue = DoNegamax(Board, MaxDepth, -9999, 9999, PlayerNumber);
                if (moveValue > bestScoreSoFar)
                {
                    bestScoreSoFar = moveValue;
                    currentChoice = node;
                }
            }
            // if the result is the best, set a hex variable and the best result variable

            // At the end, return the hex that was last found
            return currentChoice;
        }

        private int DoNegamax(Board gameState, int currentDepth, int alpha, int beta, int playerNumber)
        {
            if (currentDepth == 0 || gameState.HasWinner())
            {
                return playerNumber == this.PlayerNumber ? gameState.Score() : gameState.Score() * -1;
            }

            var value = -9999;
            var childNodes = new List<Hex>();
            // Get child nodes (moves in my path plus moves in their path)
            // Order the child nodes (both paths first, then mine, then theirs)

            // Foreach child node
            foreach (var node in childNodes)
            {
                var newBoard = Board.GetCopy();
                newBoard.TakeHex(node);

                value = Math.Max(value,
                    -1 * DoNegamax(newBoard, 
                        currentDepth - 1, 
                        -1 * alpha, 
                        -1 * beta, 
                        EnemyPlayerNumber));
                alpha = Math.Max(alpha, value);
                if (alpha >= beta)
                {
                    break;
                }
            }
            // value = max of value and -StartSearch(child node, depth -1, -alpha, -beta, opponent)
            // alpha = max value of alpha and value
            // if alpha >= beta then exit

            return value;
        }

        public override string PlayerType()
        {
            return "Negamax AI";
        }
    }
}
