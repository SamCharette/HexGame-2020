using System;
using System.Drawing;
using Engine.Hexagonal;
using Math = WindowsGame.Hexagonal.Math;

namespace WindowsGame.Hexagonal
{
    /// <summary>
    ///     Represents a 2D hexagon board
    /// </summary>
    public class Board
    {
        private BoardState _boardState;
        private int _height;
        private Hex[,] _hexes;
        private float _pixelHeight;
        private float _pixelWidth;
        private int _width;
        private int _xOffset;
        private int _yOffset;

        /// <param name="width">Board width</param>
        /// <param name="height">Board height</param>
        /// <param name="side">Hexagon side length</param>
        /// <param name="orientation">Orientation of the hexagons</param>
        public Board(int width, int height, int side, HexOrientation orientation)
        {
            Initialize(width, height, side, orientation, 0, 0);
        }

        /// <param name="width">Board width</param>
        /// <param name="height">Board height</param>
        /// <param name="side">Hexagon side length</param>
        /// <param name="orientation">Orientation of the hexagons</param>
        /// <param name="xOffset">X coordinate offset</param>
        /// <param name="yOffset">Y coordinate offset</param>
        public Board(int width, int height, int side, HexOrientation orientation, int xOffset, int yOffset)
        {
            Initialize(width, height, side, orientation, xOffset, yOffset);
        }

        /// <summary>
        ///     Sets internal fields and creates board
        /// </summary>
        /// <param name="width">Board width</param>
        /// <param name="height">Board height</param>
        /// <param name="side">Hexagon side length</param>
        /// <param name="orientation">Orientation of the hexagons</param>
        /// <param name="xOffset">X coordinate offset</param>
        /// <param name="yOffset">Y coordinate offset</param>
        private void Initialize(int width, int height, int side, HexOrientation orientation, int xOffset, int yOffset)
        {
            this._width = width;
            this._height = height;
            this._xOffset = xOffset;
            this._yOffset = yOffset;
            _hexes = new Hex[height, width]; //opposite of what we'd expect
            _boardState = new BoardState();

            var h = Math.CalculateH(side); // short side
            var r = Math.CalculateR(side); // long side

            //
            // Calculate pixel info..remove?
            // because of staggering, need to add an extra r/h
            float hexWidth = 0;
            float hexHeight = 0;

            hexWidth = r + r;
            hexHeight = side + h;
            _pixelWidth = width * hexWidth + width * r;
            _pixelHeight = height * hexHeight + h;


            var inTopRow = false;
            var inBottomRow = false;
            var inLeftColumn = false;
            var inRightColumn = false;
            var isTopLeft = false;
            var isTopRight = false;
            var isBotomLeft = false;
            var isBottomRight = false;


            // i = y coordinate (rows), j = x coordinate (columns) of the hex tiles 2D plane
            for (var i = 0; i < height; i++)
            {
                for (var j = 0; j < width; j++)
                {
                    // Set position booleans
                    inTopRow = i == 0;

                    inBottomRow = i == height - 1;

                    inLeftColumn = j == 0;

                    inRightColumn = j == width - 1;

                    if (inTopRow && inLeftColumn)
                        isTopLeft = true;
                    else
                        isTopLeft = false;

                    if (inTopRow && inRightColumn)
                        isTopRight = true;
                    else
                        isTopRight = false;

                    if (inBottomRow && inLeftColumn)
                        isBotomLeft = true;
                    else
                        isBotomLeft = false;

                    if (inBottomRow && inRightColumn)
                        isBottomRight = true;
                    else
                        isBottomRight = false;



                    //
                    // Calculate Hex positions
                    //
                    if (isTopLeft)
                    {
                        _hexes[0, 0] = new Hex(0 + r + xOffset, 0 + yOffset, side, orientation);
                    }
                    else
                    {

                        if (inLeftColumn)
                        {
       
                            var hex = new Hex(_hexes[i - 1, j].Points[(int) PointyVertice.BottomRight],
                                    side, orientation)
                                {Row = i, Column = j};
                            _hexes[i, j] = hex;

                        }
                        else
                        {
                            // Calculate from Hex to the left
                            var x = _hexes[i, j - 1].Points[(int) PointyVertice.UpperRight].X;
                            var y = _hexes[i, j - 1].Points[(int) PointyVertice.UpperRight].Y;
                            x += r;
                            y -= h;
                            var hex = new Hex(x, y, side, orientation) {Row = i, Column = j};
                            _hexes[i, j] = hex;
                        }

                    }

                }
            }
        }


        public bool PointInBoardRectangle(Point point)
        {
            return PointInBoardRectangle(point.X, point.Y);
        }

        public bool PointInBoardRectangle(int x, int y)
        {
            //
            // Quick check to see if X,Y coordinate is even in the bounding rectangle of the board.
            // Can produce a false positive because of the staggerring effect of hexes around the edge
            // of the board, but can be used to rule out an x,y point.
            //
            var topLeftX = 0 + XOffset;
            var topLeftY = 0 + _yOffset;
            var bottomRightX = topLeftX + _pixelWidth;
            var bottomRightY = topLeftY + PixelHeight;


            if (x > topLeftX && x < bottomRightX && y > topLeftY && y < bottomRightY)
                return true;
            return false;
        }

        public Hex FindHexMouseClick(Point point)
        {
            return FindHexMouseClick(point.X, point.Y);
        }

        public Hex FindHexMouseClick(int x, int y)
        {
            Hex target = null;

            if (PointInBoardRectangle(x, y))
                for (var i = 0; i < _hexes.GetLength(0); i++)
                {
                    for (var j = 0; j < _hexes.GetLength(1); j++)
                        if (Math.InsidePolygon(_hexes[i, j].Points, 6, new PointF(x, y)))
                        {
                            target = _hexes[i, j];
                            break;
                        }

                    if (target != null) break;
                }

            return target;
        }

        #region Properties

        public Hex[,] Hexes
        {
            get => _hexes;
            set { }
        }

        public float PixelWidth
        {
            get => _pixelWidth;
            set { }
        }

        public float PixelHeight
        {
            get => _pixelHeight;
            set { }
        }

        public int XOffset
        {
            get => _xOffset;
            set { }
        }

        public int YOffset
        {
            get => _xOffset;
            set { }
        }

        public int Width
        {
            get => _width;
            set { }
        }

        public int Height
        {
            get => _height;
            set { }
        }


        public BoardState BoardState
        {
            get => _boardState;
            set => throw new NotImplementedException();
        }

        #endregion
    }
}