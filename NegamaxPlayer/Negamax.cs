using System;
using System.Collections.Generic;
using System.Linq;
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
            return player == 1 ? 2 : 1;
        }

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
        public PlayerType Me => PlayerNumber == 1 ? Players.PlayerType.Blue : Players.PlayerType.Red;
        public PlayerType Them => PlayerNumber == 1 ? Players.PlayerType.Red : Players.PlayerType.Blue;
        public Negamax(int playerNumber, int boardSize, Config playerConfig) : base(playerNumber, boardSize, playerConfig)
        {
            Setup(boardSize);
            MaxDepth = GetDefault(playerConfig, "maxLevels", 5);
            Talkative = GetDefault(playerConfig, "talkative", 0);
            StartingLevels = GetDefault(playerConfig, "startingLevels", 3);
            MovesBetweenLevelJump = GetDefault(playerConfig, "movesPerBrainJump", 1);
            CostPerNodeTillEnd = GetDefault(playerConfig, "costPerNodeTillEnd", 25);
            CostToMoveToUnclaimedNode = GetDefault(playerConfig, "costToMoveToUnclaimedNode", 5);
            CostToMoveToClaimedNode = GetDefault(playerConfig, "costToMoveToClaimedNode", 0);

            MovesMade = 0;
        }

        public Negamax()
        {

        }

        public void Setup(int size)
        {
            Size = size;
            Board = new Board();
            Board.Setup(Size);

        }

        public override Tuple<int, int> SelectHex(Tuple<int, int> opponentMove)
        {
            if (opponentMove != null)
            {
                Board.TakeHex(opponentMove, OtherPlayer(PlayerNumber));
            }
            BestHex = null;

            DoNegamax(Board, CurrentLevels, -9999, 9999, PlayerNumber);

            if (BestHex == null)
            {
                var openNodes = Board.Hexes.Where(x => x.Owner == 0);
                var selectedNode = openNodes.OrderBy(x => Guid.NewGuid()).FirstOrDefault();

                BestHex = selectedNode.ToTuple();

                Quip("Had to pick randomly.");
            }

            Board.TakeHex(BestHex, PlayerNumber);
            MovesMade++;
            return BestHex;
        }

        private PlayerType Opponent()
        {
            return this.PlayerNumber == 1 ? Players.PlayerType.Red : Players.PlayerType.Blue;
        }

        private int DoNegamax(Board gameState, int currentDepth, int alpha, int beta, int playerNumber)
        {
            if (currentDepth == 0 || gameState.HasWinner())
            {
     
                return playerNumber == this.PlayerNumber ? gameState.Score(this.PlayerNumber) : gameState.Score(this.PlayerNumber) * -1;
            }


            var value = -9999;
            var scout = new Pathfinder(Board, playerNumber);

            var myPath = scout.GetPathForPlayer().Where(x => x.Owner == 0).ToList();
            scout.SetPlayer(OtherPlayer(playerNumber));

            var theirPath = scout.GetPathForPlayer().Where(x => x.Owner == 0).ToList();
            var childNodes = new HashSet<Hex>();
            myPath.ForEach(x => childNodes.Add(x));
            theirPath.ForEach(x => childNodes.Add(x));

    
            foreach (var node in childNodes)
            {
                var newBoard = Board.GetCopy();
                newBoard.TakeHex(node.ToTuple(), playerNumber);
                var newValue = -1 * DoNegamax(newBoard, 
                                   currentDepth - 1, 
                                   -1 * alpha, 
                                   -1 * beta, 
                                   OtherPlayer(playerNumber));
                if (newValue > value)
                {
                    value = newValue;
                    BestHex = node.ToTuple();
                }
                alpha = Math.Max(alpha, value);
                if (alpha >= beta)
                {
                    break;
                }
            }

            return value;
        }

        public override string PlayerType()
        {
            return "Negamax AI";
        }
    }
}
