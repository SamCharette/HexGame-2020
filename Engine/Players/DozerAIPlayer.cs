using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Engine.GameTypes;
using Engine.Interfaces;

namespace Engine.Players
{
    public class Node
    {
        public int X;
        public int Y;
        public int Owner = 0;
        public Status Status;
        public int G;
        public int H;
        public Node Parent = null;

        public int F => G + H;

        public bool IsAtBeginning(bool isHorizontal)
        {
            return false;
        }
    }

    public enum Status
    {
        Open,
        Closed,
        Untested,
        Impassable
    };
    public class DozerAIPlayer : IPlayer
    {
        public string Name { get; set; }
        public int PlayerNumber { get; set; }

        public List<Node> Nodes = new List<Node>();
        private List<Node> _bestPath = new List<Node>();
        private Board _board;

        private int _costToMove = 20;
        public bool IsHorizontal => PlayerNumber == 2;

        public DozerAIPlayer(int playerNumber)
        {
            PlayerNumber = playerNumber;
        }
        public Hex SelectHex(Board board)
        {
            _board = board;
            SetUpInMemoryBoard(board);
            FindMyWay();

            if (_bestPath.Any())
            {
                // If we have a best path, traverse it to get the next choice on it
                var hexIWant = _bestPath.OrderBy(x => x.F).ThenBy(z => Guid.NewGuid()).FirstOrDefault(y => y.Owner != PlayerNumber);
                if (hexIWant != null)
                {
                    var boardHex = board.HexAt(hexIWant.X, hexIWant.Y);
                    if (boardHex != null)
                    {
                        return boardHex;
                    }
                }
            }

            // If we can't figure anything else out, we need to just return something random
            var openHexes = board.Spaces.Where(x => x.Owner == null);
            var selectedHex = openHexes.OrderBy(x => Guid.NewGuid()).FirstOrDefault();
            return selectedHex;
        }

        private void FindMyWay()
        {
            // Let's look at the open list.  If it's empty, we're done on this path
            if (!Nodes.Any(x => x.Status == Status.Open))
            {
                return;
            }

            // Get the best looking open hex
            var bestLookingNode = Nodes.OrderBy(x => x.F).ThenBy(z => Guid.NewGuid()).FirstOrDefault(y => y.Status == Status.Open);
            // Close the hex
            bestLookingNode.Status = Status.Closed;

            // WAIT!  Did we just close the end node?
            if (_board.IsHexAtEnd(_board.HexAt(bestLookingNode.X, bestLookingNode.Y), IsHorizontal))
            {
                // We have a path
                var parentNode = bestLookingNode.Parent;
                _bestPath.Add(bestLookingNode);
                while (parentNode != null)
                {
                    _bestPath.Add(parentNode);
                    parentNode = parentNode.Parent;
                }

                return;
            }

            // Get all of the neighbours
            var neighbours = _board.GetTraversableNeighbours(bestLookingNode.X, bestLookingNode.Y, this);
            foreach (var neighbour in neighbours)
            {
                // Is it on the open list already?
                var localNode = Nodes.FirstOrDefault(x => x.X == neighbour.X && x.Y == neighbour.Y);
                if (localNode?.Status == Status.Open)
                {
                    if (localNode.G > bestLookingNode.G + _costToMove)
                    {
                        // if it's quicker to get to this node from our current one, update it
                        localNode.Parent = bestLookingNode;
                        localNode.G = bestLookingNode.G + _costToMove;
                    }
                } else if (localNode?.Status == Status.Untested)
                {
                    localNode.Parent = bestLookingNode;
                    localNode.G = bestLookingNode.G + _costToMove;
                    localNode.Status = Status.Open;
                }
            }
            FindMyWay();
        }

        private void SetUpInMemoryBoard(Board board)
        {
            Nodes = new List<Node>();
            _bestPath = new List<Node>();
            foreach (var hex in board.Spaces)
            {
                var node = new Node
                {
                    X = hex.X,
                    Y = hex.Y,
                    Owner = hex.Owner?.PlayerNumber ?? 0
                };
                if (node.Owner != PlayerNumber && node.Owner != 0)
                {
                    node.Status = Status.Impassable;
                }
                else
                {
                    node.Status = board.IsHexAtBeginning(hex, IsHorizontal) ? Status.Open : Status.Untested;
                }

                if (IsHorizontal)
                {
                    node.H = board.Size - 1 - node.Y;
                }
                else
                {
                    node.H = board.Size - 1 - node.X;
                }
                Nodes.Add(node);
                
            }

        }

        
    }
}
