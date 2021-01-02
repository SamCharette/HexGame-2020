using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MonteCarloPlayer.Board
{
    public class Board
    {
        public int Size { get; set; }
        public List<Vertex> Hexes { get; set; }
        public List<Edge> Connections { get; set; }
        public Vertex Top { get; set; }
        public Vertex Bottom { get; set; }
        public Vertex Left { get; set; }
        public Vertex Right { get; set; }

        public Board()
        {

        }

        public Board(int size)
        {
            Size = size;
            Top = new Vertex();
            Bottom = new Vertex();
            Left = new Vertex();
            Right = new Vertex();
            SetupBoard(Size);
        }

        public void SetupBoard(int size)
        {
            CreateVertices(size);
        }

        private void CreateEdges()
        {
            foreach (var hex in Hexes)
            {
                // Get hex neighbours
                // Create the edges between the hex and the neighbours
            }
        }

        private void CreateVertices(int size)
        {
            for (var row = 0; row < size; row++)
            {
                for (var column = 0; column < size; column++)
                {
                    var hex = new Vertex(row, column);
                    if (row == 0)
                    {
                        ConnectVertices(hex, Top);
                    }

                    if (row == Size - 1)
                    {
                        ConnectVertices(hex, Bottom);
                    }

                    if (column == 0)
                    {
                        ConnectVertices(hex, Left);
                    }

                    if (column == Size - 1)
                    {
                        ConnectVertices(hex, Right);
                    }
                    Hexes.Add(hex);
                }
            }
        }

        private void ConnectVertices(Vertex one, Vertex two)
        {
            var edge = new Edge {In = one, Out = two};
            var edge2 = new Edge {In = two, Out = one};
            Connections.Add(edge);
            Connections.Add(edge2);
        }
    }
}
