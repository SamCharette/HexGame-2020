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
        public int OwnerNumber = 0;
        public int Column => q;
        public int Row => r;
        public int Height => s;

        
        public Hex()
        {
         
        }

        public Hex(int row, int column)
        {
            r = row;
            q = column;
        }

        public Hex(Tuple<int, int> coordinates)
        {
            r = coordinates.Item1;
            q = coordinates.Item2;
        }

        public Tuple<int,int> ToTuple()
        {
            return new Tuple<int, int>(Row, Column);
        }

        public int DistanceTo(Hex hex)
        {
            return (Math.Abs(q + hex.q) + Math.Abs(r - hex.r) + Math.Abs(q + r - hex.q - hex.r)) / 2;
        }


        
    }
}
