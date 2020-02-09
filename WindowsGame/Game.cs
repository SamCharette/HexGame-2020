using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using WindowsGame.Hexagonal;
using Engine;
using Engine.Hexagonal;
using Board = WindowsGame.Hexagonal.Board;

namespace WindowsGame
{
	public partial class Game : Form
	{

		Board board;
		GraphicsEngine graphicsEngine;
        private Referee referee;
        private Color emptyColor = Color.White;
        private Color emptyBlueSide = Color.Azure;
        private Color emptyRedSide = Color.MistyRose;
        private Color emptyCorner = Color.Plum;
        private Color takenBeforeByPlayer1 = Color.DeepSkyBlue;
        private Color lastTakenByPlayer1 = Color.Blue;
        private Color takenBeforeByPlayer2 = Color.LightCoral;
        private Color lastTakenByPlayer2 = Color.Red;
        private Color backgroundColor = Color.Green;


        public Game()
		{
			InitializeComponent();

			textBoxHexBoardSize.Text = "11";
            comboBoxPlayer1Type.SelectedItem = comboBoxPlayer1Type.Items[0];
            comboBoxPlayer2Type.SelectedItem = comboBoxPlayer2Type.Items[0];
		}

		public async void Play()
        {
            int boardSize = Convert.ToInt32(textBoxHexBoardSize.Text);
            this.lblWInner.Visible = false;
			
            referee = new Referee(Convert.ToInt32(textBoxHexBoardSize.Text));
			referee.NewGame(Convert.ToInt32(textBoxHexBoardSize.Text));
            referee.CreatePlayer(comboBoxPlayer1Type.GetItemText(comboBoxPlayer1Type.SelectedItem), 1);
            referee.CreatePlayer(comboBoxPlayer2Type.GetItemText(comboBoxPlayer2Type.SelectedItem), 2);

			board = new Board(Convert.ToInt32(textBoxHexBoardSize.Text),
                Convert.ToInt32(textBoxHexBoardSize.Text),
                25,
                HexOrientation.Pointy
            )
            {
                BoardState =
                {
                    BackgroundColor = backgroundColor,
                    GridPenWidth = 2,
                    ActiveHexBorderColor = Color.Red,
                    ActiveHexBorderWidth = 2
                }
            };

            graphicsEngine = new GraphicsEngine(board, 20, 20);


            // make the edges colourful!
            foreach (var hex in board.Hexes)
            {
                if (hex.Row == 0 || hex.Row == boardSize - 1)
                {
                    ChangeHexColor(hex, emptyBlueSide);
                }
                if (hex.Column == 0 || hex.Column == boardSize - 1)
                {
                    ChangeHexColor(hex, emptyRedSide);
                }
                if (hex.Column == 0 && hex.Row == 0 || hex.Column == 0 && hex.Row == boardSize - 1
                    || hex.Column == boardSize -1 && hex.Row == 0 || hex.Column == boardSize - 1 && hex.Row == boardSize - 1)
                {
                    ChangeHexColor(hex, emptyCorner);
                }
            }

            this.Refresh();

			try
            {
                var isThereAWinnor = false;
                while (!isThereAWinnor)
                {

                    Console.WriteLine("Player taking turn: " + referee.CurrentPlayer().PlayerNumber);

					if (referee.lastHexForPlayer1 != null && referee.lastHexForPlayer2 != null)
					{
                        var lastHex = board.Hexes[
                        referee.CurrentPlayer().PlayerNumber == 1
                            ? referee.lastHexForPlayer1.X
                            : referee.lastHexForPlayer2.X,
                        referee.CurrentPlayer().PlayerNumber == 1
                            ? referee.lastHexForPlayer1.Y
                            : referee.lastHexForPlayer2.Y];

                        ChangeHexColor(lastHex,
                            referee.CurrentPlayer().PlayerNumber == 1 ? takenBeforeByPlayer1 : takenBeforeByPlayer2);
                    
					}


                    var hexTaken = await (referee.TakeTurn(referee.CurrentPlayer()));
                    //hexTaken = referee.TakeTurn(referee.CurrentPlayer());

                    if (hexTaken != null)
                    {
                        Console.WriteLine("Hex selected was : " + hexTaken.X + ", " + hexTaken.Y);

						var boardHex = board.Hexes[hexTaken.X, hexTaken.Y];

						ChangeHexColor(boardHex, referee.CurrentPlayer().PlayerNumber == 1
                            ? lastTakenByPlayer1
                            : lastTakenByPlayer2);


                        isThereAWinnor = referee.Winner();

                    }

                    this.Refresh();
                }

				// Show the winning path
                var colorForWinningPath = referee.CurrentPlayer().PlayerNumber == 1 ? lastTakenByPlayer1 : lastTakenByPlayer2;
				foreach (var hex in referee.winningPath)
				{
					ChangeHexColor(GetBoardHexFromCoordinates(hex.X, hex.Y), colorForWinningPath);
				}

                this.lblWInner.Text = "The winner is: Player #" + referee.CurrentPlayer().PlayerNumber;
                this.lblWInner.Visible = true;
				this.Refresh();
				Console.WriteLine("The winner is player #" + referee.CurrentPlayer().PlayerNumber);
            }
            catch (Exception e)
            {
                Console.WriteLine("No winner today!");

            }
        }

		private Hex GetBoardHexFromCoordinates(int X, int Y)
        {
            return board.Hexes[X, Y];
		}

		private void ChangeHexColor(Hex hex, Color color)
		{
			if (hex != null )
			{
				hex.HexState.BackgroundColor = color;
			}
		}

		private void Form_MouseMove(object sender, MouseEventArgs e)
        {
            if (board != null)
            {
                var hexHoveringOver = board.FindHexMouseClick(e.X - graphicsEngine.BoardXOffset, e.Y - graphicsEngine.BoardYOffset);
                if (hexHoveringOver != null)
                {
                    var label = "[" + hexHoveringOver.Row + "," + hexHoveringOver.Column + "]";
                    if (hexHoveringOver.HexState.BackgroundColor == lastTakenByPlayer1 || hexHoveringOver.HexState.BackgroundColor == takenBeforeByPlayer1)
                    {
                        label += " Player 1";
                    }
                    else if(hexHoveringOver.HexState.BackgroundColor == lastTakenByPlayer2 || hexHoveringOver.HexState.BackgroundColor == takenBeforeByPlayer2)
                    {
                        label += " Player 2";
                    }
                    else
                    {
                        label += " not owned";
                    }
                    labelXY.Text =label;
                }
			    else
                {
                    labelXY.Text = "";
                }
            }
           

		}
        private void buttonTestBoard_Click(object sender, EventArgs e)
		{
			Play();
		}


		private void Form_MouseClick(object sender, MouseEventArgs e)
		{

			Console.WriteLine("Mouse Click " + e.Location.ToString());

			if (board != null && graphicsEngine != null)
			{
				//
				// need to account for any offset
				//
				Point mouseClick = new Point(e.X - graphicsEngine.BoardXOffset, e.Y - graphicsEngine.BoardYOffset);

				Console.WriteLine("Click in Board bounding rectangle: {0}", board.PointInBoardRectangle(e.Location));

				Hex clickedHex = board.FindHexMouseClick(mouseClick);

				if (clickedHex == null)
				{
					Console.WriteLine("No hex was clicked.");
					board.BoardState.ActiveHex = null;

				}
				else
				{
					Console.WriteLine("Hex was clicked: [" + clickedHex.Row + "," + clickedHex.Column + "]");

                    referee.ClickOnHexCoords(clickedHex.Row, clickedHex.Column);
                }

			}
		}

		private void Form_Paint(object sender, PaintEventArgs e)
        {
            //Draw the graphics/GUI
            graphicsEngine?.Draw(e.Graphics);
        }

		private void Form_Closing(object sender, FormClosingEventArgs e)
		{
			graphicsEngine = null;
			board = null;
        }
    }
}