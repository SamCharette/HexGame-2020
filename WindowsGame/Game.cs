using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using WindowsGame.Hexagonal;

namespace WindowsGame
{
	public partial class Game : Form
	{

		Hexagonal.Board board;
		Hexagonal.GraphicsEngine graphicsEngine;

		public Game()
		{
			InitializeComponent();

			textBoxHexSide.Text = "25";
			textBoxHexBoardHeight.Text = "11";
			textBoxHexBoardWidth.Text = "11";
			textBoxXOffset.Text = "20";
			textBoxtYOffset.Text = "20";
			comboBoxOrientation.SelectedItem = comboBoxOrientation.Items[0];
			textBoxPenWidth.Text = "2";
		}

		private void Form_MouseMove(object sender, MouseEventArgs e)
		{
			labelXY.Text = e.X.ToString() + "," + e.Y.ToString();

		}

		private void buttonTestBoard_Click(object sender, EventArgs e)
		{
			Hexagonal.HexOrientation orientation = HexOrientation.Flat;
			Console.WriteLine(Convert.ToString(comboBoxOrientation.SelectedItem));


			switch (Convert.ToString(comboBoxOrientation.SelectedItem))
			{
				case "Flat":
					orientation = Hexagonal.HexOrientation.Flat;
					break;
				case "Pointy":
					orientation = Hexagonal.HexOrientation.Pointy;
					break;
				default:
					break;
			}

			Board board = new Board(Convert.ToInt32(textBoxHexBoardWidth.Text),
				Convert.ToInt32(textBoxHexBoardHeight.Text),
				Convert.ToInt32(textBoxHexSide.Text),
				 orientation
				);
			board.BoardState.BackgroundColor = Color.Green;
			board.BoardState.GridPenWidth = Convert.ToInt32(textBoxPenWidth.Text);
			board.BoardState.ActiveHexBorderColor = Color.Red;
			board.BoardState.ActiveHexBorderWidth = Convert.ToInt32(textBoxPenWidth.Text);

			this.board = board;
			this.graphicsEngine = new GraphicsEngine(board, Convert.ToInt32(textBoxXOffset.Text), Convert.ToInt32(textBoxtYOffset.Text));
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