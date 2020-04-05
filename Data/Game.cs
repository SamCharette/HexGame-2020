using System;
using System.Collections.Generic;
using System.Text;
using LiteDB;

namespace Data
{
    public class Game
    {
        public ObjectId Id { get; set; }
        public Config Player1 { get; set; }
        public Config Player2 { get; set; }
        public int BoardSize { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public ILiteCollection<Move> Moves { get; set; }
        public int Winner { get; set; }

        public Game()
        {
            Id = ObjectId.NewObjectId();
        }
    }
}
