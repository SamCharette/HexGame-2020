using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Players.Common;

namespace Players.Minimax.List
{
    public enum NodeLocation
    {
        Top,
        Bottom,
        Left,
        Right,
        Board
    }
    public class ListNode
    {
        public int Row;
        public int Column;
        public int BoardSize;
        public Guid RandomValue;
        public List<ListNode> Neighbours;
        public Common.PlayerType Owner = Common.PlayerType.White;
        public int LookAtMe => Touches > 0 ? 1 : 0;

        public int G;
        public int H;
        public Status Status = Status.Untested;
        public ListNode Parent;
        public int Touches = 0;

        public int F => G + H;

        public int RawDistanceToLeft
        {
            get
            {
                if (Location == NodeLocation.Left)
                {
                    return 0;
                }
                else
                {
                    if (Location == NodeLocation.Right)
                    {
                        return BoardSize;
                    }
                    return Column;

                }

            }
        }

        public int RawDistanceToRight
        {
            get
            {
                if (Location == NodeLocation.Right)
                {
                    return 0;
                }
                else
                {
                    if (Location == NodeLocation.Left)
                    {
                        return BoardSize;
                    }
                    return BoardSize - Column;

                }

            }
        }
        public int RawDistanceToTop
        {
            get
            {
                if (Location == NodeLocation.Top)
                {
                    return 0;
                }
                else
                {
                    if (Location == NodeLocation.Bottom)
                    {
                        return BoardSize;
                    }
                    return Row;

                }

            }
        }
        public int RawDistanceToBottom
        {
            get
            {
                if (Location == NodeLocation.Bottom)
                {
                    return 0;
                }
                else
                {
                    if (Location == NodeLocation.Top)
                    {
                        return BoardSize;
                    }
                    return BoardSize - Row;

                }

            }
        }
        public NodeLocation Location { get; set; }

        public ListNode(ListNode sourceNode)
        {
            BoardSize = sourceNode.BoardSize;
            Row = sourceNode.Row;
            Column = sourceNode.Column;
            G = sourceNode.G;
            H = sourceNode.H;
            Status = sourceNode.Status;
            Owner = sourceNode.Owner;
            Location = NodeLocation.Board;
            Neighbours = new List<ListNode>(BoardSize * BoardSize);
        }


        public ListNode(int Size, int row, int column)
        {
            BoardSize = Size;
            Row = row;
            Column = column;
            Neighbours = new List<ListNode>(BoardSize * BoardSize);
            Location = NodeLocation.Board;
            RandomValue = Guid.NewGuid();
        }
        private bool IsInsideBoard()
        {
            return Location == NodeLocation.Board;
        }
        public void PingNeighbours(bool sayHi = true)
        {
            foreach (var neighbour in Neighbours)
            {
                if (neighbour.IsInsideBoard())
                {
                    if (sayHi)
                    {
                        neighbour.Touches++;
                    }
                    else
                    {
                        neighbour.Touches--;
                    }
                }
            }
        }

        public bool IsNeighboursWith(ListNode neighbour)
        {
            return Neighbours.Any(x => x.Row == neighbour.Row && x.Column == neighbour.Column) ;
        }
        public void AttachTo(ListNode neighbour)
        {
            if (!Neighbours.Any(x => x.Row == neighbour.Row && x.Column == neighbour.Column))
            {
                Neighbours.Add(neighbour);
            }
        }
        public void DetachFrom(ListNode neighbour)
        {
            var neighbour1 = Neighbours.FirstOrDefault(x => x.Row == neighbour.Row && x.Column == neighbour.Column);
            if (neighbour1 != null)
            {
                Neighbours.Remove(neighbour1);
            }
            
        }
        
        public int RemainingDistance()
        {
            var bestStartNode = Neighbours.OrderBy(x => x.GetDistanceToStart())
                .FirstOrDefault(x => x.Owner == Owner) ?? this;
            var bestEndNode = Neighbours.OrderBy(x => x.GetDistanceToEnd())
                                  .FirstOrDefault(x => x.Owner == Owner) ?? this;

            return bestStartNode.GetDistanceToStart() + bestEndNode.GetDistanceToEnd();
        }

        public int GetDistanceToStart()
        {
            if (Owner == Common.PlayerType.Blue)
            {
                return GetDistanceToTop();
            }

            if (Owner == Common.PlayerType.Red)
            {
                return GetDistanceToLeft();
            }

            return -1;
        }

        public int GetDistanceToEnd()
        {
            if (Owner == Common.PlayerType.Blue)
            {
                return GetDistanceToBottom();
            }

            if (Owner == Common.PlayerType.Red)
            {
                return GetDistanceToRight();
            }

            return -1;
        }

        public int GetDistanceToTop()
        {
            // Using the adjacency graph, let's find the node connected that is closest 
            // to the top, as we are essentially that close from here because we are connected

            var bestNode = Neighbours.OrderBy(x => x.RawDistanceToTop)
                .FirstOrDefault(x => x.Owner == Owner) ?? this;
            return bestNode.RawDistanceToTop;
        }
        public int GetDistanceToLeft()
        {
            var bestNode = Neighbours.OrderBy(x => x.RawDistanceToLeft)
                .FirstOrDefault(x => x.Owner == Owner) ?? this;
            return bestNode.RawDistanceToLeft;
        }
        public int GetDistanceToBottom()
        {
            // Using the adjacency graph, let's find the node connected that is closest 
            // to the top, as we are essentially that close from here because we are connected
            var bestNode = Neighbours.OrderBy(x => x.RawDistanceToBottom)
                .FirstOrDefault(x => x.Owner == Owner) ?? this;
            return bestNode.RawDistanceToBottom;
        }

        public int GetDistanceToRight()
        {
            // Using the adjacency graph, let's find the node connected that is closest 
            // to the top, as we are essentially that close from here because we are connected
            var bestNode = Neighbours.OrderBy(x => x.RawDistanceToRight)
                .FirstOrDefault(x => x.Owner == Owner) ?? this;
            return bestNode.RawDistanceToRight;
        }


    }
}