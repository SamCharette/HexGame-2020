using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Players;

namespace NegamaxPlayer
{
    public class Negamax : Player
    {
        public Board Board { get; set; }
        public int MaxDepth { get; set; }
        public Tuple<int,int> BestHex { get; set; }

        public int OtherPlayer(int player)
        {
            return player == 1 ? -1 : 1;
        } 
        public int Player { get;set; }

        public int MovesMade { get; set; }
        public int MovesBetweenLevelJump { get; set; }
        public int StartingLevels { get; set; }
        public int CostPerNodeTillEnd { get; set; }
        public int CostToMoveToUnclaimedNode { get; set; }
        public int CostToMoveToClaimedNode { get; set; }
        public int CurrentLevels
        {
            get
            {
                var numberOfJumps = MovesMade / MovesBetweenLevelJump;
                return Math.Min(MaxDepth, StartingLevels + numberOfJumps);
            }
        }

        public override string CodeName()
        {
            return "Naomi";
        }

        private int RandomMovesMade = 0;

        public Negamax(int playerNumber, int boardSize, Config playerConfig) : base(playerNumber, boardSize, playerConfig)
        {

            Setup(boardSize, playerNumber, playerConfig);
        }

        private int _size;
        public new int Size {get => _size;
            set
            {
                _size = value;
                Setup(_size);
            }

        }

        public Negamax()
        {
            Setup(11);
        }

        private void SetSettings(Config playerConfig)
        {
            MaxDepth = GetDefault(playerConfig, "maxLevels", 5);
            StartingLevels = GetDefault(playerConfig, "minLevels", 3);
            Talkative = GetDefault(playerConfig, "talkative", 0);
            MovesBetweenLevelJump = GetDefault(playerConfig, "movesPerBrainJump", 1);
            CostPerNodeTillEnd = GetDefault(playerConfig, "costPerNodeTillEnd", 25);
            CostToMoveToUnclaimedNode = GetDefault(playerConfig, "costToMoveToUnclaimedNode", 5);
            CostToMoveToClaimedNode = GetDefault(playerConfig, "costToMoveToClaimedNode", 0);
            Monitors["Items pruned"] = 0;
            Monitors["Transposition table uses"] = 0;

        }

        public void Setup(int size = 11, int playerNumber = 1, Config playerConfig = null)
        {
            Player = playerNumber == 1 ? 1 : -1; // This is the one the AI uses
            PlayerNumber = playerNumber; // this is the one the ref looks at
            _size = size;
            Board = new Board();
            Board.Setup(Size);
            MovesMade = 0;

            if (playerConfig != null)
            {
                SetSettings(playerConfig);
            }
        }

        public void Quip(List<Hex> path, string quip)
        {
            var thingToSay = quip;
            foreach (var hex in path)
            {
                thingToSay += " " + hex;
            }
            Quip(thingToSay, 2);
        }

        public override Tuple<int, int> SelectHex(Tuple<int, int> opponentMove)
        {
            if (opponentMove != null)
            {
                Board.TakeHex(opponentMove, OtherPlayer(Player));
                Quip("Opponent chose hex: " + opponentMove);
            }
            BestHex = null;
            Quip("-------------------------------------");
            Quip("-=-=-=-=-=-=-=-={ Turn " + (MovesMade + 1) + " for me }=-=-=-=-=-=-=-=-");
            Quip("-------------------------------------");

            RelayPerformanceInformation();
            var boardToCheck = new Board(Board);
            DoNegamax(boardToCheck, CurrentLevels, -9999, 9999, Player);

            if (BestHex != null && Board.HexAt(BestHex).Owner != 0)
            {
                RandomMovesMade++;
                Quip("Um, for some reason I like the idea of taking an already taken spot " + BestHex + " (" + RandomMovesMade + " Random moves made)");
                BestHex = null;
            }
            if (BestHex == null)
            {
                var openNodes = Board.Hexes.Where(x => x.Owner == 0);
                var selectedNode = openNodes.OrderBy(x => Guid.NewGuid()).FirstOrDefault();

                BestHex = selectedNode.ToTuple();

                Quip("Had to pick randomly.");
            }

            Board.TakeHex(BestHex, Player);
            MovesMade++;
            Quip("Selecting hex: " + BestHex);
            return BestHex;
        }


        private int DoNegamax(Board gameState, int currentDepth, int alpha, int beta, int pointOfView)
        {
            if (currentDepth == 0 || gameState.HasWinner() || gameState.Hexes.All(x => x.Owner != 0))
            {
                var score = gameState.Score(this.Player);
                return pointOfView * score;
            }

            var queue = new PriorityQueue();

            var scout = new Pathfinder(gameState, pointOfView);
            var myPath = scout.GetPathForPlayer().Where(x => x.Owner == 0).ToList();
            

            scout.SetPlayer(-1 * pointOfView);
            var theirPath = scout.GetPathForPlayer().Where(x => x.Owner == 0).ToList();
            
            var childNodes = new HashSet<Hex>();
            myPath.ForEach(x => queue.Push(x)); 
            theirPath.ForEach(x => queue.Push(x));
            var combinedPath = myPath.Where(x => theirPath.Contains(x)).ToList();
            var queuedItemsToSearch = "Queued items: ";
            while (childNodes.Count < 5 && queue.HasItems())
            {
                var hex = queue.Pop();
                queuedItemsToSearch += hex + " ";
                childNodes.Add(hex);
            }
 
            if (currentDepth == CurrentLevels)
            {
                Quip(myPath, "My path: ");
                Quip(theirPath, "Their path: ");
                Quip(combinedPath, "Nodes in both paths: ");
                Quip(queuedItemsToSearch);
            }

            foreach (var node in childNodes)
            {
                var newBoard = new Board(Board);
                newBoard.TakeHex(node.ToTuple(), pointOfView);
                var newValue = -1 * DoNegamax(newBoard, 
                                   currentDepth - 1, 
                                   -1 * beta, 
                                   -1 * alpha, 
                                   -pointOfView);
                if (newValue > alpha)
                {
                    alpha = newValue;
                    if (currentDepth == CurrentLevels)
                    {
                        BestHex = node.ToTuple();

                    }
                }

                if (beta <= alpha)
                {
                    Monitors["Items pruned"]++;
                    return beta;
                }
            }

        

            return alpha;
        }

        public override string PlayerType()
        {
            return "Negamax AI";
        }
    }
}
