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
            // We're going to get a list of hexes that represents a winning path.
            // This will mark those, and only those, as available to be used.
            foreach (var hex in hexes)
            {
                var node = Nodes.FirstOrDefault(x => x.Location.X == hex.X && x.Location.Y == hex.Y);
                if (node != null)
                {
                    node.Owner = hex.Owner?.PlayerNumber ?? 0;
                }
            }
            // Start at the beginning and add all taken hexes to the open list
            startingNodes = PlayerNumber == 1
                ? Nodes.Where(x => x.Location.X == 0)
                    .ToList()
                : Nodes.Where(x => x.Location.Y == 0)
                    .ToList();


            foreach (var node in startingNodes)
            {
                node.State = NodeState.Open;
            }

            // Let's just start with one of the nodes at the edge of the board.  There is an "edge" case 
            // whereby there could be more than one on the edge of the board in the same path, and
            // this could select the farther one.
            
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
                return null;
            }

            CurrentRecursion++;

            // A* stuff goes here

    
           
            // Otherwise

            // So we get a list of nodes that are walkable from the current node, and currently untested
            var nextSteps = Nodes.Where(currentNode.IsWalkableFromHere).ToList();

            foreach (var step in nextSteps)
            {
                step.G = currentNode.G + 1;
                step.ParentNode = currentNode;
                step.State = NodeState.Open;
            }

            // We put the current node in the closed list
            currentNode.State = NodeState.Closed;
            // We call this again on whichever one is the cheapest
            var hexToGoToNext = nextSteps?.OrderByDescending(x => x.F).FirstOrDefault();
            if (hexToGoToNext == null)
            {
                return null;
            }

            var futureValue = FindBestPathFrom(hexToGoToNext);

            if (futureValue == null)
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
            }

            return futureValue;
        }
    }

    //private List<Hex> FindBestPathIn(List<Hex> path, bool isHorizontal)
    //{

    //    var pathStart = isHorizontal
    //        ? path.FirstOrDefault(x => x.Y == 0)
    //        : path.FirstOrDefault(x => x.X == 0);


    //    var openList = new List<Hex> { pathStart };
    //    var closedList = new List<Hex>();

    //    var bestPath = FindNextPathHexes(ref openList, ref closedList, ref pathStart, isHorizontal);

    //    return bestPath;
    //}

    //private List<Hex> FindNextPathHexes(ref List<Hex> open, ref List<Hex> closed, ref Hex current, bool isHorizontal)
    //{
    //    bool isAtTheEnd = (isHorizontal && current.Y == Size - 1) || (!isHorizontal && current.X == Size - 1);

    //    // First, let's see if we're at the end
    //    if (isAtTheEnd)
    //    {
    //        var bestPath = new List<Hex>();
    //        var nextHex = current;
    //        while (nextHex.ParentHex == null)
    //        {
    //            bestPath.Add(nextHex);
    //            nextHex = nextHex.ParentHex;
    //        }

    //        return bestPath;
    //    }

    //    // Not at the end?  Continue on then
    //    var fullListOfNeighbours = Board.GetFriendlyNeighbours(current.X, current.Y, CurrentPlayer());

    //    var closedList = closed;

    //    var listOfNeighbours = fullListOfNeighbours.Where(x => closedList.All(y => y != x));

    //    foreach (var hex in listOfNeighbours)
    //    {
    //        hex.ParentHex = current;
    //        hex.CostInPath = current.CostInPath + 1;
    //        closed.Add(current);
    //        open.Remove(current);
    //        open.Add(hex);
    //    }

    //    var bestGuessHex = open.OrderByDescending(x => x.TotalCost(isHorizontal, Size)).FirstOrDefault();



    //    return FindNextPathHexes(ref open, ref closed, ref bestGuessHex, isHorizontal);
    //}

    //}
}
