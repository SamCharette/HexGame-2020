using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MathNet.Numerics.Providers.Common.OpenBlas;
using Players.Base;
using Players.Common;
using Players.Minimax.List;

namespace Players.Minimax.List
{
    public class ListPlayer : MinimaxPlayer
    {
        public new ListMap Memory;
        private int Size;
        private int _maxLevels;
        private int _maxSeconds;
        private int _nodesChecked;
        private int costPerClaimedNode = 1;
        private int costPerUnclaimedNode = 50;
        private int costPerNodeTillEnd = 100;
        private int worstImpossibleScore = -9999;
        private int bestImpossibleScore = 9999;

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
            Memory = new ListMap(Size);
        }

        public override Tuple<int, int> SelectHex(Tuple<int, int> opponentMove)
        {
            if (opponentMove != null)
            {
                // First we set the opponent's hex as being owned by them.
                var opponentHex = Memory.Board.FirstOrDefault(x => x.Row == opponentMove.Item1 && x.Column == opponentMove.Item2);
                if (opponentHex != null)
                {
                    Memory.TakeHex(Opponent, opponentMove.Item1, opponentMove.Item2);
                }
            }

            
            var myBestPathFromHere = StartLookingForBestPath(Me, Memory);
            //var opponentBestPathFromHere = StartLookingForBestPath(Opponent, _memory);
            ListNode choice = null;

            if (myBestPathFromHere != null)// && opponentBestPathFromHere != null)
            {
                var possibleMoves = new List<ListNode>();
                possibleMoves.AddRange(myBestPathFromHere.Where(x => x.Owner == Common.PlayerType.White));
                //possibleMoves.AddRange(opponentBestPathFromHere.Where(x => x.Owner == Common.PlayerType.White));

                _nodesChecked = 0;
                int bestScore = worstImpossibleScore;

                foreach (var move in possibleMoves)
                {
                    var watch = System.Diagnostics.Stopwatch.StartNew();
                    var thoughtBoard = Memory;//new ListMap(_memory);

                    var scoreForThisMove = LetMeThinkAboutIt(thoughtBoard, Me, _maxLevels, -9999, 9999);
                    if (scoreForThisMove > bestScore)
                    {
                        bestScore = scoreForThisMove;
                        choice = thoughtBoard.Board.FirstOrDefault(x => x.Row == move.Row && x.Column == move.Column);
                    }

                    watch.Stop();
                    Quip("1 potential move with " + _nodesChecked + " nodes checked in " + watch.ElapsedMilliseconds + " milliseconds");
                }
            }

            // And when in doubt, get a random one
            if (choice == null)
            {
                Quip("Random it is...");
                choice = Memory.Board.OrderBy(x => x.RandomValue)
                    .FirstOrDefault(x => x.Owner == Common.PlayerType.White);
            }

            Memory.TakeHex(Me, choice.Row, choice.Column);
            return new Tuple<int, int>(choice.Row, choice.Column);
        }

        private int ScoreFromPath(List<ListNode> path)
        {
            if (path == null || ! path.Any())
            {
                return worstImpossibleScore;
            }
            return path.Count(x => x.Owner == Common.PlayerType.White);
        }

        private int ScoreFromBoard(Common.PlayerType player, ListMap board)
        {
            var opponent = player == Common.PlayerType.Blue ? Common.PlayerType.Red : Common.PlayerType.Blue;
            // Get the player's best path
            var playerPath = StartLookingForBestPath(player, board);
            // Get the opponent best path
            var opponentPath = StartLookingForBestPath(opponent, board);

            var score = ScoreFromPath(opponentPath) - ScoreFromPath(playerPath);
            board.ClearPathValues();
            return score;
        }

        private int ScoreFromBoard(Common.PlayerType player, ListMap board, List<ListNode> playerPath, List<ListNode> opponentPath)
        {
            var score =  ScoreFromPath(opponentPath) - ScoreFromPath(playerPath);
            board.ClearPathValues();
            return score;
        }

        public List<ListNode> StartLookingForBestPath(Common.PlayerType player, ListMap board)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
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

            var path = ContinueLookingForPath(board, player, start, end);
            watch.Stop();
            //if (path != null && path.Any())
            //{
            //    Quip("Path found in " + watch.ElapsedMilliseconds + " milliseconds.");
            //}
            //else
            //{
            //    Quip("Path wasn't found in " + watch.ElapsedMilliseconds + " milliseconds");
            //}

            return path;
        }

        private List<ListNode> ContinueLookingForPath(ListMap map, PlayerType player, ListNode start, ListNode end)
        {
            ListNode bestLookingNode = null;


            if (map.Board.All(x => x.Status == Status.Untested))
            {
                start.Status = Status.Open;
                bestLookingNode = start;
            }
            else
            {

                bestLookingNode = map.Board
                    .OrderBy(x => x.F)
                    .ThenBy(x => x.RandomValue)
                    .FirstOrDefault(x => x.Status == Status.Open);
            }

            if (bestLookingNode == null )
            {
                return null;
            }

            // CLOSE IT
            bestLookingNode.Status = Status.Closed;

            if (bestLookingNode.IsNeighboursWith(end))
            {
                var preferredPath = new List<ListNode>();
                var parent = bestLookingNode;
                while (parent != null && (parent.Row != start.Row && parent.Column != start.Column))
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
                        
                        if (node.G > bestLookingNode.G + (node.Owner == player ? costPerClaimedNode : costPerUnclaimedNode))
                        {
                            node.Parent = bestLookingNode;
                            node.G = bestLookingNode.G + (node.Owner == player ? costPerUnclaimedNode : costPerClaimedNode); 
                            node.H = (player == Common.PlayerType.Red ? _size - 1 - node.Column : _size - 1 - node.Row) * costPerNodeTillEnd;
                        }
                    }
                    else if (node.Status == Status.Untested)
                    {
                        node.Status = Status.Open;
                        node.Parent = bestLookingNode;
                        node.G = bestLookingNode.G + (node.Owner == player ? costPerClaimedNode : costPerUnclaimedNode);
                        node.H = (player == Common.PlayerType.Red ? _size - 1 - node.Column : _size - 1 - node.Row) * costPerNodeTillEnd;
                    }
                }


            }
            return ContinueLookingForPath(map, player, start, end);

        }

        private int LetMeThinkAboutIt(ListMap thoughtBoard, Common.PlayerType player, int depth, int alpha, int beta)
        {
            var currentAlpha = alpha;
            var currentBeta = beta;
            
            if (depth == 0 || thoughtBoard.Board.All(x => x.Owner != Common.PlayerType.White))
            {
                return ScoreFromBoard(player, thoughtBoard);
            }

            var newThoughtBoard = thoughtBoard;

            var myBestPathFromHere = StartLookingForBestPath(Me, newThoughtBoard);
            //var opponentBestPathFromHere = StartLookingForBestPath(Opponent, newThoughtBoard);


            var possibleMoves = new List<ListNode>();
            if (myBestPathFromHere != null)
            {
                possibleMoves.AddRange(myBestPathFromHere.OrderByDescending(x => x.RandomValue).Where(x => x.Owner == Common.PlayerType.White));
            }
            //if (opponentBestPathFromHere != null)
            //{
                //possibleMoves.AddRange(opponentBestPathFromHere.Where(x => x.Owner == Common.PlayerType.White));
            //}
            // Get possible moves for player
            if (possibleMoves.Any())
            {
                
                if (player == Me)
                {
                    var bestValue = worstImpossibleScore;

                    foreach (var move in possibleMoves)
                    {
                        newThoughtBoard.TakeHex(player, move.Row, move.Column);
                        bestValue = Math.Max(bestValue, LetMeThinkAboutIt(newThoughtBoard, Opponent, depth - 1, currentAlpha, currentBeta));
                        currentAlpha = Math.Max(currentAlpha, bestValue);
                        newThoughtBoard.ReleaseHex(move.Row, move.Column);
                        _nodesChecked++;
                        if (currentBeta <= currentAlpha)
                        {
                            break;
                        }
                    }
                 
                    return bestValue;
                }
                else
                {
                    var bestValue = bestImpossibleScore;
                    foreach (var move in possibleMoves)
                    {

                        newThoughtBoard.TakeHex(player, move.Row, move.Column);
                        bestValue = Math.Min(bestValue, LetMeThinkAboutIt(newThoughtBoard, Me, depth - 1, currentAlpha, currentBeta));
                        currentBeta = Math.Min(currentBeta, bestValue);
                        newThoughtBoard.ReleaseHex(move.Row, move.Column);
                        _nodesChecked++;
                        if (currentBeta <= currentAlpha)
                        {
                            break;
                        }
                    }

                    return bestValue;
                }
            }
            else
            {
                return ScoreFromBoard(player, newThoughtBoard);
            }
        }

    }
}