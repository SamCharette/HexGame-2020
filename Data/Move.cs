using System.Net.Mime;
using LiteDB;

namespace Data
{
    public class Move
    {
        public ObjectId Id { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
        public int MoveNumber { get; set; }
        public int PlayerNumber { get; set; }
        public string PlayerNotes { get; set; }
        public int SecondsTaken { get; set; }

        public Move()
        {
            Id = ObjectId.NewObjectId();
        }

    }
}
