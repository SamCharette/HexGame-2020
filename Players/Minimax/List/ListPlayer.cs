using System;
using System.Collections.Generic;
using System.Linq;
using Players.Common;

namespace Players.Minimax.List
{
    public class ListPlayer : MinimaxPlayer
    {
        public ListMap Memory { get; set; }
        public ListPlayer(int playerNumber, int boardSize, Config playerConfig) : base(playerNumber, boardSize, playerConfig)
        {
            PlayerNumber = playerNumber;
            Me = PlayerNumber == 1 ? Common.PlayerType.Blue : Common.PlayerType.Red;
            Size = boardSize;
            MaxLevels = GetDefault(playerConfig, "maxLevels", 5);
            CostPerNodeTillEnd = GetDefault(playerConfig, "costPerNodeTillEnd", 10);
            CostToMoveToUnclaimedNode = GetDefault(playerConfig, "costToMoveToUnclaimedNode", 2);
            CostToMoveToClaimedNode = GetDefault(playerConfig, "costToMoveToClaimedNode", 0);
            talkative = Convert.ToInt32(playerConfig.talkative);
            
            Name = playerConfig.name;
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

        public void Startup()
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
        }

        public override Tuple<int, int> SelectHex(Tuple<int, int> opponentMove)
        {
            Memory.TakeHex(Opponent(), opponentMove.Item1, opponentMove.Item2);

            // When in doubt, choose random
            var choice = JustGetARandomHex();
            return new Tuple<int, int>(choice.Row, choice.Column);
        }

        public PlayerType Opponent()
        {
            if (Me == Common.PlayerType.Blue)
            {
                return Common.PlayerType.Red;
            }

            return Common.PlayerType.Blue;
        }

        public int ScoreFromBoard()
        {
            // Get the player score
            var playerScore = 0;
            var opponentScore = 0;
            if (Me == Common.PlayerType.Blue)
            {
                var path = FindPath(Memory.Top, Memory.Bottom, Me);
                playerScore = Size - path.Count(x => x.Owner == Common.PlayerType.White);
                var opponentPath = FindPath(Memory.Left, Memory.Right, Opponent());
                opponentScore = Size - opponentPath.Count(x => x.Owner == Common.PlayerType.White);
            } else
            {
                var path = FindPath(Memory.Left, Memory.Right, Me);
                playerScore = Size - path.Count(x => x.Owner == Common.PlayerType.White);
                var opponentPath = FindPath(Memory.Top, Memory.Bottom, Opponent());
                opponentScore = Size - opponentPath.Count(x => x.Owner == Common.PlayerType.White);

            }

            return playerScore - opponentScore;
        }

        public List<ListHex> FindPath(ListHex start, ListHex end, PlayerType player)
        {
        
            Memory.CleanPathingVariables();
       
            var neighbours = Memory.GetTraversablePhysicalNeighbours(start, player);
            neighbours.ForEach(x => x.Status = Status.Open);
            return PathBetween(start, end, player);

        }

        public List<ListHex> PathBetween(ListHex start, ListHex end, Common.PlayerType player)
        {
            // Get the best looking node
            var bestLookingHex = Memory.Board
                .OrderBy(x => x.F)
                .ThenBy(x => x.RandomValue)
                .FirstOrDefault(z => z.Status == Status.Open);

            if (bestLookingHex == null)
            {
                if (start.Status == Status.Untested || start.Status == Status.Open)
                {
                    bestLookingHex = start;
                }
                else
                {
                    return new List<ListHex>();
                }
               
            }

            if (Memory.ArePhysicalNeighbours(bestLookingHex, end))
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

           
            var neighbours = Memory.GetTraversablePhysicalNeighbours(bestLookingHex, Me);

            foreach (var node in neighbours)
            {
                if (node.Owner != Opponent())
                {
                    if (node.Status == Status.Open)
                    {
                        if (node.G > bestLookingHex.G + (node.Owner == player ? CostToMoveToClaimedNode : CostToMoveToUnclaimedNode))
                        {
                            node.Parent = bestLookingHex;
                            node.G = bestLookingHex.G + (node.Owner == player ? CostToMoveToClaimedNode : CostToMoveToUnclaimedNode); ;
                            node.H = (player == Common.PlayerType.Red ? _size - 1 - node.Column : _size - 1 - node.Row) * CostPerNodeTillEnd;
                        }
                    }
                    else if (node.Status == Status.Untested)
                    {
                        node.Status = Status.Open;
                        node.Parent = bestLookingHex;
                        node.G = bestLookingHex.G + (node.Owner == player ? CostToMoveToClaimedNode : CostToMoveToUnclaimedNode);
                        node.H = (player == Common.PlayerType.Red ? _size - 1 - node.Column : _size - 1 - node.Row) * CostPerNodeTillEnd;
                    }
                }


            }
            return PathBetween(start, end, player);
        }

    }
}
