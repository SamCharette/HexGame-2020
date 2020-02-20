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

    public enum AxialDiagonalDirections
    {
        Top,
        TopLeft,
        BottomLeft,
        Bottom,
        BottomRight,
        TopRight
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

        // TO DO This isn't set up correctly for the appropriate value changes
        public Dictionary<AxialDiagonalDirections, Tuple<int, int>> DiagonalDirections = new Dictionary<AxialDiagonalDirections, Tuple<int, int>>()
        {
            { AxialDiagonalDirections.Top, new Tuple<int, int>(0, -1) },
            { AxialDiagonalDirections.TopRight, new Tuple<int, int>(+1, -1) },
            { AxialDiagonalDirections.BottomRight, new Tuple<int, int>(+1, 0) },
            { AxialDiagonalDirections.Bottom, new Tuple<int, int>(0, +1) },
            { AxialDiagonalDirections.BottomLeft, new Tuple<int, int>(-1, +1) },
            { AxialDiagonalDirections.TopLeft, new Tuple<int, int>(-1, 0) }
        };
        public Hex()
        {
         
        }

        public Hex(int row, int column)
        {
            r = row;
            q = column;
        }

        public int DistanceTo(Hex hex)
        {
            return (Math.Abs(q + hex.q) + Math.Abs(r - hex.r) + Math.Abs(q + r - hex.q - hex.r)) / 2;
        }
        public Hex Neighbour(AxialDirections direction)
        {
            var (deltaQ, deltaR) = Directions[direction];
            var neighbour = new Hex {q = q + deltaQ, r = r + deltaR};
            return neighbour;

        }

        
        public Hex Neighbour(AxialDiagonalDirections direction)
        {
            var (deltaQ, deltaR) = DiagonalDirections[direction];
            var neighbour = new Hex { q = q + deltaQ, r = r + deltaR };
            return neighbour;
        }
    }
}
