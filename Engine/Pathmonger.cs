using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Engine.GameTypes;

namespace Engine
{
    public class Node
    {
        public Point Location;
        public float G;
        public float H;
        public float F { get { return this.G + this.H; } }
        public NodeState State;
        public Node ParentNode;
        public int Owner;
        public bool IsWalkableFromHere(Node neighbour)
        {
            // is untested?
            if (neighbour.State != NodeState.Untested)
            {
                return false;
            }
            // Only walkable if we own it
            if (neighbour.Owner != Owner)
            {
                return false;
            }
            // Top right
            if (neighbour.Location.X == Location.X + 1 && neighbour.Location.Y == Location.Y - 1)
            {
                return true;
            }
            // Right
            if (neighbour.Location.X == Location.X + 1 && neighbour.Location.Y == Location.Y)
            {
                return true;
            }
            // Bottom right
            if (neighbour.Location.X == Location.X && neighbour.Location.Y == Location.Y + 1)
            {
                return true;
            }
            // Bottom left
            if (neighbour.Location.X == Location.X - 1 && neighbour.Location.Y == Location.Y + 1)
            {
                return true;
            }
            // Left
            if (neighbour.Location.X == Location.X - 1 && neighbour.Location.Y == Location.Y)
            {
                return true;
            }
            // Top Left
            if (neighbour.Location.X == Location.X && neighbour.Location.Y == Location.Y - 1)
            {
                return true;
            }
            return false;
        }
    }

    public enum NodeState { Untested, Open, Closed }


    public class Pathmonger
    {

        public int Size;
        public int CurrentRecursion;
        public int MaxRecursion;
        public bool IsHorizontal;
        public List<Node> startingNodes = new List<Node>();
        public List<Node> Nodes;

        private const int CostToMove = 10;

        private int PlayerNumber
        {
            get { return IsHorizontal ? 2 : 1; }
        }

        
        public Pathmonger(int size, bool isHorizontal)
        {
            Size = size;
            
            IsHorizontal = isHorizontal;
            Nodes = new List<Node>();
            for (var i = 0; i < size; i++)
            {
                for (var j = 0; j < size; j++)
                {
                    var node = new Node
                    {
                        H = isHorizontal ? Size - j - 1 : Size - i - 1,
                        Location = {X = i, Y = j},
                        State = NodeState.Untested
                    };
                    Nodes.Add(node);
                }
            }
            Console.WriteLine("Rawr!  I'm the Pathmonger!  I monger all the paths!");
        }
        private bool IsNodeAtBeginning(Node node)
        {
            return IsHorizontal ? node.Location.Y == 0 : node.Location.X == 0;
        }

        private bool IsNodeAtEnd(Node node)
        {
            return IsHorizontal ? node.Location.Y == Size - 1 : node.Location.X == Size - 1;
        }

        public void SetUpAvailableBlocks(List<Hex> hexes, List<Hex> winningPath)
        {
            // hexes is the entire board.  winningPath is the path, however circuitous, that lead to a win.
            foreach (var hex in hexes)
            {
                // For each hex on the board, we want to get the node that is associated with it
                var node = Nodes.FirstOrDefault(x => x.Location.X == hex.X && x.Location.Y == hex.Y);
                if (node != null)
                {
                    // If we found it we want to list its owner as either player 1, player 2 or 0, for unowned
                    node.Owner = hex.Owner?.PlayerNumber ?? 0;
                    node.State = node.Owner == PlayerNumber ?  NodeState.Untested : NodeState.Closed;
                }
            }
            // Start at the beginning and add all taken hexes to the open list
            startingNodes = PlayerNumber == 2
                ? Nodes.Where(node => node.Location.Y == 0 && node.State == NodeState.Untested).ToList()
                : Nodes.Where(node => node.Location.X == 0 && node.State == NodeState.Untested).ToList();
            
            foreach (var node in startingNodes)
            {  
                node.State = NodeState.Open;
            }

            
        }

        public List<Node> Start()
        {
            CurrentRecursion = 0;
            MaxRecursion = Size * Size / 2; // If we don't find a path after searching half of the entire board, we failed
            return FindBestPathFrom(startingNodes.FirstOrDefault());
        }

        public List<Node> FindBestPathFrom(Node currentNode)
        {

            // First, if we've gone too far in the recursion, just die
            if (CurrentRecursion > MaxRecursion)
            {
                Console.WriteLine("OW!  Pathmonger head hurt!  Too many loopies!");
                return null;
            }
            if (currentNode == null)
            {
                Console.WriteLine("RAWR!  Why is u feedin me a NULL?!?");
                return null;
            }

            Console.WriteLine("Yum, feeding on [" +
                              currentNode.Location.X +
                              "," +
                              currentNode.Location.Y +
                              "] g=" +
                              currentNode.G +
                              " h=" +
                              currentNode.H +
                              " f=" +
                              currentNode.F );
            CurrentRecursion++;

            // So we get a list of nodes that are walkable from the current node, and currently untested
            var nextSteps = Nodes.Where(currentNode.IsWalkableFromHere).ToList();

            // Are there next steps?
            if (nextSteps.Any())
            {
                foreach (var step in nextSteps)
                {
                    step.G = currentNode.G + CostToMove;
                    step.ParentNode = currentNode;
                    step.State = NodeState.Open;
                }
                // We put the current node in the closed list
                currentNode.State = NodeState.Closed;
                // We call this again on whichever one is the cheapest
                var hexToGoToNext = nextSteps?.OrderByDescending(x => x.F).FirstOrDefault();
                return FindBestPathFrom(hexToGoToNext);
            } 
            else
            {
                // So if we're here, it's because there are no further nodes to check in this line
                // are we at the end?
                if (IsNodeAtEnd(currentNode))
                {
                    var bestPath = new List<Node>();
                    bestPath.Add(currentNode);
                    var parent = currentNode.ParentNode;
                    while (parent != null)
                    {
                        bestPath.Add(parent);
                        parent = parent.ParentNode;
                    }

                    return bestPath;
                } else
                {
                    // If we haven't reached the end, and we can't go forward, then we need to backtrack
                    // to the best one in the open list
                    currentNode.State = NodeState.Closed;
                    var hexToGoToNext = Nodes.FirstOrDefault(x => x.State == NodeState.Open);
                    // If we still can't find a hex to continue with
                    if (hexToGoToNext == null)
                    {
                        Console.WriteLine("RAWR!  POOPIE!  I don't know where to go from here.");
                        return null;
                    } else
                    {
                        return FindBestPathFrom(hexToGoToNext);
                    }
                }
            }
        }
    }

}
