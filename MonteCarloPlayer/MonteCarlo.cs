using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Complex;
using Players;

namespace MonteCarloPlayer
{
    public class MonteCarlo : Player
    {
        public Tuple<int,int> CurrentBestMove { get; set; }
        public Matrix<int> Board { get; set; }

        private int RandomMovesMade = 0;
        private int MovesMade = 0;
        private int NumberOfSecondsForSearch;

        public MonteCarlo(int playerNumber, int boardSize, Config playerConfig) 
            : base(playerNumber, boardSize, playerConfig)
        {
            Size = boardSize;
            PlayerNumber = playerNumber;
            Board = Matrix<int>.Build.Dense(Size, Size, 0);
            Setup(playerConfig);
        }

        public int MyPlayerNumber => PlayerNumber == 2 ? -1 : 1;
        public int EnemyPlayerNumber => PlayerNumber == 2 ? 1 : -1;

        public override Tuple<int, int> SelectHex(Tuple<int, int> opponentMove)
        {
            if (opponentMove != null)
            {
                Board[opponentMove.Item1, opponentMove.Item2] = EnemyPlayerNumber;
            }

            CurrentBestMove = null;

            StartLookingForAHex();

            MovesMade++;
            return CheckBestMove();
        }

        public void StartLookingForAHex()
        {

            var RootNode = new Node(Board) {PlayerNumber = MyPlayerNumber};

            var end = DateTime.Now.AddSeconds(NumberOfSecondsForSearch);

            // Monte Carlo has 4 main parts
            while (DateTime.Now < end)
            {
                // 1) Selection
                // Select children until a leaf is found (one where no playout has been made for).
                var node = GetBestNode(RootNode);

                // 2) Expansion
                // Unless the leaf found in 1 ends the game in a win or loss, create one (or more)
                // child nodes (moves)
                if (!IsFinalMove(node.Board))
                {
                    var childNode = GetRandomChildNode(node.Board, MyPlayerNumber);
                    node.Children.Add(childNode);
                    childNode.Parent = node;

                    // 3) Simulation
                    // For the child(ren) above, play out games by itself until a win or loss is found
                    var winner = RunSimulation(childNode, MyPlayerNumber);

                    // 4) Backpropagation
                    // use the result of 3 to update the nodes from the end to the root
                    BackPropagate(childNode, winner);
                }

            }

            // Finally, now that we're out of time, grab the best one
            var bestNode = RootNode.GetBestChild();
            CurrentBestMove = new Tuple<int, int>(bestNode.Row, bestNode.Column);
        }

        private void BackPropagate(Node node, int winner)
        {
            var iWon = winner == MyPlayerNumber;
            node.NumberOfSimulations += 1;

            var parent = node;
 
            while (parent != null)
            {
                parent.NumberOfSimulations += 1;
                if (iWon)
                {
                    parent.Score += 1;
                }

                parent = parent.Parent;
            }
        }

        private int RunSimulation(Node node, int playerNumber)
        {
            while (!IsFinalMove(node.Board))
            {
                var move = GetRandomMoveFrom(node.Board);
                node.UpdateBoard(move, playerNumber);
                playerNumber *= -1;
            }

            return playerNumber;

        }

        private bool IsFinalMove(Matrix<int> board)
        {
            return false;
        }

        private Tuple<int,int,int> GetRandomMoveFrom(Matrix<int> board)
        {
            return board
                .EnumerateIndexed(Zeros.Include)
                .Where(x => x.Item3 == 0)
                .OrderBy(x => Guid.NewGuid())
                .FirstOrDefault();
        }

        private Node GetRandomChildNode(Matrix<int> board, int playerNumber)
        {
            var move = GetRandomMoveFrom(board);
            var node = new Node(board);
            node.Board[move.Item1, move.Item2] = playerNumber;
            node.Row = move.Item1;
            node.Column = move.Item2;
            return node;
        }

        private Node GetBestNode(Node root)
        {
            var node = root;
            var children = root.Children;
            while (children.Count > 0)
            {
                node = children.OrderByDescending(x => x.UpperConfidenceValue()).FirstOrDefault();
                children = node.Children;
            }

            return node;
        }

        private bool HexIsEmpty(Tuple<int, int> hex)
        {
            return Board[hex.Item1, hex.Item2] == 0;
        }

        private Tuple<int, int> CheckBestMove()
        {
            if (CurrentBestMove != null && HexIsEmpty(CurrentBestMove))
            {
              
                Quip("Um, for some reason I like the idea of taking an already taken spot " + CurrentBestMove + " (" + RandomMovesMade + " Random moves made)");
                CurrentBestMove = null;
            }
            if (CurrentBestMove == null)
            {
                var move = GetRandomMoveFrom(Board);

                Board[move.Item1, move.Item2] = MyPlayerNumber;
                CurrentBestMove = new Tuple<int, int>(move.Item1, move.Item2);
     
                RandomMovesMade++;
                Quip("Had to pick randomly.");
            }

            return CurrentBestMove;
        }

        private void Setup(Config playerConfig)
        {
            NumberOfSecondsForSearch = GetDefault(playerConfig, "maxSearchTime", 60);
        }

        public override string CodeName()
        {
            return "Melece";
        }
    }
}
