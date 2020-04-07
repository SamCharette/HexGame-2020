using System.Net.Mime;

namespace Data
{
    public class Move
    {

        public int Row { get; set; }
        public int Column { get; set; }
        public int MoveNumber { get; set; }
        public int PlayerNumber { get; set; }
        public string PlayerNotes { get; set; }
        public int SecondsTaken { get; set; }


    }
}
