using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.GameTypes;
using Engine.Interfaces;

namespace Engine.Players
{
    public class PathfinderNode : BaseNode
    {
       
        public Status Status;
        public int G;
        public int H;
        public PathfinderNode Parent = null;

        public int F => G + H;

    }

    public enum Status
    {
        Open,
        Closed,
        Untested
    };

    /*
     *  PATH FINDER PLAYER
     *
     * This player type is only basically intelligent.  It will:
     *
     * 1) Select a random hex on the 0 row/column to begin with
     *
     * 2) It will keep track of game state
     *
     * 3) It will Update its own representation of the board with enemy moves
     *
     * 4) It will find best path based on A*
     *
     * 5) It will select the next hex available on the path it finds
     *
     * It will NOT:
     *
     * 1) plan ahead
     *
     * 2) React to the enemy in a thoughtful way
     */

    public class PathFinderPlayer : Player
    {
        private List<PathfinderNode> _preferredPath;
        private new List<PathfinderNode> _memory;
        private bool havePath = false;
        private bool poppedMyMoveCherry = false;
        private PathfinderNode nodeIWant;
      
        public PathFinderPlayer(int playerNumber, int boardSize) : base(playerNumber, boardSize)
        {
            _preferredPath = new List<PathfinderNode>();
        }

        public string PlayerType()
        {
            return "Pathfinder AI";
        }
        public bool IsAvailableToPlay()
        {
            return true;
        }

        public override Tuple<int, int> SelectHex(Tuple<int, int> opponentMove)
        {
            if (opponentMove != null)
            {
                // Let's note the enemy's movement
                PathfinderNode enemyHex =
                    (PathfinderNode) _memory.FirstOrDefault(hex => hex.X == opponentMove.Item1 && hex.Y == opponentMove.Item2);

                if (enemyHex != null)
                {
                    enemyHex.Owner = PlayerNumber == 1 ? 2 : 1;
                    enemyHex.Status = Status.Closed;

                    // If we have a preferred path, but this is on it, we will need to recalculate
                    if (havePath && _preferredPath.Any(hex => hex.X == enemyHex.X && hex.Y == enemyHex.Y))
                    {
                        havePath = false;
                    }

                }
            }

            if (!havePath)
            {
                nodeIWant = null;

                LookForPath();
            }


            if (nodeIWant != null)
            {
                return new Tuple<int, int>(nodeIWant.X, nodeIWant.Y);
            }
            return null;
        }

        private bool IsNodeAtBeginning(PathfinderNode node)
        {
            if (_isHorizontal)
            {
                return node.Y == 0;
            }

            return node.X == 0;
        }

        private bool IsNodeAtEnd(PathfinderNode node)
        {
            if (_isHorizontal)
            {
                return node.Y == _size - 1;
            }

            return node.X == _size - 1;
        }

        private void LookForPath()
        {
            // If we don't have any open hexes to start with we's boneded :)
            if (!_memory.Any(x => x.Status == Status.Open))
            {
                if (!poppedMyMoveCherry)
                {
                    // Get a random starting node
                    poppedMyMoveCherry = true;
                    var node = _memory.Where(IsNodeAtBeginning).OrderBy(x => Guid.NewGuid())
                        .FirstOrDefault(final => final.Owner == 0);
                    if (node != null)
                    {
                        nodeIWant = node;
                        nodeIWant.Owner = PlayerNumber;
                        nodeIWant.G = 0;
                        return;
                    }
                }

                return;
            }

            // GEt the best looking node
            var bestLookingNode = _memory.OrderBy(x => x.F).ThenBy(y => Guid.NewGuid())
                .FirstOrDefault(z => z.Status == Status.Open);

            if (bestLookingNode == null)
            {
                return;
            }

            bestLookingNode.Status = Status.Closed;
            if (IsNodeAtEnd(bestLookingNode))
            {
                var parent = bestLookingNode;
                while (parent != null)
                {
                    _preferredPath.Add(parent);
                    parent = parent.Parent;
                }
            }
        }
        

        protected override void SetUpInMemoryBoard()
        {
            _memory = new List<PathfinderNode>();

            for (int x = 0; x < EndNodeLocation; x++)
            {
                for (int y = 0; y < EndNodeLocation; y++)
                {
                    var newNode = new PathfinderNode();
                    {
                        newNode.X = x;
                        newNode.Y = y;
                        newNode.Owner = 0;

                        if (_isHorizontal)
                        {
                            newNode.Status = y == 0 ? Status.Open : Status.Untested;
                            newNode.H = EndNodeLocation - y;
                        }
                        else
                        {
                            newNode.Status = x == 0 ? Status.Open : Status.Untested;
                            newNode.H = EndNodeLocation - x;
                        }
                    }
                }
            }
        }

    }
}
