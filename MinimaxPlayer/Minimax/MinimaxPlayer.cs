using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data;
using MinimaxPlayer.Minimax.List;
using Players;

namespace MinimaxPlayer.Minimax
{
    public class MinimaxPlayer : Player
    {
        // Player default information
        private new string _type = "Minimax with Alpha Beta Pruning";
        private new string _codeName = "Minnie";
        private new string _version = "MMABP List v1-";
        private new string _defaultName = "Minnie";
        private new string _description =
            "Uses A* to find best paths, then the minimax algorithm with alpha beta pruning to determine which to take.  Generally stronger than Dozer.";


        public PlayerType Me;
        public int Size;

        protected const int AbsoluteWorst = -9999;
        protected const int AbsoluteBest = 9999;
        protected const string MovesExamined = "Total Moves Examined";
        protected const string MovesExaminedThisTurn = "Moves Examined This Turn";
        protected const string CurrentScore = "Current Score";
        protected const string AverageTimeToDecision = "Average time to decision";
        protected const string TotalTimeThinking = "Total time thinking";
        protected const string NumberOfRandomMoves = "# of random moves";
        protected const string NumberOfPlannedMoves = "# of planned moves";
        protected const string NumberOfNodesChecked = "Nodes Checked";
        protected const string NumberOfPrunesMade = "Prunes Made";

        public int MaxLevels { get; set; }
        public int CostToMoveToClaimedNode { get; set; }
        public int CostToMoveToUnclaimedNode { get; set; }
        public int CostPerNodeTillEnd { get; set; }

        public Dictionary<AxialDirections, Tuple<int, int>> Directions = new Dictionary<AxialDirections, Tuple<int, int>>()
        {
            { AxialDirections.TopLeft, new Tuple<int, int>(0, -1) },
            { AxialDirections.TopRight, new Tuple<int, int>(+1, -1) },
            { AxialDirections.Right, new Tuple<int, int>(+1, 0) },
            { AxialDirections.BottomRight, new Tuple<int, int>(0, +1) },
            { AxialDirections.BottomLeft, new Tuple<int, int>(-1, +1) },
            { AxialDirections.Left, new Tuple<int, int>(-1, 0) }
        };

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

        public MinimaxPlayer(int playerNumber, int boardSize, Config playerConfig) : base(playerNumber, boardSize,
            playerConfig)
        {
            SetVersionNumber("MinimaxPlayer.dll");

            PlayerNumber = playerNumber;
            Me = PlayerNumber == 1 ? Players.PlayerType.Blue : Players.PlayerType.Red;
            Size = boardSize;

            StartingLevels = GetDefault(playerConfig, "minLevels", 1);
            MaxLevels = GetDefault(playerConfig, "maxLevels", 5);
            MovesBetweenLevelJump = GetDefault(playerConfig, "movesPerBrainJump", 1);
            CostPerNodeTillEnd = GetDefault(playerConfig, "costPerNodeTillEnd", 25);
            CostToMoveToUnclaimedNode = GetDefault(playerConfig, "costToMoveToUnclaimedNode", 5);
            CostToMoveToClaimedNode = GetDefault(playerConfig, "costToMoveToClaimedNode", 0);
            Talkative = Convert.ToInt32((string)playerConfig.Talkative);


            Name = playerConfig.Name;
            MovesMade = 0;

            Startup();
        }

        private void Startup()
        {
            Memory = new ListMap(Size);

            CurrentChoice = null;
        }

        public override void GameOver(int winningPlayerNumber)
        {
            RelayPerformanceInformation();
            Memory = null;
        }


        public PlayerType Opponent()
        {
            if (Me == Players.PlayerType.Blue) return Players.PlayerType.Red;

            return Players.PlayerType.Blue;
        }


        public override string PlayerType()
        {
            return "Minimax AI";
        }

        private ListHex RandomHex()
        {
            Console.WriteLine("Getting a random hex.");
            var openNodes = Memory.Board.Where(x => x.Owner == Players.PlayerType.White);
            var selectedNode = openNodes.OrderBy(x => x.RandomValue).FirstOrDefault();
            return selectedNode;
        }


        public override Tuple<int, int> SelectHex(Tuple<int, int> opponentMove)
        {
            if (opponentMove != null)
            {
                Console.WriteLine("Opponent chose " + opponentMove);
                Memory.TakeHex(Opponent(), opponentMove.Item1, opponentMove.Item2);
            }

            CurrentChoice = null;

            var inquisitor = new Inquisitor();
            inquisitor.StartInquisition(Memory, this);
            CurrentChoice = inquisitor.GetChoice();

            if (CurrentChoice == null || Memory.HexAt(CurrentChoice).Owner != Players.PlayerType.White)
            {
                Console.WriteLine("Why are you trying to take hex " + CurrentChoice + "???");
                CurrentChoice = null;
            }

            CurrentChoice = CurrentChoice ?? RandomHex().ToTuple();

            Memory.TakeHex(Me, CurrentChoice.Item1, CurrentChoice.Item2);
            //var appraiser = new Appraiser();
            //var score = appraiser.ScoreFromBoard(Memory, this);
            //Console.WriteLine("Choosing " + CurrentChoice + " Score: " + score);
            MovesMade++;
            return CurrentChoice;
        }
    }
}
