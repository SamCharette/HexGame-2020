using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using WindowsGame.Hexagonal;
using AnimatedGif;
using Engine;
using Engine.Hexagonal;
using Players;
using Board = WindowsGame.Hexagonal.Board;
using Hex = WindowsGame.Hexagonal.Hex;

namespace WindowsGame
{
	public partial class Game : Form
	{

		Board board;
		GraphicsEngine graphicsEngine;
        private Referee referee;
        private List<Bitmap> playThrough;
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
            referee = new Referee(Convert.ToInt32(textBoxHexBoardSize.Text));
			referee.NewGame(Convert.ToInt32(textBoxHexBoardSize.Text));
            referee.AddPlayer(comboBoxPlayer1Type.GetItemText(comboBoxPlayer1Type.SelectedItem), 1);
            referee.AddPlayer(comboBoxPlayer2Type.GetItemText(comboBoxPlayer2Type.SelectedItem), 2);
            StartGame();
        }

		public async void StartGame()
        {
            int boardSize = referee.Size;
            this.lblWInner.Visible = false;
            this.btnSave.Visible = false;
            playThrough = new List<Bitmap>();
			
			board = new Board(boardSize,
                boardSize,
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

            playThrough.Add(graphicsEngine.CreateImage());

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
                
                while (referee.WinningPlayer == null)
                {

                    //Console.WriteLine("Player taking turn: " + referee.CurrentPlayer().PlayerNumber);

					if (referee.lastHexForPlayer1 != null && referee.lastHexForPlayer2 != null)
					{
                        var lastHex = board.Hexes[
                        referee.CurrentPlayer().PlayerNumber == 1
                            ? referee.lastHexForPlayer1.Item1
                            : referee.lastHexForPlayer2.Item1,
                        referee.CurrentPlayer().PlayerNumber == 1
                            ? referee.lastHexForPlayer1.Item2
                            : referee.lastHexForPlayer2.Item2];

                        ChangeHexColor(lastHex,
                            referee.CurrentPlayer().PlayerNumber == 1 ? takenBeforeByPlayer1 : takenBeforeByPlayer2);
                    
					}


                    var hexTaken = await (referee.TakeTurn(referee.CurrentPlayer()));

                    if (hexTaken != null)
                    {
                        //Console.WriteLine("Hex selected was : " + hexTaken.Item1 + ", " + hexTaken.Item2);

                        var boardHex = board.Hexes[hexTaken.Item1, hexTaken.Item2];

                        ChangeHexColor(boardHex, referee.CurrentPlayer().PlayerNumber == 2
                            ? lastTakenByPlayer1
                            : lastTakenByPlayer2);

                        playThrough.Add(graphicsEngine.CreateImage());
                    }

                    this.Refresh();
                }

				// Show the winning path
                var colorForWinningPath = referee.CurrentPlayer().PlayerNumber == 1 ? lastTakenByPlayer1 : lastTakenByPlayer2;
				foreach (var hex in referee.winningPath)
				{
					ChangeHexColor(GetBoardHexFromCoordinates(hex.X, hex.Y), colorForWinningPath);
				}

                this.lblWInner.Text = "The winner is: Player #" + referee.WinningPlayer.PlayerNumber;
                this.lblWInner.Visible = true;
                this.btnSave.Enabled = true;
                this.btnSave.Visible = true;
				this.Refresh();
				Console.WriteLine("The winner is player #" + referee.WinningPlayer.PlayerNumber);
                playThrough.Add(graphicsEngine.CreateImage());
            }
            catch (Exception e)
            {
                Console.WriteLine("No winner today!");

            }
        }

        private void MakeGif(List<Bitmap> frames, string fileName)
        {
            using (var gif = AnimatedGif.AnimatedGif.Create(fileName + ".gif", 175))
            {
                Bitmap lastFrame = frames.FirstOrDefault();
                gif.AddFrame(lastFrame, 750, GifQuality.Bit8);
                foreach (var frame in frames)
                {
                    gif.AddFrame(frame, -1, GifQuality.Bit8);
                    lastFrame = frame;
                }

                gif.AddFrame(lastFrame, 1000, GifQuality.Bit8);
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

				//Console.WriteLine("Click in Board bounding rectangle: {0}", board.PointInBoardRectangle(e.Location));

				Hex clickedHex = board.FindHexMouseClick(mouseClick);

				if (clickedHex == null)
				{
					//Console.WriteLine("No hex was clicked.");
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

        private void btnSave_Click(object sender, EventArgs e)
        {
            // Save the game and the moves.
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                using (XmlWriter writer = XmlWriter.Create(saveFileDialog1.FileName))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("Match");
                    writer.WriteElementString("Date", DateTime.Now.ToShortDateString());
                    writer.WriteElementString("Size", textBoxHexBoardSize.Text);
                    writer.WriteStartElement("Players");
                        writer.WriteStartElement("Player");
                            writer.WriteElementString("Type", referee.Player1.GetType().Name);
                            writer.WriteElementString("Number", "1");
                        writer.WriteEndElement();
                        writer.WriteStartElement("Player");
                            writer.WriteElementString("Type", referee.Player2.GetType().Name);
                            writer.WriteElementString("Number", "2");
                        writer.WriteEndElement();
                    writer.WriteEndElement();
                    writer.WriteStartElement("Moves");
                    int moveNumber = 1;
                    foreach (var move in referee.AllGameMoves)
                    {
                        writer.WriteStartElement("Move");
                        writer.WriteElementString("Number", moveNumber.ToString());
                        writer.WriteElementString("Player", move.player.PlayerNumber.ToString());
                        writer.WriteElementString("X", move.hex.Item1.ToString());
                        writer.WriteElementString("Y", move.hex.Item2.ToString());
                        writer.WriteEndElement();
                        moveNumber++;
                    }
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                    writer.Flush();
                }
                MakeGif(playThrough, saveFileDialog1.FileName);


                btnSave.Enabled = false;
            }
            
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {

         
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {

                    var doc = new XmlDocument();

                    //Load the document with the last book node.
                    var reader = new XmlTextReader(openFileDialog1.OpenFile())
                    {
                        WhitespaceHandling = WhitespaceHandling.None
                    };

                    reader.Read();

                    doc.Load(reader);

                    var matchNode = doc.GetElementsByTagName("Match");
                    var sizeNode = matchNode.Item(0)?.ChildNodes.Item(1);

                    if (sizeNode == null)
                    {
                        Console.WriteLine("The file was not in the proper format.");
                        return;
                    }
                    referee = new Referee(Convert.ToInt32(sizeNode.InnerText));

                    var firstPlayer = new Playback(1, referee.Size);
                    var otherPlayer = new Playback(2, referee.Size);
                    var player1Turn = 1;
                    var player2Turn = 1;
                    var moves = doc.GetElementsByTagName("Move");
                    foreach (XmlNode move in moves)
                    {
                        var player = Convert.ToInt32(move["Player"]?.InnerText);
                        var x = Convert.ToInt32(move["X"]?.InnerText);
                        var y = Convert.ToInt32(move["Y"]?.InnerText);
                        var turnNumber = move["Number"];
                        if (player == 1)
                        {
                            firstPlayer.AddMove(x, y, player1Turn);
                            player1Turn++;
                        }
                        else
                        {
                            otherPlayer.AddMove(x, y, player2Turn);
                            player2Turn++;
                        }
                    }

                    referee.Player1 = firstPlayer;
                    referee.Player2 = otherPlayer;

                    // and feed it the moves
                    StartGame();
                }
                catch (Exception exception)
                {
                    Console.WriteLine("Game couldn't be loaded properly : " + exception.Message);
                    
                }
  
            }
        }
    }
}