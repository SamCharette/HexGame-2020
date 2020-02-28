using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Players.Minimax
{
    public enum AxialDirections
    {
        TopLeft,
        TopRight,
        Right,
        BottomRight,
        BottomLeft,
        Left
    }
    public class MinimaxMap
    {
        public int Size;
        public List<MinimaxNode> Board;

        public MinimaxMap()
        {

        }



        public MinimaxMap(int size)
        {
            Size = size;
            Board = new List<MinimaxNode>(Size * Size);
            for (var column = 0; column < Size; column++)
            {
                for (var row = 0; row < Size; row++)
                {
                    Board.Add(new MinimaxNode(Size, row, column));
                }
            }
        }

        public MinimaxMap(MinimaxMap mapToClone)
        {
            Size = mapToClone.Size;
            Board = new List<MinimaxNode>(Size * Size);
            foreach (var node in mapToClone.Board)
            {
                var newNode = new MinimaxNode(Size, node.Row, node.Column);
                newNode.Owner = node.Owner;
                Board.Add(newNode);
            }
            foreach (var node in mapToClone.Board)
            {
                var clonedNode = Board.FirstOrDefault(x => x.Row == node.Row && x.Column == node.Column);
                foreach (var adjacentNode in node.AdjacencyList)
                {
                    var adjacentClonedNode =
                        Board.FirstOrDefault(x => x.Row == adjacentNode.Row && x.Column == adjacentNode.Column);
                    clonedNode.AdjacencyList.Add(adjacentClonedNode);
                }
            }
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

       

        public bool TakeHex(MinimaxGamePlayer owner, int row, int column)
        {
            var node = Board.FirstOrDefault(x =>
                x.Row == row && x.Column == column && x.Owner == MinimaxGamePlayer.White);
            if (node == null)
            {
                return false;
            }

            node.Owner = owner;
            var listOfNodesToUpdate = new List<MinimaxNode>();
            listOfNodesToUpdate.Add(node);

            // Get and set adjacencies with neighbours
            for (var i = 0; i < 6; i++)
            {
                var delta = Directions[(AxialDirections) i];
                var neighbourNode =
                    Board.FirstOrDefault(x => x.Row == row + delta.Item1 
                                              && x.Column == column + delta.Item2);

                if (neighbourNode != null && neighbourNode.Owner == node.Owner)
                {
                    listOfNodesToUpdate.AddRange(neighbourNode.AdjacencyList.Where(x => !listOfNodesToUpdate.Any(y => y.Row == x.Row && y.Column == x.Column)));
                }

                // And now, if there's more than just the node in the list
                // we can go in and update all of the adjacency lists
                listOfNodesToUpdate.ForEach(x => x.UpdateAdjacencyList(listOfNodesToUpdate));
            }

            return true;
        }
    }
}
