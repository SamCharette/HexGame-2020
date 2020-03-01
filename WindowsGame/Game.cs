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
        private readonly Color _takenBeforeByPlayer1 = Color.DeepSkyBlue;
        private readonly Color _takenBeforeByPlayer2 = Color.LightCoral;
        private readonly Color _emptyBlueSide = Color.Azure;
        private readonly Color _emptyCorner = Color.Plum;
        private readonly Color _emptyRedSide = Color.MistyRose;
        private readonly Color _lastTakenByPlayer1 = Color.Blue;
        private readonly Color _lastTakenByPlayer2 = Color.Red;
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
            var appPath = Application.StartupPath;
            var configPath = Path.Combine(appPath, "Config\\players.json");
            _playerConfigs = JsonConvert.DeserializeObject<List<Config>>(File.ReadAllText(configPath));

            foreach (var player in _playerConfigs)
            {
                comboBoxPlayer1Type.Items.Add(player.name);
                comboBoxPlayer2Type.Items.Add(player.name);
            }

            textBoxHexBoardSize.Text = @"11";
            comboBoxPlayer1Type.SelectedItem = comboBoxPlayer1Type.Items[0];
            comboBoxPlayer2Type.SelectedItem = comboBoxPlayer2Type.Items[0];

        }

        public void Play()
        {
            _referee = new Referee(Convert.ToInt32(textBoxHexBoardSize.Text));
            _referee.GameOver += GameOver;
            _referee.PlayerMadeMove += PlayerMadeMove;
            _referee.NewGame(Convert.ToInt32(textBoxHexBoardSize.Text));
            _referee.AddPlayer(_playerConfigs.FirstOrDefault(x => x.name == comboBoxPlayer1Type.SelectedItem), 1);
            _referee.AddPlayer(_playerConfigs.FirstOrDefault(x => x.name == comboBoxPlayer2Type.SelectedItem), 2);
            StartGame();
        }


        public async void StartGame()
        {
            buttonTestBoard.Enabled = false;
            var boardSize = _referee.Size;
            lblWInner.Visible = false;
            btnSave.Visible = false;
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


            // make the edges colourful!
            foreach (var hex in _board.Hexes)
            {
                if (hex.Row == 0 || hex.Row == boardSize - 1) ChangeHexColor(hex, _emptyBlueSide);
                if (hex.Column == 0 || hex.Column == boardSize - 1) ChangeHexColor(hex, _emptyRedSide);
                if (hex.Column == 0 && hex.Row == 0 || hex.Column == 0 && hex.Row == boardSize - 1
                                                    || hex.Column == boardSize - 1 && hex.Row == 0 ||
                                                    hex.Column == boardSize - 1 && hex.Row == boardSize - 1)
                    ChangeHexColor(hex, _emptyCorner);
            }

            Refresh();

            try
            {
                _playThrough.Add(_graphicsEngine.CreateImage());

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


                    _referee.StartGame();

                   
                }

                
            }
            catch (Exception)
            {
                Console.WriteLine(@"The winner, because of a foul, is player #" + _referee.OpponentPlayer().PlayerNumber + "!");
            }
        }
         
        public void PlayerMadeMove(object sender, EventArgs args)
        {
            PlayerMadeMoveArgs moveArgs = (PlayerMadeMoveArgs) args;
            if (moveArgs != null)
            {
                var boardHex = _board.Hexes[moveArgs.move.Item1, moveArgs.move.Item2];

                if (_referee.WinningPlayer == null)
                    ChangeHexColor(boardHex, _referee.CurrentPlayer().PlayerNumber == 2
                        ? _lastTakenByPlayer1
                        : _lastTakenByPlayer2);
                else
                    ChangeHexColor(boardHex, _referee.WinningPlayer.PlayerNumber == 1
                        ? _lastTakenByPlayer1
                        : _lastTakenByPlayer2);
            }

            Refresh();

            _playThrough.Add(_graphicsEngine.CreateImage());
        }
        public void GameOver(object sender, EventArgs args)
        {
            // Show the winning path
            var colorForWinningPath =
                _referee.CurrentPlayer().PlayerNumber == 1 ? _lastTakenByPlayer1 : _lastTakenByPlayer2;
            foreach (var hex in _referee.winningPath)
                ChangeHexColor(GetBoardHexFromCoordinates(hex.Row, hex.Column), colorForWinningPath);

            lblWInner.Text = @"The winner is: Player #" + _referee.WinningPlayer.PlayerNumber;
            lblWInner.Visible = true;
            btnSave.Enabled = true;
            btnSave.Visible = true;
            Refresh();
            // Clear up the memory for the ref
            Console.WriteLine(@"The winner is player #" + _referee.WinningPlayer.PlayerNumber);
            _playThrough.Add(_graphicsEngine.CreateImage());
            _referee.Dispose();
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
                    if (hexHoveringOver.HexState.BackgroundColor == _lastTakenByPlayer1 ||
                        hexHoveringOver.HexState.BackgroundColor == _takenBeforeByPlayer1)
                        label += " Player 1";
                    else if (hexHoveringOver.HexState.BackgroundColor == _lastTakenByPlayer2 ||
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

        private void btnSave_Click(object sender, EventArgs e)
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


                btnSave.Enabled = false;
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
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
                    Console.WriteLine(@"Game couldn't be loaded properly : " + exception.Message);
                }
        }

        private void buttonReloadConfig_Click(object sender, EventArgs e)
        {
            SetUpPlayers();
        }
    }
}