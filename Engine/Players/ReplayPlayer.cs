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
    public class ReplayPlayer : IPlayer
    {
        public string Name { get; set; }
        public int PlayerNumber { get; set; }

        public List<Move> Moves = new List<Move>();
        public int LastMoveNumber = 0;

        public void AddMove(int x, int y, int moveNumber)
        {
            var move = new Move {X = x, Y = y, MoveNumber = moveNumber};
            Moves.Add(move);
        }
        public Hex SelectHex(Board board)
        {
            var moveToMake = Moves.OrderBy(x => x.MoveNumber).FirstOrDefault(y => y.MoveNumber > LastMoveNumber);
            var hexToTake = board.Spaces?.FirstOrDefault(x => x.X == moveToMake?.X && x.Y == moveToMake?.Y);
            LastMoveNumber++;
            return hexToTake;
        }
    }
}
