using System.Collections.Generic;
using System.Net.Mime;

namespace Data
{
    public class Move
    {
        public int Id { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
        public int MoveNumber { get; set; }
        public int PlayerNumber { get; set; }
        public string PlayerNotes { get; set; }
        public List<Monitor> PlayerMonitors { get; set; }
        public int TimeTaken { get; set; }

        public Move()
        {
            PlayerMonitors = new List<Monitor>();
        }

    }
}
