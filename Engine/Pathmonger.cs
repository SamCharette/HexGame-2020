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
        public NodeState State = NodeState.Untested;
        public Node ParentNode = null;
        public int OwningPlayer;
        
        public bool CanWalkTo(Node neighbour)
        {
            // Can't be a neighbour to itself
            if (neighbour.Location.X == Location.X && neighbour.Location.Y == Location.Y)
            {
                return false;
            }
            // is untested?
            if (neighbour.State == NodeState.Closed)
            {
                return false;
            }
            // Only walkable if we own it
            if (neighbour.OwningPlayer != OwningPlayer)
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
        public List<Node> Nodes;
        public List<Node> FinalPath = new List<Node>();

        private const int CostToMove = 20;

        private int PlayerNumber
        {
            get { return IsHorizontal ? 2 : 1; }
        }

        
        public Pathmonger(int size, bool isHorizontal)
        {
            Size = size;
            
            IsHorizontal = isHorizontal;
            Nodes = new List<Node>();
            for (var x = 0; x < size; x++)
            {
                for (var y = 0; y < size; y++)
                {
                    var node = new Node
                    {
                        H = isHorizontal ? Size - y - 1 : Size - x - 1,
                        Location = {X = x, Y = y}
                    };
                    Nodes.Add(node);
                }
            }
        }
        private bool IsNodeAtBeginning(Node node)
        {
            return IsHorizontal ? node.Location.Y == 0 : node.Location.X == 0;
        }

        private bool IsNodeAtEnd(Node node)
        {
            return IsHorizontal ? node.Location.Y == Size - 1 : node.Location.X == Size - 1;
        }

        public void SetUpAvailableBlocks(List<Hex> hexes)
        {
            // hexes is the entire board.  winningPath is the path, however circuitous, that lead to a win.
            foreach (var hex in hexes)
            {
                // For each hex on the board, we want to get the node that is associated with it
                var node = Nodes.FirstOrDefault(x => x.Location.X == hex.X && x.Location.Y == hex.Y);
                if (node != null)
                {
                    // If we found it we want to list its owner as either player 1, player 2 or 0, for unowned
                    node.OwningPlayer = hex.Owner;
                }
            }
            // Start at the beginning and add all taken hexes to the open list
            var startingNodes = PlayerNumber == 2
                ? Nodes.Where(node => node.Location.Y == 0 && node.State == NodeState.Untested).ToList()
                : Nodes.Where(node => node.Location.X == 0 && node.State == NodeState.Untested).ToList();
            
            foreach (var node in startingNodes)
            {  
                node.State = NodeState.Open;
            }

            
        }

        public void Start()
        {
            CurrentRecursion = 0;
            MaxRecursion = Size * Size / 2; // If we don't find a path after searching half of the entire board, we failed
            FindBestPathFrom();
        }

        public void FindBestPathFrom()
        {
            // Let's grab the cheapest open item
            var currentNode = Nodes.OrderBy(x => x.F).FirstOrDefault(y => y.State == NodeState.Open);

            // nothing open?  THat's an error
            if (currentNode == null)
            {
                Console.WriteLine("An error occurred, and we ran out of open nodes.  No successful path found.");
                return;
            }

            CurrentRecursion++;

            // We put the current node in the closed list
            currentNode.State = NodeState.Closed;

            // If the node we just added to the closed state is at the end, we're done.
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

                FinalPath = bestPath;
                return;
            }

            // So we get a list of nodes that are walkable from the current node, and currently untested
            var nextSteps = Nodes.Where(currentNode.CanWalkTo).ToList();

            // Are there next steps?
            if (nextSteps.Any())
            {
                foreach (var step in nextSteps)
                {
                    // get the actual node in our play area
                    if (step.State == NodeState.Open)
                    {
                        // if the step is already open, we need to check to see if the way to it is better if we go
                        // there from here, so we need to compare its G with what it would be if we moved from here
                        if ((currentNode.G + CostToMove) < step.G)
                        {
                            //  Here we've discovered that it's actually easier to get to this step from where we are
                            // than it is from where we got to it before
                            step.ParentNode = currentNode;
                            step.G = currentNode.G = CostToMove;
                        }
                    }
                    else
                    {
                        step.G = currentNode.G + CostToMove;
                        step.ParentNode = currentNode;
                        step.State = NodeState.Open;
                    }
                }
               
            }
            FindBestPathFrom();

        }
    }

}
