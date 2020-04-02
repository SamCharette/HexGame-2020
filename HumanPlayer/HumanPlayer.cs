using System;
using Players;
using Timer = System.Timers.Timer;

namespace HumanPlayer
{
    public class HumanPlayer : Player
    {
        private int _xFromUserInput = 0;
        private int _yFromUserInput = 0;
        private bool _hasReceivedInput = false;

        public override Tuple<int, int> SelectHex(Tuple<int, int> opponentMove)
        {
            _hasReceivedInput = false;
            while (!_hasReceivedInput)
            {

            }

            return new Tuple<int, int>(_xFromUserInput, _yFromUserInput);
        }

        public new string PlayerType()
        {
            return "Human";
        }
        public new bool IsAvailableToPlay()
        {
            return true;
        }

        public void ClickMadeOn(Tuple<int, int> clickedHex)
        {
            _xFromUserInput = clickedHex.Item1;
            _yFromUserInput = clickedHex.Item2;
            _hasReceivedInput = true;
        }

        public HumanPlayer(int playerNumber, int boardSize, Config playerConfig) : base(playerNumber, boardSize, playerConfig)
        {
            RelayPerformanceInformation();
            base.SetUpInMemoryBoard();
            Name = "Human Player";
        }
    }
}
