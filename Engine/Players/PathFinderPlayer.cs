using System;
using System.Collections.Generic;
using System.IO;
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
        private int costToMoveToClaimedNode = 1;
        private int costToMoveToUnclaimedNode = 2;
        private int costPerNodeTillEnd = 1;
      
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
            if (!_memory.Any())
            {
                SetUpInMemoryBoard();
            }
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
                        Console.WriteLine("Enemy took a spot that I wanted for later");
                        havePath = false;
                    }

                }
            }

            if (!havePath)
            {
                nodeIWant = null;
                Console.WriteLine("I need a path...");
                LookForPath();
            }
            else
            {
                nodeIWant = _preferredPath.FirstOrDefault(x => x.Owner == 0);
            }


            if (nodeIWant != null)
            {
                nodeIWant.Owner = PlayerNumber;
                return new Tuple<int, int>(nodeIWant.X, nodeIWant.Y);
            }

            Console.WriteLine("Pfft.  I give up!");
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
                    Console.WriteLine("I'm popping my cherry!");
                    // Get a random starting node
                    poppedMyMoveCherry = true;
                    var node = _memory.Where(IsNodeAtBeginning).OrderBy(x => Guid.NewGuid())
                        .FirstOrDefault(final => final.Owner == 0);
                    if (node != null)
                    {
                        nodeIWant = node;
                        nodeIWant.Owner = PlayerNumber;
                        nodeIWant.G = 0;
                        node.H = (_isHorizontal ? _size - 1 - node.Y : _size - 1 - node.X) * costPerNodeTillEnd;

                        return;
                    }
                }

                return;
            }

            // GEt the best looking node
            PathfinderNode bestLookingNode = _memory
                .Where(z => z.Status == Status.Open)
                .OrderBy(x => x.F)
                .FirstOrDefault();

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

                nodeIWant = bestLookingNode;
                havePath = true;
                return;
            }

            var neighbours = _memory
                .Where(node => node.Status == Status.Untested)
                .Where(x => AreNeighbours(x, bestLookingNode));
            foreach (var node in neighbours)
            {
                if (node.Status == Status.Open)
                {
                    if (node.G > bestLookingNode.G + (IsMine(bestLookingNode)
                            ? costToMoveToClaimedNode
                            : costToMoveToUnclaimedNode))
                    {
                        node.Parent = bestLookingNode;
                        node.G = bestLookingNode.G + (IsMine(bestLookingNode)
                                     ? costToMoveToClaimedNode
                                     : costToMoveToUnclaimedNode);
                        node.H = (_isHorizontal ? _size - 1 - node.Y : _size - 1 - node.X) * costPerNodeTillEnd;
                    }
                }
                else
                {
                    node.Status = Status.Open;
                    node.Parent = bestLookingNode;
                    node.G = bestLookingNode.G + (IsMine(bestLookingNode)
                                 ? costToMoveToClaimedNode
                                 : costToMoveToUnclaimedNode);
                    node.H = (_isHorizontal ? _size - 1 - node.Y : _size - 1 - node.X) * costPerNodeTillEnd;
                }

            }
            LookForPath();

        }

        private bool IsMine(PathfinderNode node)
        {
            return node.Owner == PlayerNumber;
        }

        private bool AreNeighbours(PathfinderNode node, PathfinderNode possibleNeighbour)
        {
            // Can't be a neighbour to itself
            if (node.X == possibleNeighbour.X && node.Y == possibleNeighbour.Y)
            {
                return false;
            }
            //// is untested?
            //if (possibleNeighbour.State == NodeState.Closed)
            //{
            //    return false;
            //}
            //// Only walkable if we own it
            //if (neighbour.OwningPlayer != OwningPlayer)
            //{
            //    return false;
            //}
            // Top right
            if (node.X == possibleNeighbour.X + 1 && node.Y == possibleNeighbour.Y - 1)
            {
                return true;
            }
            // Right
            if (node.X == possibleNeighbour.X + 1 && node.Y == possibleNeighbour.Y)
            {
                return true;
            }
            // Bottom right
            if (node.X == possibleNeighbour.X && node.Y == possibleNeighbour.Y + 1)
            {
                return true;
            }
            // Bottom left
            if (node.X == possibleNeighbour.X - 1 && node.Y == possibleNeighbour.Y + 1)
            {
                return true;
            }
            // Left
            if (node.X == possibleNeighbour.X - 1 && node.Y == possibleNeighbour.Y)
            {
                return true;
            }
            // Top Left
            if (node.X == possibleNeighbour.X && node.Y == possibleNeighbour.Y - 1)
            {
                return true;
            }
            return false;
        }
        

        protected override void SetUpInMemoryBoard()
        {
            _memory = new List<PathfinderNode>();
            Random random = new Random();
            int firstPickCoordinate = random.Next(_size);

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
                            if (newNode.X == firstPickCoordinate && newNode.Y == 0)
                            {
                                newNode.Status = Status.Open;
                            }
                            else
                            {
                                newNode.Status = Status.Untested;
                            }
                            newNode.H = EndNodeLocation - y;
                        }
                        else
                        {
                            if (newNode.X == 0 && newNode.Y == firstPickCoordinate)
                            {
                                newNode.Status = Status.Open;
                            }
                            else
                            {
                                newNode.Status = Status.Untested;
                            }
                            newNode.H = EndNodeLocation - x;
                        }
                    }
                    _memory.Add(newNode);
                }
            }
        }

    }
}
