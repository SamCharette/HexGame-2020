using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Data
{
    public class Game
    {
        public int Id { get; set; }
        public GamePlayer Player1 { get; set; }
        public GamePlayer Player2 { get; set; }
        public int BoardSize { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public List<Move> Moves { get; set; }
        public int Winner { get; set; }


        public Game()
        {
            Moves = new List<Move>();
        }

        public int TotalTime(int playerNumber)
        {
            return Moves.Where(x => x.PlayerNumber == playerNumber).Select(x => x.TimeTaken).Sum();
        }

        public int AverageTime(int playerNumber)
        {
            var numberOfMoves = Moves.Count(x => x.PlayerNumber == playerNumber);
            return TotalTime(playerNumber) / numberOfMoves;
        }
        public string AverageTimeFormatted(int playerNumber)
        {
            return FormatTimeSpan(TimeSpan.FromMilliseconds(AverageTime(playerNumber)));
        }

        public string TotalTimeFormatted(int playerNumber)
        {
            return FormatTimeSpan(TimeSpan.FromMilliseconds(TotalTime(playerNumber)));
        }

        private static string FormatTimeSpan(TimeSpan timeSpan)
        {
            Func<Tuple<int, string>, string> tupleFormatter = t => $"{t.Item1} {t.Item2}{(t.Item1 == 1 ? string.Empty : "s")}";
            var components = new List<Tuple<int, string>>
            {
                Tuple.Create((int) timeSpan.TotalDays, "day"),
                Tuple.Create(timeSpan.Hours, "hour"),
                Tuple.Create(timeSpan.Minutes, "min"),
                Tuple.Create(timeSpan.Seconds, "sec"),
            };

            components.RemoveAll(i => i.Item1 == 0);

            string extra = "";

            if (components.Count > 1)
            {
                var finalComponent = components[components.Count - 1];
                components.RemoveAt(components.Count - 1);
                extra = $" {tupleFormatter(finalComponent)}";
            }

            return $"{string.Join(", ", components.Select(tupleFormatter))}{extra}";
        }
    }
}
