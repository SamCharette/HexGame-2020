using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Omu.ValueInjecter;
using Players.Common;

namespace Players.Minimax.List
{
    /*
    *  This player is currently (Mar-16-2020) doing ok.  It's slow though.
    *
    *  Things to look at:
    *
    *  Scoring - better scoring function?
    *  Pathfinding - called way too often, I bet.  Should only restart pathfinding
    *      if current path is broken (like dozer)
    *  Final Move Recognition - Should be able to identify when there is a final move it can
    *       make so that it doesn't lose by choosing a different one
    *  Threatened Game State - should be able to identify when the opponent is one
    *       move away from winning so that it can stop them.
    *
    *
    */
    public class ListPlayer : MinimaxPlayer
    {
        private readonly object _workingLock;

        public ListPlayer(int playerNumber, int boardSize, Config playerConfig) : base(playerNumber, boardSize,
            playerConfig)
        {
            PlayerNumber = playerNumber;
            Me = PlayerNumber == 1 ? Common.PlayerType.Blue : Common.PlayerType.Red;
            Size = boardSize;

            StartingLevels = GetDefault(playerConfig, "minLevels", 1);
            MaxLevels = GetDefault(playerConfig, "maxLevels", 5);
            MovesBetweenLevelJump = GetDefault(playerConfig, "movesPerBrainJump", 1);
            MaxLevels = GetDefault(playerConfig, "maxLevels", 5);
            CostPerNodeTillEnd = GetDefault(playerConfig, "costPerNodeTillEnd", 5);
            CostToMoveToUnclaimedNode = GetDefault(playerConfig, "costToMoveToUnclaimedNode", 2);
            CostToMoveToClaimedNode = GetDefault(playerConfig, "costToMoveToClaimedNode", 0);
            talkative = Convert.ToInt32(playerConfig.talkative);


            Name = playerConfig.name;
            Monitors[MovesExamined] = 0;
            Monitors[MovesExaminedThisTurn] = 0;
            Monitors[CurrentScore] = 0;
            Monitors[AverageTimeToDecision] = 0;
            Monitors[TotalTimeThinking] = 0;
            Monitors[NumberOfPlannedMoves] = 0;
            Monitors[NumberOfRandomMoves] = 0;
            Monitors[NumberOfNodesChecked] = 0;
            Monitors[NumberOfPrunesMade] = 0;

            MovesMade = 0;
            _workingLock = new object();

            Startup();
        }

        public ListMap Memory { get; set; }
        public Tuple<int, int> CurrentChoice { get; set; }
        public Queue<ListHex> MoveQueue { get; set; }
        public List<ListHex> ProposedPath { get; set; }
        public ConcurrentDictionary<ListHex, int> MoveScores { get; set; }
        public int StartingLevels { get; set; }

        public int CurrentLevels
        {
            get
            {
                var numberOfJumps = MovesMade / MovesBetweenLevelJump;
                return Math.Min(MaxLevels, StartingLevels + numberOfJumps);
            }
        }

        public int MovesMade { get; set; }

        public int MovesBetweenLevelJump { get; set; }
        public int MaximumThreads => Environment.ProcessorCount / 2;
        public List<Task> Threads { get; set; }

        public int CurrentThreadsInUse { get; set; }

        public override string PlayerType()
        {
            return "Minimax AI";
        }

        public void Startup()
        {
            Memory = new ListMap(Size);
            Memory.Name = "Memory";
            Monitors[MovesExamined] = 0;
            Monitors[MovesExaminedThisTurn] = 0;
            Monitors[CurrentScore] = 0;
            Monitors[AverageTimeToDecision] = 0;
            Monitors[TotalTimeThinking] = 0;
            Monitors[NumberOfPlannedMoves] = 0;
            Monitors[NumberOfRandomMoves] = 0;
            Monitors[NumberOfNodesChecked] = 0;
            Monitors[NumberOfPrunesMade] = 0;
            CurrentChoice = null;
        }

        public override Tuple<int, int> SelectHex(Tuple<int, int> opponentMove)
        {
            if (opponentMove != null)
                if (!Memory.TakeHex(Opponent(), opponentMove.Item1, opponentMove.Item2))
                    Quip("Problem setting opponent's move. " + opponentMove);

            var turnStartTime = DateTime.Now;


            // TODO Add a check to see if there are any hexes that are winners
            foreach (var hex in Memory.Board) 
            {
                if (Memory.CanHexReachBothEnds(hex, Me))
                {
                    CurrentChoice = hex.ToTuple();
                    Quip("Found a winner! " + CurrentChoice);
                    break;
                }
            }


            CurrentChoice = null;

            if (CurrentChoice == null)
            {
                MoveQueue = new Queue<ListHex>();
                MoveScores = new ConcurrentDictionary<ListHex, int>();
                ProposedPath = new List<ListHex>();
                Threads = new List<Task>();
                CurrentThreadsInUse = 0;
                Monitors[MovesExaminedThisTurn] = 0;


                ProposedPath = GetAPathForMe(Memory).Where(x => x.Owner == Common.PlayerType.White).ToList();
                var theirPath = GetAPathForOpponent(Memory).Where(x => x.Owner == Common.PlayerType.White).ToList();
                var possibleMoves = GetPossibleMovesFrom(ProposedPath, theirPath, true);

                foreach (var move in possibleMoves)
                    //      Quip("Enqueueing move: " + move);
                    if (move.Owner == Common.PlayerType.White)
                        MoveQueue.Enqueue(move);
                    else
                        Quip("Egads! " + move + " is already taken!");

                while (MoveQueue.Any() || Threads.Any(x => x.Status == TaskStatus.Running))
                    if (MoveQueue.Any() && CurrentThreadsInUse < MaximumThreads)
                    {
                        CurrentThreadsInUse++;

                        lock (_workingLock)
                        {
                            var move = MoveQueue.Dequeue();
                            var newMap = new  ListMap().InjectFrom( new CloneInjection(), Memory) as ListMap;
                            var newMapMove =
                                newMap.Board.FirstOrDefault(x => x.Row == move.Row && x.Column == move.Column);
                            Threads.Add(Task.Factory.StartNew(() => StartSearchingForScore(newMap, newMapMove)));
                        }
                    }

                Task.WaitAll(Threads.ToArray());

                CurrentChoice = MoveScores.ToList().OrderByDescending(x => x.Value).FirstOrDefault().Key.ToTuple();
                Monitors[MovesExamined] += Monitors[MovesExaminedThisTurn];
            }

            RelayPerformanceInformation();

            if (CurrentChoice == null)
            {
                Monitors[NumberOfRandomMoves]++;
                var hex = RandomHex();
                CurrentChoice = new Tuple<int, int>(hex.Row, hex.Column);
            }
            else
            {
                Monitors[NumberOfPlannedMoves]++;
            }

            Quip("Taking hex: " + CurrentChoice);
            Memory.TakeHex(Me, CurrentChoice);
            var timeTaken = (int) DateTime.Now.Subtract(turnStartTime).TotalMilliseconds;
            MovesMade++;
            Monitors[TotalTimeThinking] += timeTaken;
            Monitors[AverageTimeToDecision] = Monitors[TotalTimeThinking] /
                                              (Monitors[NumberOfRandomMoves] + Monitors[NumberOfPlannedMoves]);
            RelayPerformanceInformation();
            return CurrentChoice;
        }

        public void StartSearchingForScore(ListMap searchInThisMap, ListHex move)
        {
            RelayPerformanceInformation();
            Monitors[MovesExaminedThisTurn]++;
            var score = ThinkAboutTheNextMove(searchInThisMap, 
                ProposedPath, 
                move, 
                CurrentLevels, 
                AbsoluteWorst,
                AbsoluteBest, 
                false);

            MoveScores[move] = score;
            CurrentThreadsInUse--;
        }

        public ListHex RandomHex()
        {
            var openNodes = Memory.Board.Where(x => x.Owner == Common.PlayerType.White);
            var selectedNode = openNodes.OrderBy(x => x.RandomValue).FirstOrDefault();
            return selectedNode;
        }

        public int ThinkAboutTheNextMove(
            ListMap map,
            List<ListHex> path,
            ListHex currentMove,
            int depth,
            int alpha,
            int beta,
            bool isMaximizing)
        {
            if (depth == 0 || map.Board.All(x => x.Owner != Common.PlayerType.White))
            {
                return ScoreFromBoard(map);
            }
            if (!map.TakeHex(isMaximizing ? Me : Opponent(), currentMove))
                Quip("Problem taking hex as part of my searching." + currentMove);

            var myPath = GetAPathForPlayer(map, isMaximizing);

            var possibleMoves = GetPossibleMovesFrom(myPath, path, isMaximizing);
            if (!possibleMoves.Any())
            {
                var score = ScoreFromBoard(map);
                if (!map.ReleaseHex(currentMove)) Quip("Problem releasing hex when no moves are found " + currentMove);

                return score;
            }

            if (isMaximizing)
            {
                foreach (var move in possibleMoves)
                {
                    alpha = Math.Max(alpha, ThinkAboutTheNextMove(map, myPath, move, depth - 1, alpha, beta, false));
                    Monitors[NumberOfNodesChecked]++;
                    if (beta <= alpha)
                    {
                        Monitors[NumberOfPrunesMade]++;
                        break;
                    }
                }

                if (!map.ReleaseHex(currentMove)) Quip("Problem releasing hex after maximizing " + currentMove);

                return alpha;
            }

            foreach (var move in possibleMoves)
            {
                beta = Math.Min(beta, ThinkAboutTheNextMove(map, myPath, move, depth - 1, alpha, beta, true));
                Monitors[NumberOfNodesChecked]++;

                if (beta <= alpha)
                {
                    Monitors[NumberOfPrunesMade]++;
                    break;
                }
            }

            if (!map.ReleaseHex(currentMove)) Quip("Problem releasing hex after minimizing " + currentMove);
            return beta;
        }

        private List<ListHex> GetPossibleMovesFrom(List<ListHex> myPath, List<ListHex> theirPath, bool isMaximizing)
        {
            var possibleMoves = new ConcurrentBag<ListHex>();
            var bothLike = myPath.Where(theirPath.Contains).ToList();

            foreach (var hex in bothLike.Where(x => x.Owner == Common.PlayerType.White))
            {
                possibleMoves.Add(hex);
            }
            foreach (var hex in myPath.Where(x => x.Owner == Common.PlayerType.White
                                                  && !bothLike.Any(y => y.Row == x.Row && y.Column == x.Column)).ToList())
            {
                possibleMoves.Add(hex);
            }
            //foreach (var hex in theirPath.Where(x => x.Owner == Common.PlayerType.White).ToList())
            //    possibleMoves.TryAdd(hex.HexName, hex);

            return possibleMoves.ToList();
        }

        public PlayerType CurrentlySearchingAs(bool isMaximizing)
        {
            return isMaximizing ? Me : Opponent();
        }

        public override void GameOver(int winningPlayerNumber)
        {
            RelayPerformanceInformation();
            Memory = null;
        }

        public PlayerType Opponent()
        {
            if (Me == Common.PlayerType.Blue) return Common.PlayerType.Red;

            return Common.PlayerType.Blue;
        }

        public List<ListHex> GetAPathForPlayer(ListMap map, bool isMaximizing)
        {
            map.CleanPathingVariables();

            return isMaximizing ? GetAPathForMe(map) : GetAPathForOpponent(map);
        }

        public List<ListHex> GetAPathForMe(ListMap map)
        {
            var start = Me == Common.PlayerType.Blue ? map.Top : map.Left;
            var end = Me == Common.PlayerType.Blue ? map.Bottom : map.Right;
            return FindPath(map, start, end, Me);
        }

        public List<ListHex> GetAPathForOpponent(ListMap map)
        {
            var start = Opponent() == Common.PlayerType.Blue ? map.Top : map.Left;
            var end = Opponent() == Common.PlayerType.Blue ? map.Bottom : map.Right;
            return FindPath(map, start, end, Opponent());
        }

        public int ScoreFromBoard(ListMap map)
        {
            // Get the player score
            var playerScore = 0;
            var opponentScore = 0;


            //if (Me == Common.PlayerType.Blue)
            //{
            //    var path = FindPath(Memory.Top, Memory.Bottom, Me);
            //    playerScore = Size - path.Count(x => x.Owner == Common.PlayerType.White);
            //    var opponentPath = FindPath(Memory.Left, Memory.Right, Opponent());
            //    opponentScore = Size - opponentPath.Count(x => x.Owner == Common.PlayerType.White);
            //} else
            //{
            //    var path = FindPath(Memory.Left, Memory.Right, Me);
            //    playerScore = Size - path.Count(x => x.Owner == Common.PlayerType.White);
            //    var opponentPath = FindPath(Memory.Top, Memory.Bottom, Opponent());
            //    opponentScore = Size - opponentPath.Count(x => x.Owner == Common.PlayerType.White);

            //}
            var path = FindPath(map, map.Top, map.Bottom, Me);
            playerScore = Size - path.Count(x => x.Owner == Common.PlayerType.White);
            var opponentPath = FindPath(map, map.Left, map.Right, Opponent());
            opponentScore = Size - opponentPath.Count(x => x.Owner == Common.PlayerType.White);

            //var path = FindPath(map, map.Top, map.Bottom, Me);
            //playerScore = path.OrderByDescending(x => x.F).FirstOrDefault()?.F ?? 0;
            //var opponentPath = FindPath(map, map.Left, map.Right, Opponent());
            //opponentScore = opponentPath.OrderByDescending(x => x.F).FirstOrDefault()?.F ?? 0;
            return playerScore - opponentScore;
        }

        //  private bool CanIWinWithThisMove(ListHex hex)
        // {

        //if (hex != null)
        //{
        //    Memory.TakeHex(Me, hex.Row, hex.Column);
        //    var canIWinHere = IsWinningMove(Me);
        //    Memory.ReleaseHex( hex.Row, hex.Column);
        //    if (canIWinHere)
        //    {
        //        Quip("Yes, I CAN win if I get here (" + hex.Row + ", " + hex.Column + ")");
        //    }
        //    return canIWinHere;
        //}

        //return false;
        //   }

        //private bool CanILoseIfIDontTakeThisHex(ListHex hex)
        //{

        //if (hex != null)
        //{
        //    Memory.TakeHex(Opponent(), hex.Row, hex.Column);
        //    var canILoseHere = IsWinningMove(Opponent());
        //    Memory.ReleaseHex(hex.Row, hex.Column);
        //    if (canILoseHere)
        //    {
        //        Quip("Bleh, THEY can win if they get here (" + hex.Row + ", " + hex.Column + ")");
        //    }
        //    return canILoseHere;
        //}

        //return false;
        //    }

        public bool IsWinningMove(PlayerType player, ListMap map)
        {
            var start = player == Common.PlayerType.Blue ? map.Top : map.Left;
            var end = player == Common.PlayerType.Blue ? map.Bottom : map.Right;

            return start.IsAttachedTo(end);
        }

        public List<ListHex> FindPath(ListMap map, ListHex start, ListHex end, PlayerType player)
        {
            map.CleanPathingVariables();

            var neighbours = map.GetTraversablePhysicalNeighbours(start, player);
            neighbours.ForEach(x => x.Status = Status.Open);
            var path = PathBetween(map, start, end, player);

            return path;
        }

        public List<ListHex> PathBetween(ListMap map, ListHex start, ListHex end, PlayerType player)
        {
            // Get the best looking node
            var bestLookingHex = map.Board
                .OrderBy(x => x.F())
                .ThenBy(x => x.RandomValue)
                .FirstOrDefault(z => z.Status == Status.Open);

            if (bestLookingHex == null)
            {
                if (start.Status == Status.Untested || start.Status == Status.Open)
                    bestLookingHex = start;
                else
                    return new List<ListHex>();
            }

            if (map.ArePhysicalNeighbours(bestLookingHex, end))
            {
                var preferredPath = new List<ListHex>();

                var parent = bestLookingHex;
                while (parent != null)
                {
                    preferredPath.Add(parent);
                    parent = parent.Parent;
                }

                return preferredPath;
            }

            bestLookingHex.Status = Status.Closed;


            var neighbours = map.GetTraversablePhysicalNeighbours(bestLookingHex, Me);

            foreach (var node in neighbours)
                if (node.Owner != Opponent())
                {
                    if (node.Status == Status.Open)
                    {
                        if (node.G > bestLookingHex.G +
                            (node.Owner == player ? CostToMoveToClaimedNode : CostToMoveToUnclaimedNode))
                        {
                            node.Parent = bestLookingHex;
                            node.G = bestLookingHex.G +
                                     (node.Owner == player ? CostToMoveToClaimedNode : CostToMoveToUnclaimedNode);
                            ;
                            node.H =
                                (player == Common.PlayerType.Red ? _size - 1 - node.Column : _size - 1 - node.Row) *
                                CostPerNodeTillEnd;
                        }
                    }
                    else if (node.Status == Status.Untested)
                    {
                        node.Status = Status.Open;
                        node.Parent = bestLookingHex;
                        node.G = bestLookingHex.G +
                                 (node.Owner == player ? CostToMoveToClaimedNode : CostToMoveToUnclaimedNode);
                        node.H = (player == Common.PlayerType.Red ? _size - 1 - node.Column : _size - 1 - node.Row) *
                                 CostPerNodeTillEnd;
                    }
                }

            return PathBetween(map, start, end, player);
        }
    }
}