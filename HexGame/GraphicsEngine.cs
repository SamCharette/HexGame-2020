using System;
using System.Drawing;
using WindowsGame.Hexagonal;
using Math = System.Math;

namespace WindowsGame
{
    public class GraphicsEngine
    {
        private Board _board;
        private float _boardPixelHeight;
        private float _boardPixelWidth;
        private int _boardXOffset;
        private int _boardYOffset;

        public GraphicsEngine(Board board)
        {
            Initialize(board, 0, 0);
        }

        public GraphicsEngine(Board board, int xOffset, int yOffset)
        {
            Initialize(board, xOffset, yOffset);
        }

        public int BoardXOffset
        {
            get => _boardXOffset;
            set => throw new NotImplementedException();
        }

        public int BoardYOffset
        {
            get => _boardYOffset;
            set => throw new NotImplementedException();
        }

        private void Initialize(Board board, int xOffset, int yOffset)
        {
            this._board = board;
            _boardXOffset = xOffset;
            _boardYOffset = yOffset;
        }

        public void Draw(Graphics graphics)
        {
            graphics.DrawImage(CreateImage(), new Point(_boardXOffset, _boardYOffset));
        }

        public Bitmap CreateImage()
        {
            var width = Convert.ToInt32(Math.Ceiling(_board.PixelWidth));
            var height = Convert.ToInt32(Math.Ceiling(_board.PixelHeight));
            // seems to be needed to avoid bottom and right from being chopped off
            width += 1;
            height += 1;

            //
            // Create drawing objects
            //
            var bitmap = new Bitmap(width, height);
            var bitmapGraphics = Graphics.FromImage(bitmap);
            var p = new Pen(Color.Black);
            var sb = new SolidBrush(Color.Black);


            //
            // Draw Board background
            //
            sb = new SolidBrush(_board.BoardState.BackgroundColor);
            bitmapGraphics.FillRectangle(sb, 0, 0, width, height);

            //
            // Draw Hex Background 
            //
            for (var i = 0; i < _board.Hexes.GetLength(0); i++)
            for (var j = 0; j < _board.Hexes.GetLength(1); j++)
                //bitmapGraphics.DrawPolygon(p, board.Hexes[i, j].Points);
                bitmapGraphics.FillPolygon(new SolidBrush(_board.Hexes[i, j].HexState.BackgroundColor),
                    _board.Hexes[i, j].Points);


            //
            // Draw Hex Grid
            //
            p.Color = _board.BoardState.GridColor;
            p.Width = _board.BoardState.GridPenWidth;
            for (var i = 0; i < _board.Hexes.GetLength(0); i++)
            for (var j = 0; j < _board.Hexes.GetLength(1); j++)
                bitmapGraphics.DrawPolygon(p, _board.Hexes[i, j].Points);

            //
            // Draw Active Hex, if present
            //
            if (_board.BoardState.ActiveHex != null)
            {
                p.Color = _board.BoardState.ActiveHexBorderColor;
                p.Width = _board.BoardState.ActiveHexBorderWidth;
                bitmapGraphics.DrawPolygon(p, _board.BoardState.ActiveHex.Points);
            }

            //
            // Draw internal bitmap to screen
            //
            return bitmap;
        }
    }
}