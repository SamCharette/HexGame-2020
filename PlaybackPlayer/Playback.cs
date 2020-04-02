using System;
using System.Collections.Generic;
using System.Linq;
using Players;

namespace PlaybackPlayer
{
    public class Playback : Player
    {

        public List<Move> Moves = new List<Move>();
        public int LastMoveNumber = 0;

        public new string PlayerType()
        {
            return "Playback Player";
        }

        public new bool IsAvailableToPlay()
        {
            return false;
        }
        public void AddMove(int x, int y, int moveNumber)
        {
            var move = new Move {X = x, Y = y, MoveNumber = moveNumber};
            Moves.Add(move);
        }
        public override Tuple<int, int> SelectHex(Tuple<int, int> opposingPick)
        {
            var moveToMake = Moves.OrderBy(x => x.MoveNumber).FirstOrDefault(y => y.MoveNumber > LastMoveNumber);
            LastMoveNumber++;
            if (moveToMake == null)
            {
                return null;
            }
            return new Tuple<int, int>(moveToMake.X, moveToMake.Y);
        }

        public Playback(int playerNumber, int boardSize, Config playerConfig) : base(playerNumber, boardSize, playerConfig)
        {
            RelayPerformanceInformation();
        }
    }
}