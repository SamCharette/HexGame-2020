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
            MaxLevels = GetDefault(playerConfig, "maxLevels", 20);
            CostPerNodeTillEnd = GetDefault(playerConfig, "costPerNodeTillEnd", 1000);
            CostToMoveToUnclaimedNode = GetDefault(playerConfig, "costToMoveToUnclaimedNode", 100);
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

            Memory = new ListMap(Size);
            //Memory.Reset(Size);

            Startup();
        }

        public void Startup()
        {

        }

        public Common.PlayerType Opponent()
        {
            if (Me == Common.PlayerType.Blue)
            {
                return Common.PlayerType.Red;
            }

            return Common.PlayerType.Blue;
        }

        public List<ListHex> FindPath(ListHex start, ListHex end)
        {
            //if (Memory.AreFriendlyNeighbours(start, end))
            //{
            //    return new List<ListHex> 
            //    {
            //        start,
            //        end
            //    };
            //}
            if (start.IsAttachedTo(end) || end.IsAttachedTo(start))
            {
                return new List<ListHex>
                {
                    start,
                    end
                };
            }

            Memory.CleanPathingVariables();
            start.Status = Status.Open;
            return PathBetween(start, end, Me);

        }

        public List<ListHex> PathBetween(ListHex start, ListHex end, Common.PlayerType player)
        {
            ListHex bestLookingHex = null;;

            // GEt the best looking node
            bestLookingHex = Memory.Board
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

            bestLookingHex.Status = Status.Closed;

            if (bestLookingHex == end)
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
                            node.H = (_isHorizontal ? _size - 1 - node.Column : _size - 1 - node.Row) * CostPerNodeTillEnd;
                        }
                    }
                    else if (node.Status == Status.Untested)
                    {
                        node.Status = Status.Open;
                        node.Parent = bestLookingHex;
                        node.G = bestLookingHex.G + (node.Owner == player ? CostToMoveToClaimedNode : CostToMoveToUnclaimedNode);
                        node.H = (_isHorizontal ? _size - 1 - node.Column : _size - 1 - node.Row) * CostPerNodeTillEnd;
                    }
                }


            }
            return PathBetween(bestLookingHex, end, player);
        }

    }
}
