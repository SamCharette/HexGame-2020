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
using Newtonsoft.Json;
using Players.Common;

namespace WindowsGame
{
    public partial class Game : Form
    {
        private readonly Color _backgroundColor = Color.Green;

        private Board _board;
        private GraphicsEngine _graphicsEngine;
        private List<Bitmap> _playThrough;
        private Referee _referee;
        private Hex _lastTakenByPlayer1;
        private Hex _lastTakenByPlayer2;
        private readonly Color _takenBeforeByPlayer1 = Color.DeepSkyBlue;
        private readonly Color _takenBeforeByPlayer2 = Color.LightCoral;
        private readonly Color _emptyBlueSide = Color.Azure;
        private readonly Color _emptyCorner = Color.Plum;
        private readonly Color _emptyRedSide = Color.MistyRose;
        private readonly Color _lastTakenByPlayer1Colour = Color.Blue;
        private readonly Color _lastTakenByPlayer2Colour = Color.Red;
        private List<Config> _playerConfigs;


        public Game()
        {
     
            InitializeComponent();
            SetUpPlayers();
        }

        private void SetUpPlayers()
        {
            comboBoxPlayer1Type.Items.Clear();
            comboBoxPlayer2Type.Items.Clear();
            player1Metrics.Text = "";
            player2Metrics.Text = "";
            var appPath = Application.StartupPath;
            var configPath = Path.Combine(appPath, "Config\\players.json");
            _playerConfigs = JsonConvert.DeserializeObject<List<Config>>(File.ReadAllText(configPath));

            int count = 0;
            foreach (var player in _playerConfigs)
            {
                comboBoxPlayer1Type.Items.Add(player.name);
                if (player.playerNumber == "1")
                {
                    comboBoxPlayer1Type.SelectedItem = comboBoxPlayer1Type.Items[count];
                }
                comboBoxPlayer2Type.Items.Add(player.name);
                if (player.playerNumber == "2")
                {
                    comboBoxPlayer2Type.SelectedItem = comboBoxPlayer2Type.Items[count];
                }

                count++;
            }

            textBoxHexBoardSize.Text = @"11";

        }
        public void PerformanceInformationRelayed(object sender, EventArgs args)
        {
            var playerArgs = (PerformanceEventArgs) args;
            var playerMetricLabel = playerArgs.PlayerNumber == 1 ? player1Metrics : player2Metrics;

            var textToShow = "";
            foreach (var counter in playerArgs.Counters)
            {
                textToShow = textToShow + counter.Key + " : " + counter.Value + "\n";
            }
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() =>
                {
                    playerMetricLabel.Text = textToShow;
                    Refresh();

                }));
            }
            else
            {
                playerMetricLabel.Text = textToShow;
            }



        }
        public void Play()
        {
            _referee = new Referee(Convert.ToInt32(textBoxHexBoardSize.Text));
            //_referee.GameOver += GameOver;
            //_referee.PlayerMadeMove += PlayerMadeMove;
            _referee.NewGame(Convert.ToInt32(textBoxHexBoardSize.Text));
            _referee.AddPlayer(_playerConfigs.FirstOrDefault(x => x.name == comboBoxPlayer1Type.SelectedItem), 1);
            _referee.Player1.RelayInformation += PerformanceInformationRelayed;
            _referee.AddPlayer(_playerConfigs.FirstOrDefault(x => x.name == comboBoxPlayer2Type.SelectedItem), 2);
            _referee.Player2.RelayInformation += PerformanceInformationRelayed;
            StartGame();
        }


        public async void StartGame()
        {
            buttonTestBoard.Enabled = false;
            var boardSize = _referee.Size;
            saveGameToolStripMenuItem.Enabled = false;
            loadGameToolStripMenuItem.Enabled = false;
            reloadConfigurationToolStripMenuItem.Enabled = false;
            _playThrough = new List<Bitmap>();

            _board = new Board(boardSize,
                boardSize,
                25,
                HexOrientation.Pointy
            )
            {
                BoardState =
                {
                    BackgroundColor = _backgroundColor,
                    GridPenWidth = 2,
                    ActiveHexBorderColor = Color.Red,
                    ActiveHexBorderWidth = 2
                }
            };

            _graphicsEngine = new GraphicsEngine(_board, 20, 20);


            Refresh();
            _playThrough.Add(_graphicsEngine.CreateImage());

            // make the edges colourful!
            foreach (var hex in _board.Hexes)
            {
                if (hex.Row == 0 || hex.Row == boardSize - 1)
                {
                    ChangeHexColor(hex, _emptyBlueSide);
                }
                if (hex.Column == 0 || hex.Column == boardSize - 1)
                {
                    ChangeHexColor(hex, _emptyRedSide);
                }
                if (hex.Column == 0 && hex.Row == 0 || hex.Column == 0 && hex.Row == boardSize - 1
                    || hex.Column == boardSize - 1 && hex.Row == 0 || hex.Column == boardSize - 1 && hex.Row == boardSize - 1)
                {
                    ChangeHexColor(hex, _emptyCorner);
                }
            }

       

            try
            {

                while (_referee.WinningPlayer == null)
                {

                    //Console.WriteLine("Player taking turn: " + referee.CurrentPlayer().PlayerNumber);

                    if (_referee.lastHexForPlayer1 != null && _referee.lastHexForPlayer2 != null)
                    {
                        var lastHex = _board.Hexes[
                        _referee.CurrentPlayer().PlayerNumber == 1
                            ? _referee.lastHexForPlayer1.Item1
                            : _referee.lastHexForPlayer2.Item1,
                        _referee.CurrentPlayer().PlayerNumber == 1
                            ? _referee.lastHexForPlayer1.Item2
                            : _referee.lastHexForPlayer2.Item2];

                        ChangeHexColor(lastHex,
                            _referee.CurrentPlayer().PlayerNumber == 1 ? _takenBeforeByPlayer1 : _takenBeforeByPlayer2);

                    }


                    var hexTaken = await (_referee.TakeTurn(_referee.CurrentPlayer()));

                    if (hexTaken != null)
                    {
                        var boardHex = _board.Hexes[hexTaken.Item1, hexTaken.Item2];

                        if (_referee.WinningPlayer != null)
                        {
                            ChangeHexColor(boardHex, _referee.CurrentPlayer().PlayerNumber == 2
                            ? _lastTakenByPlayer2Colour
                            : _lastTakenByPlayer1Colour);
                        } else
                        {
                            ChangeHexColor(boardHex, _referee.CurrentPlayer().PlayerNumber == 1
                                ? _lastTakenByPlayer2Colour
                                : _lastTakenByPlayer1Colour);
                        }
                        

                        _playThrough.Add(_graphicsEngine.CreateImage());
                    }

                    this.Refresh();
                }

                // Show the winning path
                var colorForWinningPath = _referee.CurrentPlayer().PlayerNumber == 1 ? _lastTakenByPlayer1Colour : _lastTakenByPlayer2Colour;
                foreach (var hex in _referee.winningPath)
                {
                    ChangeHexColor(GetBoardHexFromCoordinates(hex.Row, hex.Column), colorForWinningPath);
                }

      
                this.Refresh();
                Console.WriteLine("The winner is " + _referee.WinningPlayer.Name + ", player #" + _referee.WinningPlayer.PlayerNumber);
                _playThrough.Add(_graphicsEngine.CreateImage());
                buttonTestBoard.Enabled = true;
                MessageBox.Show("The winner is " + _referee.WinningPlayer.Name + ", player #" + _referee.WinningPlayer.PlayerNumber, "Winner!");
            }
            catch (Exception e)
            {
                MessageBox.Show("No winner today!", "Drat");
                Console.WriteLine("No winner today!");

            }
            saveGameToolStripMenuItem.Enabled = true;
            loadGameToolStripMenuItem.Enabled = true;
            reloadConfigurationToolStripMenuItem.Enabled = true;
        }

        public void PlayerMadeMove(object sender, EventArgs args)
        {
            PlayerMadeMoveArgs moveArgs = (PlayerMadeMoveArgs) args;
            if (moveArgs != null)
            {
                var boardHex = _board.Hexes[moveArgs.move.Item1, moveArgs.move.Item2];
                if (moveArgs.player == 1)
                {
                    UpdatePlayer1Moves(boardHex);
                }
                else
                {
                    UpdatePlayer2Moves(boardHex);
                }

            }

            Refresh();

            _playThrough.Add(_graphicsEngine.CreateImage());
        }

        private void UpdatePlayer1Moves(Hex boardHex)
        {
            ChangeHexColor(_lastTakenByPlayer1,  _takenBeforeByPlayer1);
            _lastTakenByPlayer1 = boardHex;
            ChangeHexColor(_lastTakenByPlayer1, _lastTakenByPlayer1Colour);

        }
        private void UpdatePlayer2Moves(Hex boardHex)
        {
            ChangeHexColor(_lastTakenByPlayer2, _takenBeforeByPlayer2);
            _lastTakenByPlayer2 = boardHex;
            ChangeHexColor(_lastTakenByPlayer2, _lastTakenByPlayer2Colour);

        }

        public void GameOver(object sender, EventArgs args)
        {
            // Show the winning path
            var colorForWinningPath =
                _referee.CurrentPlayer().PlayerNumber == 1 ? _lastTakenByPlayer1Colour : _lastTakenByPlayer2Colour;
            foreach (var hex in _referee.winningPath)
                ChangeHexColor(GetBoardHexFromCoordinates(hex.Row, hex.Column), colorForWinningPath);

            MessageBox.Show(@"The winner is: Player #" + _referee.WinningPlayer.PlayerNumber);

            saveGameToolStripMenuItem.Enabled = true;
            Refresh();
            // Clear up the memory for the ref
            Console.WriteLine(@"The winner is player #" + _referee.WinningPlayer.PlayerNumber);
            _playThrough.Add(_graphicsEngine.CreateImage());
            buttonTestBoard.Enabled = true;
        }

        private void MakeGif(List<Bitmap> frames, string fileName)
        {
            using (var gif = AnimatedGif.AnimatedGif.Create(fileName + ".gif", 175))
            {
                var lastFrame = frames.FirstOrDefault();
                gif.AddFrame(lastFrame, 750, GifQuality.Bit8);
                foreach (var frame in frames)
                {
                    gif.AddFrame(frame, -1, GifQuality.Bit8);
                    lastFrame = frame;
                }

                gif.AddFrame(lastFrame, 1000, GifQuality.Bit8);
            }
        }

        private Hex GetBoardHexFromCoordinates(int x, int y)
        {
            return _board.Hexes[x, y];
        }

        private void ChangeHexColor(Hex hex, Color color)
        {
            if (hex != null) hex.HexState.BackgroundColor = color;
        }

        private void Form_MouseMove(object sender, MouseEventArgs e)
        {
            if (_board != null)
            {
                var hexHoveringOver = _board.FindHexMouseClick(e.X - _graphicsEngine.BoardXOffset,
                    e.Y - _graphicsEngine.BoardYOffset);
                if (hexHoveringOver != null)
                {
                    var label = "[" + hexHoveringOver.Row + "," + hexHoveringOver.Column + "]";
                    if (hexHoveringOver.HexState.BackgroundColor == _lastTakenByPlayer1Colour ||
                        hexHoveringOver.HexState.BackgroundColor == _takenBeforeByPlayer1)
                        label += " Player 1";
                    else if (hexHoveringOver.HexState.BackgroundColor == _lastTakenByPlayer2Colour ||
                             hexHoveringOver.HexState.BackgroundColor == _takenBeforeByPlayer2)
                        label += " Player 2";
                    else
                        label += " not owned";
                    labelXY.Text = label;
                }
                else
                {
                    labelXY.Text = "";
                }
            }
        }

        private void buttonTestBoard_Click(object sender, EventArgs e)
        {
            if (textBoxHexBoardSize.Text == "")
            {
                //Check to see if entered size fits a 1080p window.
                // too small, i mean less than 5.  c'mon.
                MessageBox.Show("Must enter an integer between 5 and 20", "Invalid board size numnuts!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (Int32.Parse(textBoxHexBoardSize.Text) < 5 || Int32.Parse(textBoxHexBoardSize.Text) > 30)
            {
                // Window too large. gets unwieldy.  may have to work in some scaling int he future.
                MessageBox.Show("Must enter an integer between 5 and 20", "Invalid board size numnuts!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            // calc width & height based on board size
 //           Game.ActiveForm.Width = 569 + ((Int32.Parse(textBoxHexBoardSize.Text) - 5) * 60);
 //           Game.ActiveForm.Height = 370 + ((Int32.Parse(textBoxHexBoardSize.Text) - 5) * 38);

            //center the window after resize
//            CenterToScreen();

            Play();
        }


        private void Form_MouseClick(object sender, MouseEventArgs e)
        {
            Console.WriteLine(@"Mouse Click " + e.Location);

            if (_board != null && _graphicsEngine != null)
            {
                //
                // need to account for any offset
                //
                var mouseClick = new Point(e.X - _graphicsEngine.BoardXOffset, e.Y - _graphicsEngine.BoardYOffset);

                //Console.WriteLine("Click in Board bounding rectangle: {0}", board.PointInBoardRectangle(e.Location));

                var clickedHex = _board.FindHexMouseClick(mouseClick);

                if (clickedHex == null)
                {
                    //Console.WriteLine("No hex was clicked.");
                    _board.BoardState.ActiveHex = null;
                }
                else
                {
                    Console.WriteLine(@"Hex was clicked: [" + clickedHex.Row + @"," + clickedHex.Column + @"]");

                    _referee.ClickOnHexCoords(clickedHex.Row, clickedHex.Column);
                }
            }
        }

        private void Form_Paint(object sender, PaintEventArgs e)
        {
            //Draw the graphics/GUI
            _graphicsEngine?.Draw(e.Graphics);
        }

        private void Form_Closing(object sender, FormClosingEventArgs e)
        {
            _graphicsEngine = null;
            _board = null;
        }



        private void reloadConfigurationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetUpPlayers();
        }

        private void loadGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
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
                        Console.WriteLine(@"The file was not in the proper format.");
                        return;
                    }

                    _referee = new Referee(Convert.ToInt32(sizeNode.InnerText));

                    var firstPlayer = new Playback(1, _referee.Size, new Config());
                    var otherPlayer = new Playback(2, _referee.Size, new Config());
                    var player1Turn = 1;
                    var player2Turn = 1;
                    var moves = doc.GetElementsByTagName("Move");
                    foreach (XmlNode move in moves)
                    {
                        var player = Convert.ToInt32(move["Player"]?.InnerText);
                        var x = Convert.ToInt32(move["X"]?.InnerText);
                        var y = Convert.ToInt32(move["Y"]?.InnerText);
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

                    _referee.Player1 = firstPlayer;
                    _referee.Player2 = otherPlayer;

                    // and feed it the moves
                    StartGame();
                }
                catch (Exception exception)
                {
                    MessageBox.Show(@"Game couldn't be loaded properly : " + exception.Message);
                    Console.WriteLine(@"Game couldn't be loaded properly : " + exception.Message);
                }
        }

        private void saveGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Save the game and the moves.
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                using (var writer = XmlWriter.Create(saveFileDialog1.FileName))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("Match");
                    writer.WriteElementString("Date", DateTime.Now.ToShortDateString());
                    writer.WriteElementString("Size", textBoxHexBoardSize.Text);
                    writer.WriteStartElement("Players");
                    writer.WriteStartElement("Player");
                    writer.WriteElementString("Type", _referee.Player1.GetType().Name);
                    writer.WriteElementString("Number", "1");
                    writer.WriteEndElement();
                    writer.WriteStartElement("Player");
                    writer.WriteElementString("Type", _referee.Player2.GetType().Name);
                    writer.WriteElementString("Number", "2");
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                    writer.WriteStartElement("Moves");
                    var moveNumber = 1;
                    foreach (var move in _referee.AllGameMoves)
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

                MakeGif(_playThrough, saveFileDialog1.FileName);


                saveGameToolStripMenuItem.Enabled = false;
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var aboutForm = new About();
            aboutForm.Show();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}