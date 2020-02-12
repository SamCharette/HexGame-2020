using System;
using System.Threading;
using System.Threading.Tasks;
using Engine.GameTypes;
using Engine.Interfaces;
using System.Timers;
using System.Xml;
using Timer = System.Timers.Timer;

namespace Engine.Players
{
    public class HumanPlayer : Player
    {
        private int _xFromUserInput = 0;
        private int _yFromUserInput = 0;
        private bool _hasReceivedInput = false;
        public new Tuple<int,int> SelectHex(Tuple<int,int> opponentMove)
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

        public HumanPlayer(int playerNumber, int boardSize) : base(playerNumber, boardSize)
        {
        }
    }
}
