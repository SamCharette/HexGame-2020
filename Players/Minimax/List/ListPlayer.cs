using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
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
            CostPerNodeTillEnd = GetDefault(playerConfig, "costPerNodeTillEnd", 25);
            CostToMoveToUnclaimedNode = GetDefault(playerConfig, "costToMoveToUnclaimedNode", 5);
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


            Startup();
        }

        private void Startup()
        {
            Memory = new ListMap(Size);
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

        

        public PlayerType Opponent()
        {
            if (Me == Common.PlayerType.Blue) return Common.PlayerType.Red;

            return Common.PlayerType.Blue;
        }


        public override string PlayerType()
        {
            return "Minimax AI";
        }

        private ListHex RandomHex()
        {
            Console.WriteLine("Getting a random hex.");
            var openNodes = Memory.Board.Where(x => x.Owner == Common.PlayerType.White);
            var selectedNode = openNodes.OrderBy(x => x.RandomValue).FirstOrDefault();
            return selectedNode;
        }


        public override Tuple<int, int> SelectHex(Tuple<int, int> opponentMove)
        {
            if (opponentMove != null)
            {
                Memory.TakeHex(Opponent(), opponentMove.Item1, opponentMove.Item2);
            }

            CurrentChoice = null;
            
            var inquisitor = new Inquisitor();
            inquisitor.StartInquisition(Memory, this);

            if(CurrentChoice == null || Memory.HexAt(CurrentChoice).Owner != Common.PlayerType.White)
            {
                Console.WriteLine("Why are you trying to take hex " + CurrentChoice + "???");
                CurrentChoice = null;
            } 

            CurrentChoice = CurrentChoice ?? RandomHex().ToTuple();

            Memory.TakeHex(Me, CurrentChoice.Item1, CurrentChoice.Item2);
            var appraiser = new Appraiser();
            var score = appraiser.ScoreFromBoard(Memory, this);
            Console.WriteLine("Choosing " + CurrentChoice + " Score: " + score);
            return CurrentChoice;
        }
   
     
    }
}