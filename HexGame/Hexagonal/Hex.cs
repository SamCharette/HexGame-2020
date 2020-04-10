using System;
using System.Drawing;

namespace WindowsGame.Hexagonal
{
    public class Hex
    {
        public int Column;
        private float _h;
        private HexState _hexState;
        private HexOrientation _orientation;
        private PointF[] _points;
        private float _r;
        public int Row;
        private float _side;

        public Hex(int x, int y, int side, HexOrientation orientation)
        {
            Initialize(Math.ConvertToFloat(x), Math.ConvertToFloat(y), Math.ConvertToFloat(side), orientation);
        }

        public Hex(float x, float y, float side, HexOrientation orientation)
        {
            Initialize(x, y, side, orientation);
        }

        public Hex(PointF point, float side, HexOrientation orientation)
        {
            Initialize(point.X, point.Y, side, orientation);
        }

        public Hex()
        {
        }


        public PointF[] Points
        {
            get => _points;
            set { }
        }

        public float Side
        {
            get => _side;
            set { }
        }

        public float H
        {
            get => _h;
            set { }
        }

        public float R
        {
            get => _r;
            set { }
        }

        public float X { get; private set; }

        public float Y { get; private set; }

        public HexState HexState
        {
            get => _hexState;
            set => throw new NotImplementedException();
        }

        /// <summary>
        ///     Sets internal fields and calls CalculateVertices()
        /// </summary>
        private void Initialize(float x, float y, float side, HexOrientation orientation)
        {
            X = x;
            Y = y;
            this._side = side;
            this._orientation = orientation;
            _hexState = new HexState();
            CalculateVertices();
        }

        /// <summary>
        ///     Calculates the vertices of the hex based on orientation. Assumes that points[0] contains a value.
        /// </summary>
        private void CalculateVertices()
        {
            //  
            //  h = short length (outside)
            //  r = long length (outside)
            //  side = length of a side of the hexagon, all 6 are equal length
            //
            //  h = sin (30 degrees) x side
            //  r = cos (30 degrees) x side
            //
            //		 h
            //	     ---
            //   ----     |r
            //  /    \    |          
            // /      \   |
            // \      /
            //  \____/
            //
            // Flat orientation (scale is off)
            //
            //     /\
            //    /  \
            //   /    \
            //   |    |
            //   |    |
            //   |    |
            //   \    /
            //    \  /
            //     \/
            // Pointy orientation (scale is off)

            _h = Math.CalculateH(_side);
            _r = Math.CalculateR(_side);

            //x,y coordinates are top center point
            _points = new PointF[6];
            _points[0] = new PointF(X, Y);
            _points[1] = new PointF(X + _r, Y + _h);
            _points[2] = new PointF(X + _r, Y + _side + _h);
            _points[3] = new PointF(X, Y + _side + _h + _h);
            _points[4] = new PointF(X - _r, Y + _side + _h);
            _points[5] = new PointF(X - _r, Y + _h);
        }
    }
}