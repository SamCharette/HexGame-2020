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
        private const string MovesExamined = "Total Moves Examined";
        private const string MovesExaminedThisTurn = "Moves Examined This Turn";
        private const string CurrentScore = "Current Score";
        private const string AverageTimeToDecision = "Average time to decision";
        private const string TotalTimeThinking = "Total time thinking";
        private const string NumberOfRandomMoves = "# of random moves";
        private const string NumberOfPlannedMoves = "# of planned moves";
        private const string NumberOfNodesChecked = "Nodes Checked";
        private const string NumberOfPrunesMade = "Prunes Made";
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
        public List<ListNode> PossibleMovesToMake { get; set; }

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
            Monitors.Add(MovesExamined, 0);
            Monitors.Add(MovesExaminedThisTurn, 0);
            Monitors.Add(CurrentScore, 0);
            Monitors.Add(AverageTimeToDecision, 0);
            Monitors.Add(TotalTimeThinking, 0);
            Monitors.Add(NumberOfPlannedMoves, 0);
            Monitors.Add(NumberOfRandomMoves, 0);
            Monitors.Add(NumberOfNodesChecked, 0);
            Monitors.Add(NumberOfPrunesMade, 0);

            Startup();
        }

        public override string PlayerType()
        {
            return "MiniMax AI (" + MaxLevels + ")";
        }

        public void Startup()
        {
            Memory = new ListMap(Size);
            RelayPerformanceInformation();
        }

        public override void GameOver(int winningPlayerNumber)
        {
            RelayPerformanceInformation();
            Memory = null;
            Quip("Game over.  " + RegularMoveNumber + " regular moves and " + RandomMoveNumber + " random ones.");
        }

        public override Tuple<int, int> SelectHex(Tuple<int, int> opponentMove)
        {
            Monitors[MovesExaminedThisTurn] = 0;
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
            
            int bestScore = AbsoluteWorst;

            ListNode choice = null;
            var watch = System.Diagnostics.Stopwatch.StartNew();
            PossibleMovesToMake = PossibleMoves(true);

            if (PossibleMovesToMake != null)
            {
                RegularMoveNumber++;

                choice = PossibleMovesToMake.FirstOrDefault();

                foreach (var move in PossibleMovesToMake)
                {
                    NodesChecked = 0;
                    Memory.TakeHex(Me, move.Row, move.Column);
                    var scoreForThisMove = LetMeThinkAboutIt( MaxLevels, AbsoluteWorst, AbsoluteBest, false);
                    if (scoreForThisMove > bestScore)
                    {
                        bestScore = scoreForThisMove;
                        choice = Memory.Board.FirstOrDefault(x => x.Row == move.Row && x.Column == move.Column);
                    }
                    Memory.ReleaseHex(move.Row, move.Column);
                    Monitors[MovesExamined]++;
                    Monitors[MovesExaminedThisTurn]++;
                    Monitors[CurrentScore] = bestScore;
                    RelayPerformanceInformation();
                    //Quip("1 potential move with " + NodesChecked + " nodes checked in " + watch.ElapsedMilliseconds + " milliseconds");
                }
            }

            var score = LetMeThinkAboutIt( MaxLevels, AbsoluteWorst, AbsoluteBest, true);
            Quip( "Best score found was " + score);
            watch.Stop();
            Monitors[TotalTimeThinking] = Monitors[TotalTimeThinking] +  Convert.ToInt32(watch.ElapsedMilliseconds);


            // And when in doubt, get a random one
            if (choice == null)
            {
                Monitors[NumberOfRandomMoves]++;
                RandomMoveNumber++;

                choice = Memory.Board.OrderBy(x => x.RandomValue)
                    .FirstOrDefault(x => x.Owner == Common.PlayerType.White);
                Quip("Random choice : [" + choice.Row + ", " + choice.Column + "] #" + RandomMoveNumber);
            }
            else
            {
                Monitors[NumberOfPlannedMoves]++;
                Quip("Choosing: [" + choice.Row + ", " + choice.Column + "] normal move #" + RegularMoveNumber + " (" + watch.ElapsedMilliseconds + "ms) with " + PrunesMade + " prunes made and score was " + bestScore);
            }

            Monitors[AverageTimeToDecision] = Monitors[TotalTimeThinking] / (RegularMoveNumber + RandomMoveNumber);
            RelayPerformanceInformation();
            Memory.TakeHex(Me, choice.Row, choice.Column);

            return new Tuple<int, int>(choice.Row, choice.Column);
        }

        private int LetMeThinkAboutIt(int depth, int alpha, int beta, bool isMaximizing)
        {


            if (depth == 0 || Memory.Board.All(x => x.Owner != Common.PlayerType.White))
            {
                return ScoreFromBoard();
            }

            var possibleMoves = PossibleMoves(isMaximizing);
            if (!possibleMoves.Any())
            {
                possibleMoves.AddRange(Memory.Board
                    .OrderBy(x => x.RandomValue)
                    .Where(x => x.Owner == Common.PlayerType.White && possibleMoves.All(y => y != x)));
            }
            if (possibleMoves.Any(x => x.Owner == Common.PlayerType.White))
            {

                if (isMaximizing)
                {
                    var bestValue = alpha;

                    foreach (var move in possibleMoves.Where(x => x.Owner == Common.PlayerType.White))
                    {
                        Memory.TakeHex(CurrentlySearchingAs(true), move.Row, move.Column);
                        bestValue = Math.Max(bestValue, LetMeThinkAboutIt(depth - 1, alpha, beta, false));
                        alpha = Math.Max(alpha, bestValue);
                        NodesChecked++;
                        Monitors[NumberOfNodesChecked]++;
                        Memory.ReleaseHex(move.Row, move.Column);
                        if (beta <= alpha)
                        {
                            Monitors[NumberOfPrunesMade]++;
                            PrunesMade++;
                            break;
                        }

                    }
                    return bestValue;
                }
                else
                {
                    var bestValue = beta;
                    foreach (var move in possibleMoves.Where(x => x.Owner == Common.PlayerType.White))
                    {

                        Memory.TakeHex(CurrentlySearchingAs(false), move.Row, move.Column);
                        bestValue = Math.Min(bestValue, LetMeThinkAboutIt( depth - 1, alpha, beta, true));
                        beta = Math.Min(beta, bestValue);
                        NodesChecked++;
                        Monitors[NumberOfNodesChecked]++;
                        Memory.ReleaseHex(move.Row, move.Column);
                        if (beta <= alpha)
                        {
                            Monitors[NumberOfPrunesMade]++;
                            PrunesMade++;
                            break;
                        }
                    }
                    return bestValue;
                }
            }
            else
            {

                return ScoreFromBoard();
            }
        }

        private List<ListNode> PossibleMoves( bool isMaximizing)
        {
            // The possible moves are generated by better looking in our path,
            // followed by the opponent path, followed then by open hexes on no path
            var myBestPathFromHere = StartLookingForBestPath(isMaximizing, Memory);
             var possibleMoves = new List<ListNode>();
            if (myBestPathFromHere != null)
                possibleMoves
                    .AddRange(myBestPathFromHere
                        .OrderBy(x => x.F)
                        .ThenBy(x => x.GetDistanceToStart() + x.GetDistanceToEnd())
                        .ThenBy(x => x.LookAtMe)
                        .ThenBy(x => x.RandomValue)
                        .Where(x => x.Owner == Common.PlayerType.White));

            var opponentBestPathFromHere = StartLookingForBestPath(!isMaximizing, Memory);

            if (opponentBestPathFromHere != null)
                possibleMoves
                    .AddRange(opponentBestPathFromHere
                        .OrderBy(x => x.LookAtMe)
                        .ThenBy(x => x.RandomValue)
                        .Where(x => x.Owner == Common.PlayerType.White));
            possibleMoves.AddRange(Memory.Board
                .OrderBy(x => x.RandomValue)
                .Where(x => x.Owner == Common.PlayerType.White && possibleMoves.All(y => y != x)));
            return possibleMoves;
        }

        private int ScoreFromPath(List<ListNode> path)
        {
            if (path == null)
            {
                return Size;
            }
            return Size - path.Count(x => x.Owner == Common.PlayerType.White);
      
        }

        private int ScoreFromBoard()
        {
            int player1Score;
            int player2Score;
            if (Me == Common.PlayerType.Blue)
            {
                player1Score = (Size - Memory.Top.RemainingDistance()) + (Size - Memory.Bottom.RemainingDistance());
                player2Score = (Size - Memory.Left.RemainingDistance()) + (Size - Memory.Right.RemainingDistance());

            } 
            else
            {
                player2Score = (Size - Memory.Top.RemainingDistance()) + (Size - Memory.Bottom.RemainingDistance());
                player1Score = (Size - Memory.Left.RemainingDistance()) + (Size - Memory.Right.RemainingDistance());

            }

            var score = player1Score - player2Score;

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
                    .Where(x => x.Location == NodeLocation.Board)
                    .OrderBy(x => x.F)
                    .ThenByDescending(x => x.LookAtMe)
                    .ThenBy(x => x.RandomValue)
                    .FirstOrDefault(x => x.Status == Status.Open);
            }

            if (bestLookingNode == null )
            {
                return null;
            }

            // CLOSE IT
            bestLookingNode.Status = Status.Closed;

            if (bestLookingNode.IsNeighboursWith(end) && bestLookingNode.IsNeighboursWith(start))
            {
                var preferredPath = new List<ListNode>();
                var parent = bestLookingNode;
                while (parent != null && parent.Location == NodeLocation.Board)
                {
                    preferredPath.Add(parent);
                    parent = parent.Parent;
                }

                return preferredPath;
            }
            
            var neighbours = bestLookingNode.Neighbours.OrderBy(x => x.RemainingDistance());

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