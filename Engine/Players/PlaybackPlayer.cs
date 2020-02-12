using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Engine.GameTypes;
using Engine.Interfaces;

namespace Engine.Players
{
    public class Move
    {
        public int X;
        public int Y;
        public int MoveNumber;
    }
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
        public new Tuple<int, int> SelectHex(Tuple<int, int> opposingPick)
        {
            var moveToMake = Moves.OrderBy(x => x.MoveNumber).FirstOrDefault(y => y.MoveNumber > LastMoveNumber);
            LastMoveNumber++;
            if (moveToMake == null)
            {
                return null;
            }
            return new Tuple<int, int>(moveToMake.X, moveToMake.Y);
        }

        public Playback(int playerNumber, int boardSize) : base(playerNumber, boardSize)
        {
        }
    }
}
