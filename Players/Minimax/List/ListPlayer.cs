using System;
using System.Collections.Generic;
using System.Linq;
using Players.Base;
using Players.Common;
using Players.Minimax.List;

namespace Players.Minimax.List
{
    public class ListPlayer : MinimaxPlayer
    {
        private new ListMap _memory;
        private int Size;
        private int _maxLevels;
        private int _maxSeconds;
        private int _nodesChecked;
        private PlayerType Opponent => Me == Common.PlayerType.Blue  ? Common.PlayerType.Red : Common.PlayerType.Blue;



        public ListPlayer(int playerNumber, int boardSize, Config playerConfig) : base(playerNumber, boardSize, playerConfig)
        {
            PlayerNumber = playerNumber;
            Me = PlayerNumber == 1 ? Common.PlayerType.Blue : Common.PlayerType.Red;
            Size = boardSize;
            _maxLevels = GetDefault(playerConfig, "maxLevels", 20);
            Name = playerConfig.name;

            Startup();
        }

        public override string PlayerType()
        {
            return "MiniMax AI (" + _maxLevels + ")";
        }

        public void Startup()
        {
            _memory = new ListMap(Size);
        }

        public override Tuple<int, int> SelectHex(Tuple<int, int> opponentMove)
        {
            _nodesChecked = 0;
            if (opponentMove != null)
            {
                // First we set the opponent's hex as being owned by them.
                var opponentHex = _memory.Board.FirstOrDefault(x => x.Row == opponentMove.Item1 && x.Column == opponentMove.Item2);
                if (opponentHex != null)
                {
                    _memory.TakeHex(Opponent, opponentMove.Item1, opponentMove.Item2);
                }
            }
            ListNode choice = null;
            int bestScore = 999;

            var possibleMoves = _memory.Board.OrderBy(x => x.RandomValue).Where(x => x.Owner == Common.PlayerType.White);

            foreach (var move in possibleMoves)
            {
                var thoughtBoard = new ListMap(_memory);

                var scoreForThisMove = LetMeThinkAboutIt(thoughtBoard, Me, _maxLevels, 0, 0);
                if (scoreForThisMove < bestScore)
                {
                    bestScore = scoreForThisMove;
                    choice = _memory.Board.FirstOrDefault(x => x.Row == move.Row && x.Column == move.Column);
                }
            }

            // And when in doubt, get a random one
            if (choice == null)
            {
                Quip("Random it is...");
                choice = _memory.Board.OrderBy(x => x.RandomValue)
                    .FirstOrDefault(x => x.Owner == Common.PlayerType.White);
            }

            _memory.TakeHex(Me, choice.Row, choice.Column);
            return new Tuple<int, int>(choice.Row, choice.Column);
        }

        private int ScoreFromPath(List<ListNode> path)
        {
            // Start at the top or left, use A* to get the best path to the opposite
            // side.  The score for that player is the amount of unclaimed places in
            // the best path
            if (path == null || ! path.Any())
            {
                return 100;
            }
            return path.Count(x => x.Owner == Common.PlayerType.White);
        }

        private int ScoreFromBoard(Common.PlayerType player, ListMap board)
        {
            // Get the player's best path
            var playerPath = new List<ListNode>();
            // Get the opponent best path
            var opponentPath = new List<ListNode>();


            return ScoreFromPath(playerPath) - ScoreFromPath(opponentPath);
        }

        public List<ListNode> GetBestPath(Common.PlayerType player, ListMap board)
        {
            ListNode start;
            ListNode end;
            if (player == Common.PlayerType.Blue)
            {
                start = board.Top;
                end = board.Bottom;
            } else
            {
                start = board.Left;
                end = board.Right;
            }

            start.Status = Status.Open;

            return LookForPath(board.Board, player, end);

        }

        private List<ListNode> LookForPath(List<ListNode> board, PlayerType player, ListNode end)
        {
            ListNode bestLookingNode = null;
            Quip("Looking...");

            // GEt the best looking node
            bestLookingNode = board
                .OrderBy(x => x.F)
                .ThenBy(x => x.RandomValue)
                .FirstOrDefault(z => z.Status == Status.Open);

            if (bestLookingNode == null)
            {
                return null;
            }

            // CLOSE IT
            bestLookingNode.Status = Status.Closed;

            if (bestLookingNode == end)
            {
                var preferredPath = new List<ListNode>();
                var parent = bestLookingNode;
                while (parent != null)
                {
                    preferredPath.Add(parent);
                    parent = parent.Parent;
                }

                return preferredPath;
            }

            var neighbours = bestLookingNode.Neighbours;

            foreach (var node in neighbours)
            {
                if (node.Owner == bestLookingNode.Owner || node.Owner == Common.PlayerType.White)
                {
                    if (node.Status == Status.Open)
                    {
                        if (node.G > bestLookingNode.G + (node.Owner == player ? 0 : 1))
                        {
                            node.Parent = bestLookingNode;
                            node.G = bestLookingNode.G + (node.Owner == player ? 1 : 0); ;
                            node.H = (player == Common.PlayerType.Red ? _size - 1 - node.Column : _size - 1 - node.Row) * 1;
                        }
                    }
                    else if (node.Status == Status.Untested)
                    {
                        node.Status = Status.Open;
                        node.Parent = bestLookingNode;
                        node.G = bestLookingNode.G + (node.Owner == player ? 0 : 1);
                        node.H = (player == Common.PlayerType.Red ? _size - 1 - node.Column : _size - 1 - node.Row) * 1;
                    }
                }


            }
            return LookForPath(board, player, end);

        }


        //private int ScoreFromBoardOld(Common.PlayerType player, ListMap board)
        //{
        //    // To score the board, we should find the best path for each player
        //    // and use them to determine the score.
        //    //
        //    // Any path with fewer hexes needed to get to an edge, for instance, is better
        //    if (board.Board.Count(x => x.Owner != Common.PlayerType.White) > 2)
        //    {

        //        var opponent = player == Common.PlayerType.Blue ? Common.PlayerType.Red : Common.PlayerType.Blue;

        //        var playerScore = board.Board.Where(x => x.Owner == player)
        //            .OrderBy(y => y.RemainingDistance())
        //            .FirstOrDefault();

        //        var opponentScore = board.Board.Where(x => x.Owner == opponent)
        //            .OrderBy(y => y.RemainingDistance())
        //            .FirstOrDefault();

        //        var finalScore = playerScore.RemainingDistance() - opponentScore.RemainingDistance();

        //        return finalScore;
        //    }

        //    return 0;

        //}

        private int LetMeThinkAboutIt(ListMap thoughtBoard, Common.PlayerType player, int depth, int alpha, int beta)
        {
            var currentAlpha = alpha;
            var currentBeta = beta;

            if (depth == 0 || thoughtBoard.Board.All(x => x.Owner != Common.PlayerType.White))
            {
                return ScoreFromBoard(player, thoughtBoard);
            }

            var possibleMoves = thoughtBoard.Board.Where(x => x.Owner == Common.PlayerType.White);
            // Get possible moves for player
            if (possibleMoves.Any())
            {
                var newThoughtBoard = new ListMap(thoughtBoard);
                if (player == Me)
                {
                    var bestValue = -999999;
                    foreach (var move in possibleMoves)
                    {

                        newThoughtBoard.TakeHex(player, move.Row, move.Column);
                        bestValue = Math.Max(bestValue, LetMeThinkAboutIt(newThoughtBoard, Opponent, depth - 1, currentAlpha, currentBeta));
                        currentAlpha = Math.Max(currentAlpha, bestValue);
                        if (currentBeta <= currentAlpha)
                        {
                            _nodesChecked++;
                            break;
                        }
                    }

                    return bestValue;
                }
                else
                {
                    var bestValue = 999999;
                    foreach (var move in possibleMoves)
                    {

                        newThoughtBoard.TakeHex(player, move.Row, move.Column);
                        bestValue = Math.Min(bestValue, LetMeThinkAboutIt(newThoughtBoard, Me, depth - 1, currentAlpha, currentBeta));
                        currentBeta = Math.Min(currentBeta, bestValue);
                        if (currentBeta <= currentAlpha)
                        {
                            _nodesChecked++;
                            break;
                        }
                    }

                    return bestValue;
                }
            }
            else
            {
                return ScoreFromBoard(player, thoughtBoard);
            }
        }

    }
}