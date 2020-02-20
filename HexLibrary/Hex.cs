using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.InteropServices;

namespace HexLibrary
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

    public class Hex
    {
        public int q; // column
        public int r; // row
        public int s; // z coordinate, if using cube coordinates

        public int Column => q;
        public int Row => r;
        public int Height => s;

        public Dictionary<AxialDirections,Tuple<int,int>> Directions = new Dictionary<AxialDirections, Tuple<int, int>>()
        {
            { AxialDirections.TopLeft, new Tuple<int, int>(0, -1) },
            { AxialDirections.TopRight, new Tuple<int, int>(+1, -1) },
            { AxialDirections.Right, new Tuple<int, int>(+1, 0) },
            { AxialDirections.BottomRight, new Tuple<int, int>(0, +1) },
            { AxialDirections.BottomLeft, new Tuple<int, int>(-1, +1) },
            { AxialDirections.Left, new Tuple<int, int>(-1, 0) }
        };

        public Hex()
        {
         
        }

        public Hex(int row, int column)
        {
            r = row;
            q = column;
        }

        public Hex Neighbour(AxialDirections direction)
        {
            var (deltaQ, deltaR) = Directions[direction];
            var neighbour = new Hex {q = q + deltaQ, r = r + deltaR};
            return neighbour;

        }
    }
}
