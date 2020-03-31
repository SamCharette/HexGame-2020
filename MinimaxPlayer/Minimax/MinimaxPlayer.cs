using System;
using System.Collections.Generic;
using Players.Common;

namespace MinimaxPlayer.Minimax
{
    public class MinimaxPlayer : Player
    {
        public Players.Common.PlayerType Me;
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

        public MinimaxPlayer(int playerNumber, int boardSize, Config playerConfig) : base(playerNumber, boardSize, playerConfig)
        {
            PlayerNumber = playerNumber;
            Me = PlayerNumber == 1 ? Players.Common.PlayerType.Blue : Players.Common.PlayerType.Red;
            Size = boardSize;

        }
        public Dictionary<AxialDirections, Tuple<int, int>> Directions = new Dictionary<AxialDirections, Tuple<int, int>>()
        {
            { AxialDirections.TopLeft, new Tuple<int, int>(0, -1) },
            { AxialDirections.TopRight, new Tuple<int, int>(+1, -1) },
            { AxialDirections.Right, new Tuple<int, int>(+1, 0) },
            { AxialDirections.BottomRight, new Tuple<int, int>(0, +1) },
            { AxialDirections.BottomLeft, new Tuple<int, int>(-1, +1) },
            { AxialDirections.Left, new Tuple<int, int>(-1, 0) }
        };
    }
}
