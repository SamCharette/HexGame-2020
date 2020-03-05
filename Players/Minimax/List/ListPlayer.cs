using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Players.Common;


namespace Players.Minimax.List
{
    public class ListPlayer : MinimaxPlayer
    {
        private const int AbsoluteWorst = -9999;
        private const int AbsoluteBest = 9999;
        public ListMap Memory { get; set; }
        public int MaxLevels { get; set; }
        public int CostToMoveToClaimedNode { get; set; }
        public int CostToMoveToUnclaimedNode { get; set; }
        public int CostPerNodeTillEnd { get; set; }
        public int RegularMoveNumber { get; set; }
        public int RandomMoveNumber { get;set;}
        public int PrunesMade { get; set; }
        public int NodesChecked { get; set; }
        private Tuple<int,int> bestMove = new Tuple<int, int>(0,0);

        public PlayerType Opponent => Me == Common.PlayerType.Blue  ? Common.PlayerType.Red : Common.PlayerType.Blue;



        public ListPlayer(int playerNumber, int boardSize, Config playerConfig) : base(playerNumber, boardSize, playerConfig)
        {
            PlayerNumber = playerNumber;
            Me = PlayerNumber == 1 ? Common.PlayerType.Blue : Common.PlayerType.Red;
            Size = boardSize;
            MaxLevels = GetDefault(playerConfig, "maxLevels", 20);
            CostPerNodeTillEnd = GetDefault(playerConfig, "costPerNodeTillEnd", 1000);
            CostToMoveToUnclaimedNode = GetDefault(playerConfig, "costToMoveToUnclaimedNode", 100);
            CostToMoveToClaimedNode = GetDefault(playerConfig, "costToMoveToClaimedNode", 0);
            talkative = Convert.ToInt32(playerConfig.talkative);
            RegularMoveNumber = 0;
            RandomMoveNumber = 0;
            PrunesMade = 0;
            Name = playerConfig.name;
            NodesChecked = 0;
            Startup();
        }

        public override string PlayerType()
        {
            return "MiniMax AI (" + MaxLevels + ")";
        }

        public void Startup()
        {
            Memory = new ListMap(Size);
        }

        public override void GameOver(int winningPlayerNumber)
        {
            Memory = null;
            Quip("Game over.  " + RegularMoveNumber + " regular moves and " + RandomMoveNumber + " random ones.");
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

            PrunesMade = 0;

            int bestScore = AbsoluteBest;

            ListNode choice = null;
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var possibleMoves = PossibleMoves(Memory, true);

            if (possibleMoves != null)
            {
                RegularMoveNumber++;

                choice = possibleMoves.FirstOrDefault();

                foreach (var move in possibleMoves)
                {
                    NodesChecked = 0;
                    Memory.TakeHex(Me, move.Row, move.Column);
                    var scoreForThisMove = LetMeThinkAboutIt(Memory, MaxLevels, AbsoluteBest, AbsoluteWorst, false);
                    if (scoreForThisMove > bestScore)
                    {
                        bestScore = scoreForThisMove;
                        choice = Memory.Board.FirstOrDefault(x => x.Row == move.Row && x.Column == move.Column);
                    }
                    Memory.ReleaseHex(move.Row, move.Column);
                    //Quip("1 potential move with " + NodesChecked + " nodes checked in " + watch.ElapsedMilliseconds + " milliseconds");
                }
            }

            var score = LetMeThinkAboutIt(Memory, MaxLevels, AbsoluteBest, AbsoluteWorst, true);
            Quip( "Best score found was " + score);
            watch.Stop();

            // And when in doubt, get a random one
            if (choice == null)
            {
                RandomMoveNumber++;

                choice = Memory.Board.OrderBy(x => x.RandomValue)
                    .FirstOrDefault(x => x.Owner == Common.PlayerType.White);
                Quip("Random choice : [" + choice.Row + ", " + choice.Column + "] #" + RandomMoveNumber);
            }
            else
            {
                Quip("Choosing: [" + choice.Row + ", " + choice.Column + "] normal move #" + RegularMoveNumber + " (" + watch.ElapsedMilliseconds + "ms) with " + PrunesMade + " prunes made and score was " + bestScore);
            }

            Memory.TakeHex(Me, choice.Row, choice.Column);

            return new Tuple<int, int>(choice.Row, choice.Column);
        }

        private int LetMeThinkAboutIt(ListMap thoughtBoard, int depth, int alpha, int beta, bool isMaximizing)
        {
  

            if (depth == 0 || thoughtBoard.Board.All(x => x.Owner != Common.PlayerType.White))
            {
                return ScoreFromBoard(isMaximizing, thoughtBoard);
            }

            var possibleMoves = PossibleMoves(thoughtBoard, isMaximizing);
            
            if (possibleMoves.Any())
            {

                if (isMaximizing)
                {
                    var bestValue = AbsoluteWorst;

                    foreach (var move in possibleMoves)
                    {
                        thoughtBoard.TakeHex(CurrentlySearchingAs(true), move.Row, move.Column);
                        bestValue = Math.Max(bestValue, LetMeThinkAboutIt(thoughtBoard, depth - 1, alpha, beta, false));
                        alpha = Math.Max(alpha, bestValue);
                        NodesChecked++;
                        thoughtBoard.ReleaseHex(move.Row, move.Column);
                        if (beta <= alpha)
                        {
                            PrunesMade++;
                            break;
                        }

                    }

                    return bestValue;
                }
                else
                {
                    var bestValue = AbsoluteBest;
                    foreach (var move in possibleMoves)
                    {

                        thoughtBoard.TakeHex(CurrentlySearchingAs(false), move.Row, move.Column);
                        bestValue = Math.Min(bestValue, LetMeThinkAboutIt(thoughtBoard, depth - 1, alpha, beta, true));
                        beta = Math.Min(beta, bestValue);
                        NodesChecked++;
                        thoughtBoard.ReleaseHex(move.Row, move.Column);
                        if (beta <= alpha)
                        {
                            PrunesMade++;
                            break;
                        }
                    }

                    return bestValue;
                }
            }
            else
            {
                return ScoreFromBoard(isMaximizing, thoughtBoard);
            }
        }

        private List<ListNode> PossibleMoves(ListMap board, bool isMaximizing)
        {
            // The possible moves are generated by better looking in our path,
            // followed by the opponent path, followed then by open hexes on no path
            var myBestPathFromHere = StartLookingForBestPath(isMaximizing, board);
             var possibleMoves = new List<ListNode>();
            if (myBestPathFromHere != null)
                possibleMoves
                    .AddRange(myBestPathFromHere
                        .OrderBy(x => x.LookAtMe)
                        .ThenBy(x => x.F)
                        .ThenBy(x => x.RandomValue)
                        .Where(x => x.Owner == Common.PlayerType.White));
          
            //var opponentBestPathFromHere = StartLookingForBestPath(!isMaximizing, board);

            //if (opponentBestPathFromHere != null)
            //    possibleMoves
            //        .AddRange(opponentBestPathFromHere
            //            .OrderBy(x => x.LookAtMe)
            //            .ThenBy(x => x.RandomValue)
            //            .Where(x => x.Owner == Common.PlayerType.White));
            //possibleMoves.AddRange(Memory.Board
            //    .OrderBy(x => x.RandomValue)
            //    .Where(x => x.Owner == Common.PlayerType.White && possibleMoves.All(y => y != x)));
            return possibleMoves;
        }

        private int ScoreFromPath(List<ListNode> path, bool isMaximizing)
        {
            if (path == null || ! path.Any())
            {
                return isMaximizing ? AbsoluteWorst : AbsoluteBest;
            }
            
            return Size - path.Count(x => x.Owner == Common.PlayerType.White);
        }

        private int ScoreFromBoard(bool isMaximizing, ListMap board)
        {
            // Get the player's best path
            var playerPath = StartLookingForBestPath(isMaximizing, board);
            var playerScore = ScoreFromPath(playerPath, isMaximizing);
            
            // Get the opponent best path
            var opponentPath = StartLookingForBestPath(!isMaximizing, board);
            var opponentScore = ScoreFromPath(opponentPath, !isMaximizing);

            var score = playerScore - opponentScore;
            var hex = playerPath?.FirstOrDefault(x => x.Owner == Common.PlayerType.White);
            bestMove = new Tuple<int, int>(hex?.Row ?? 0, hex?.Column ?? 0);
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
                        
                        if (node.G > bestLookingNode.G + (node.Owner == Common.PlayerType.White ? CostToMoveToUnclaimedNode : CostToMoveToClaimedNode ))
                        { 
                            node.Parent = bestLookingNode;
                            node.G = bestLookingNode.G + (node.Owner == Common.PlayerType.White ? CostToMoveToUnclaimedNode : CostToMoveToClaimedNode); 
                            node.H = (CurrentlySearchingAs(isMaximizing) == Common.PlayerType.Red ? _size - 1 - node.Column : _size - 1 - node.Row) * CostPerNodeTillEnd;
                        }
                    }
                    else if (node.Status == Status.Untested)
                    {
                        node.Status = Status.Open;
                        node.Parent = bestLookingNode;
                        node.G = bestLookingNode.G + (node.Owner == Common.PlayerType.White ? CostToMoveToUnclaimedNode : CostToMoveToClaimedNode);
                        node.H = (CurrentlySearchingAs(isMaximizing) == Common.PlayerType.Red ? _size - 1 - node.Column : _size - 1 - node.Row) * CostPerNodeTillEnd;
                    }
                }


            }
            return ContinueLookingForPath(map, isMaximizing, start, end);

        }

    

    }
}