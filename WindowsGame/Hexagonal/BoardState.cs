using System.Drawing;

namespace WindowsGame.Hexagonal
{
    public class BoardState
    {

        #region Properties

        public System.Drawing.Color Player1HexColor { get; set; }
        public System.Drawing.Color Player2HexColor { get; set; }
        public System.Drawing.Color BackgroundColor { get; set; }

        public System.Drawing.Color GridColor { get; set; }

        public int GridPenWidth { get; set; }

        public Hex ActiveHex { get; set; }

        public System.Drawing.Color ActiveHexBorderColor { get; set; }

        public int ActiveHexBorderWidth { get; set; }

        #endregion

        public BoardState()
        {
            BackgroundColor = Color.White;
            GridColor = Color.Black;
            GridPenWidth = 1;
            ActiveHex = null;
            ActiveHexBorderColor = Color.Blue;
            ActiveHexBorderWidth = 1;
            Player1HexColor = Color.CornflowerBlue;
            Player2HexColor = Color.LightCoral;
        }
    }
}