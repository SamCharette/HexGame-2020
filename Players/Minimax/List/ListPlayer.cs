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
        private int worstImpossibleScore = 9999;
        private int bestImpossibleScore = -9999;
        private int regularMoveNumber = 0;
        private int randomMoveNumber = 0;
        private int prunesMade = 0;

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

        public override void GameOver(int winningPlayerNumber)
        {
            Memory = null;
            Quip("Game over.  " + regularMoveNumber + " regular moves and " + randomMoveNumber + " random ones.");
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

            prunesMade = 0;

            int bestScore = worstImpossibleScore;

            var myBestPathFromHere = StartLookingForBestPath(true, Memory);
            var opponentBestPathFromHere = StartLookingForBestPath(false, Memory);
            ListNode choice = null;
            var watch = System.Diagnostics.Stopwatch.StartNew();

            if (myBestPathFromHere != null || opponentBestPathFromHere != null)
            {
                regularMoveNumber++;
                var possibleMoves = new List<ListNode>();
                if (myBestPathFromHere != null) possibleMoves.AddRange(myBestPathFromHere.Where(x => x.Owner == Common.PlayerType.White));
                if (opponentBestPathFromHere != null) possibleMoves.AddRange(opponentBestPathFromHere.Where(x => x.Owner == Common.PlayerType.White));

                choice =  possibleMoves
                    .OrderByDescending(x => x.LookAtMe)
                    .ThenByDescending(x => x.RandomValue)
                    .FirstOrDefault(x => x.Owner == Common.PlayerType.White);
             
                foreach (var move in possibleMoves)
                {
                    _nodesChecked = 0;
                    var thoughtBoard = new ListMap(Memory);

                    var scoreForThisMove = LetMeThinkAboutIt(thoughtBoard, _maxLevels, bestImpossibleScore, worstImpossibleScore, true);
                    if (scoreForThisMove > bestScore)
                    {
                        bestScore = scoreForThisMove;
                        choice = thoughtBoard.Board.FirstOrDefault(x => x.Row == move.Row && x.Column == move.Column);
                    }

                    //Quip("1 potential move with " + _nodesChecked + " nodes checked in " + watch.ElapsedMilliseconds + " milliseconds");
                }
            }
            watch.Stop();

            // And when in doubt, get a random one
            if (choice == null)
            {
                randomMoveNumber++;

                choice = Memory.Board.OrderBy(x => x.RandomValue)
                    .FirstOrDefault(x => x.Owner == Common.PlayerType.White);
                Quip("Random choice : [" + choice.Row + ", " + choice.Column + "] #" + randomMoveNumber);
            }
            else
            {
                Quip("Choosing: [" + choice.Row + ", " + choice.Column + "] normal move #" + regularMoveNumber + " (" + watch.ElapsedMilliseconds + "ms) with " + prunesMade + " prunes made and score was " + bestScore);
            }

            Memory.TakeHex(Me, choice.Row, choice.Column);

            return new Tuple<int, int>(choice.Row, choice.Column);
        }

        private int ScoreFromPath(List<ListNode> path, bool isMaximizing)
        {
            if (path == null || ! path.Any())
            {
                if (isMaximizing)
                {
                    Quip("Dafuq?");
                    return worstImpossibleScore;
                }
                else
                {
                    Quip("Doubledafuq?");
                    return bestImpossibleScore;
                }

            }
            return path.Count(x => x.Owner == Common.PlayerType.White);
        }

        private int ScoreFromBoard(bool isMaximizing, ListMap board)
        {
            // Get the player's best path
            var playerPath = StartLookingForBestPath(isMaximizing, board);
            var playerScore = ScoreFromPath(playerPath, true);
            
            // Get the opponent best path
            var opponentPath = StartLookingForBestPath(!isMaximizing, board);
            var opponentScore = ScoreFromPath(opponentPath, false);

            var score = playerScore - opponentScore;
            return score;
        }


        public List<ListNode> StartLookingForBestPath(bool isMaximizing, ListMap board)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            ListNode start;
            ListNode end;
            if (isMaximizing)
            {
                if (Me == Common.PlayerType.Blue)
                {
                    start = board.Top;
                    end = board.Bottom;
                }
                else
                {
                    start = board.Left;
                    end = board.Right;
                }
            }
            else
            {
                if (Me == Common.PlayerType.Blue)
                {
                    start = board.Left;
                    end = board.Right;
                }
                else
                {
                    start = board.Top;
                    end = board.Bottom;
                }
            }
     

            start.Status = Status.Open;
            board.ClearPathValues();
            var path = ContinueLookingForPath(board, isMaximizing, start, end);
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

        private PlayerType CurrentlySearchingAs(bool isMaximizing)
        {
            // if we're maximizing, it's this player
            return isMaximizing ? Me : Opponent;
        }

        private List<ListNode> ContinueLookingForPath(ListMap map, bool isMaximizing, ListNode start, ListNode end)
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
                    .OrderByDescending(x => x.LookAtMe)
                    .ThenBy(x => x.F)
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
                        
                        if (node.G > bestLookingNode.G + (node.Owner == CurrentlySearchingAs(isMaximizing) ? costPerClaimedNode : costPerUnclaimedNode))
                        {
                            node.Parent = bestLookingNode;
                            node.G = bestLookingNode.G + (node.Owner == CurrentlySearchingAs(isMaximizing) ? costPerClaimedNode : costPerUnclaimedNode); 
                            node.H = (CurrentlySearchingAs(isMaximizing) == Common.PlayerType.Red ? _size - 1 - node.Column : _size - 1 - node.Row) * costPerNodeTillEnd;
                        }
                    }
                    else if (node.Status == Status.Untested)
                    {
                        node.Status = Status.Open;
                        node.Parent = bestLookingNode;
                        node.G = bestLookingNode.G + (node.Owner == CurrentlySearchingAs(isMaximizing) ? costPerClaimedNode : costPerUnclaimedNode);
                        node.H = (CurrentlySearchingAs(isMaximizing) == Common.PlayerType.Red ? _size - 1 - node.Column : _size - 1 - node.Row) * costPerNodeTillEnd;
                    }
                }


            }
            return ContinueLookingForPath(map, isMaximizing, start, end);

        }

        private int LetMeThinkAboutIt(ListMap thoughtBoard, int depth, int alpha, int beta, bool isMaximizing)
        {
            var currentAlpha = alpha;
            var currentBeta = beta;
            
            if (depth == 0 || thoughtBoard.Board.All(x => x.Owner != Common.PlayerType.White))
            {
                return ScoreFromBoard(isMaximizing, thoughtBoard);
            }

            var newThoughtBoard = new ListMap(thoughtBoard);
            var playerBestPathFromHere = StartLookingForBestPath(isMaximizing, newThoughtBoard);
            var opponentBestPathFromHere = StartLookingForBestPath(!isMaximizing, newThoughtBoard);


            var possibleMoves = new List<ListNode>();
            if (playerBestPathFromHere != null)
            {
                possibleMoves.AddRange(playerBestPathFromHere
                    .OrderByDescending(x => x.LookAtMe)
                    .ThenByDescending(x => x.RandomValue)
                    .Where(x => x.Owner == Common.PlayerType.White));
            }
            if (opponentBestPathFromHere != null)
            {
                possibleMoves.AddRange(opponentBestPathFromHere
                    .OrderByDescending(x => x.LookAtMe)
                    .ThenByDescending(x => x.RandomValue)
                    .Where(x => x.Owner == Common.PlayerType.White));
            }
            // Get possible moves for player
            if (possibleMoves.Any())
            {
                
                if (isMaximizing)
                {
                    var bestValue = worstImpossibleScore;

                    foreach (var move in possibleMoves)
                    {
                        newThoughtBoard.TakeHex(CurrentlySearchingAs(true), move.Row, move.Column);
                        bestValue = Math.Max(bestValue, LetMeThinkAboutIt(newThoughtBoard, depth - 1, currentAlpha, currentBeta, false));
                        currentAlpha = Math.Max(currentAlpha, bestValue);
                        _nodesChecked++;
                        if (currentBeta <= currentAlpha)
                        {
                            prunesMade++;
                            break;
                        }
                        newThoughtBoard.ReleaseHex(move.Row, move.Column);

                    }

                    return bestValue;
                }
                else
                {
                    var bestValue = bestImpossibleScore;
                    foreach (var move in possibleMoves)
                    {

                        newThoughtBoard.TakeHex(CurrentlySearchingAs(false), move.Row, move.Column);
                        bestValue = Math.Min(bestValue, LetMeThinkAboutIt(newThoughtBoard, depth - 1, currentAlpha, currentBeta, true));
                        currentBeta = Math.Min(currentBeta, bestValue);
                        _nodesChecked++;
                        if (currentBeta <= currentAlpha)
                        {
                            prunesMade++;
                            break;
                        }
                        newThoughtBoard.ReleaseHex(move.Row, move.Column);
                    }

                    return bestValue;
                }
            }
            else
            {
                return ScoreFromBoard(isMaximizing, newThoughtBoard);
            }
        }

    }
}