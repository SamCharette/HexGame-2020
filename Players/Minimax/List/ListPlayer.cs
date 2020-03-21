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
    *
    *
    */
    public class ListPlayer : MinimaxPlayer
    {
        private readonly object _workingLock;
        public Tuple<int, int> CurrentChoice { get; set; }

        public int CurrentLevels
        {
            get
            {
                var numberOfJumps = MovesMade / MovesBetweenLevelJump;
                return Math.Min(MaxLevels, StartingLevels + numberOfJumps);
            }
        }

        public int CurrentThreadsInUse { get; set; }
        public int MaximumThreads => Environment.ProcessorCount / 2;

        public ListMap Memory { get; set; }
        public Queue<ListHex> MoveQueue { get; set; }

        public int MovesBetweenLevelJump { get; set; }
        public ConcurrentDictionary<ListHex, int> MoveScores { get; set; }

        public int MovesMade { get; set; }
        public List<ListHex> ProposedPath { get; set; }
        public int StartingLevels { get; set; }
        public List<Task> Threads { get; set; }

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


        public override void GameOver(int winningPlayerNumber)
        {
            RelayPerformanceInformation();
            Memory = null;
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
                                                  && !bothLike.Any(y => y.Row == x.Row && y.Column == x.Column))
                .ToList())
            {
                possibleMoves.Add(hex);
            }
            //foreach (var hex in theirPath.Where(x => x.Owner == Common.PlayerType.White).ToList())
            //    possibleMoves.TryAdd(hex.HexName, hex);

            return possibleMoves.ToList();
        }

      

        public bool IsWinningMove(PlayerType player, ListMap map)
        {
            var start = player == Common.PlayerType.Blue ? map.Top : map.Left;
            var end = player == Common.PlayerType.Blue ? map.Bottom : map.Right;

            return start.IsAttachedTo(end);
        }

        public PlayerType Opponent()
        {
            if (Me == Common.PlayerType.Blue) return Common.PlayerType.Red;

            return Common.PlayerType.Blue;
        }


        public override string PlayerType()
        {
            return "Minimax AI";
        }

        public ListHex RandomHex()
        {
            var openNodes = Memory.Board.Where(x => x.Owner == Common.PlayerType.White);
            var selectedNode = openNodes.OrderBy(x => x.RandomValue).FirstOrDefault();
            return selectedNode;
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
                            var newMap = new ListMap().InjectFrom(new CloneInjection(), Memory) as ListMap;
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

    }
}