using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using Engine;
using Engine.Hexagonal;
using Board = Engine.Hexagonal.Board;

namespace WindowsGame
{
	public partial class Game : Form
	{

		Board board;
		GraphicsEngine graphicsEngine;
        private Referee referee;

		public Game()
		{
			InitializeComponent();

			textBoxHexBoardSize.Text = "11";
			comboBoxPlayer2Type.SelectedItem = comboBoxPlayer2Type.Items[0];
		}

		public void Play()
        {
            referee = new Referee(Convert.ToInt32(textBoxHexBoardSize.Text));
			referee.NewGame();

			board = new Board(Convert.ToInt32(textBoxHexBoardSize.Text),
                Convert.ToInt32(textBoxHexBoardSize.Text),
                25,
                HexOrientation.Pointy
            )
            {
                BoardState =
                {
                    BackgroundColor = Color.Green,
                    GridPenWidth = 2,
                    ActiveHexBorderColor = Color.Red,
                    ActiveHexBorderWidth = 2
                }
            };

            graphicsEngine = new GraphicsEngine(board, 20, 20);

			try
			{
                while (referee.Winner() == false)
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
                            referee.CurrentPlayer().PlayerNumber == 1 ? Color.DeepSkyBlue : Color.LightCoral);
                    
					}
					
                    var hexTaken = referee.TakeTurn(referee.CurrentPlayer());
                    if (hexTaken != null)
                    {
                        Console.WriteLine("Hex selected was : " + hexTaken.X + ", " + hexTaken.Y);

						var boardHex = board.Hexes[hexTaken.X, hexTaken.Y];

						ChangeHexColor(boardHex, referee.CurrentPlayer().PlayerNumber == 1
                            ? Color.Blue
                            : Color.Red);

                    }

                    this.Refresh();
                }

				// Show the winning path
                var colorForWinningPath = referee.CurrentPlayer().PlayerNumber == 1 ? Color.Blue : Color.Red;
				foreach (var hex in referee.winningPath)
				{
					ChangeHexColor(GetBoardHexFromCoordinates(hex.X, hex.Y), colorForWinningPath);
				}

				Console.WriteLine("The winner is player #" + referee.CurrentPlayer().PlayerNumber);
//                MessageBox.Show(this, "The winner is player #" + referee.CurrentPlayer().PlayerNumber);
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
			labelXY.Text = e.X.ToString() + "," + e.Y.ToString();

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
;
					board.BoardState.ActiveHex = clickedHex;
					if (e.Button == MouseButtons.Right)
					{
						clickedHex.HexState.BackgroundColor = Color.Blue;
					}
				}

			}
		}

		private void Form_Paint(object sender, PaintEventArgs e)
		{
			//Draw the graphics/GUI

			foreach (Control c in this.Controls)
			{
				c.Refresh();
			}

			if (graphicsEngine != null)
			{
				graphicsEngine.Draw(e.Graphics);
			}

			//Force the next Paint()
			this.Invalidate();

		}

		private void Form_Closing(object sender, FormClosingEventArgs e)
		{
			if (graphicsEngine != null)
			{
				graphicsEngine = null;
			}

			if (board != null)
			{
				board = null;
			}
		}


	}
}