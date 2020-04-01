using System;
using System.Collections.Generic;
using System.Linq;
using Players;

namespace NegamaxPlayer
{
    /*
     * This class is what will find a path from one side to the other
     */
    public class Pathfinder
    {
        private Board _searchSpace;
        private int _playerSearchingFor;
        private bool IsLogging;
        private int opponent;
        private int costPerFriendlyNode;
        private int costPerOpenNode;
        private int costPerNodeLeft;
        private string Log;

        public Pathfinder(Board searchThis, 
            int searchForThisPlayer, 
            int friendlyCost = 0,
            int openCost = 20,
            int costPerNodeTillEnd = 10,
            bool isLogging = false)
        {
            _searchSpace = searchThis;
            _playerSearchingFor = searchForThisPlayer;
            IsLogging = isLogging;
            costPerFriendlyNode = friendlyCost;
            costPerNodeLeft = costPerNodeTillEnd;
            costPerOpenNode = openCost;

        }

     
        public List<Hex> GetPathForPlayer()
        {
            //ClearLog();
            foreach (var hex in _searchSpace.Hexes)
            {
                hex.ClearPathingVariables();
            }

            opponent = _playerSearchingFor == 1 ? 2 : 1;
            //AddLogLine(" ============ Starting new search for " + _playerSearchingFor);
            var startHexes = GetStartingHexes(_playerSearchingFor).OrderBy(x => x.RandomValue);
            var endHexes = GetEndingHexes(_playerSearchingFor).OrderBy(x => x.RandomValue);
            var path = new List<Hex>();

            var pathEase = _searchSpace.Size * _searchSpace.Size * costPerOpenNode;
            //AddLogLine("Need to move around " + _searchSpace.Hexes.Count(x => x.Owner == opponent) + " hexes.");
            //_searchSpace.Hexes.Where(x => x.Owner == opponent).ToList().ForEach(x => AddLog(x + " "));
            //AddLogLine("");

            foreach (var startSpot in startHexes)
            {
           
                foreach (var endSpot in endHexes)
                {
                    //AddLogLine("Best score is " + pathEase);
                    //AddLogLine("---------- Searching between " + startSpot + " and " + endSpot);
                    foreach (var hex in _searchSpace.Hexes)
                    {
                        hex.ClearPathingVariables();
                    }

                    startSpot.G = startSpot.Owner == 0 ? costPerOpenNode : costPerFriendlyNode;
                    var newPath = PathBetween(startSpot, endSpot, pathEase).ToList();
                    //Console.WriteLine(GetLog());

                    newPath = newPath.OrderByDescending(x => x.F).ToList();
                    if (newPath.Any() && ((newPath.First().F < pathEase) 
                        || (newPath.First().F == pathEase && newPath.Count < path.Count)))
                    {
                        pathEase = newPath.First().F;
                        path = newPath;
                        //Console.WriteLine("");
                        //Console.WriteLine("(" + path.Count + ") Better path found with score : " + pathEase);
                        //Console.Write("Path found : ");
                        //newPath.ForEach(x => Console.Write(" " + x));
                        //Console.WriteLine("");
                    }
                }
            }
            //AddLogLine("---------- ");
            //AddLogLine("Final score is " + pathEase);
            //AddLog("Path found : ");
            //path.ForEach(x => AddLog(" " + x));
            //AddLogLine("");
            //OutputLogToConsole();
            return path;
        }

        public List<Hex> GetStartingHexes(int player)
        {
            var opponent = player == 1 ? 2 : 1;
            if (player == 1)
            {
                return _searchSpace.Hexes.Where(x => x.Row == 0 && x.Owner != opponent).ToList();
            }
            return _searchSpace.Hexes.Where(x => x.Column == 0 && x.Owner != opponent).ToList();

        }

        public List<Hex> GetEndingHexes(int player)
        {
            var opponent = player == 1 ? 2 : 1;
            if (player == 1)
            {
                return _searchSpace.Hexes.Where(x => x.Row == _searchSpace.Size - 1 && x.Owner != opponent).ToList();
            }
            return _searchSpace.Hexes.Where(x => x.Column ==  _searchSpace.Size - 1 && x.Owner != opponent).ToList();

        }

        public List<Hex> PathBetween(Hex start, Hex end, int currentBest)
        {
            // Get the best looking node
            var bestLookingHex = _searchSpace.Hexes
                .OrderBy(x => x.F)
                .ThenBy(x => x.RandomValue)
                .FirstOrDefault(z => z.Status == Status.Open);

            if (bestLookingHex == null)
            {
                if (start.Status == Status.Untested || start.Status == Status.Open)
                    bestLookingHex = start;
                else
                    return new List<Hex>();
            }

            if (bestLookingHex.Equals(end))
            {
                AddLogLine(bestLookingHex + " is at end.");
                var preferredPath = new List<Hex>();

                var parent = bestLookingHex;
                while (parent != null)
                {
                    if (!preferredPath.Contains(parent))
                    {
                        preferredPath.Add(parent);
                        parent = parent.Parent;
                    } 
                    else
                    {
                        parent = null;
                    }
                }

                return preferredPath;
            }

            bestLookingHex.Status = Status.Closed;


            var neighbours =  _searchSpace.GetNeighboursFrom(bestLookingHex, _playerSearchingFor);
            AddLog(neighbours.Count + " neighbours to examine.");
            foreach (var node in neighbours)
            {
                if (node.Owner != opponent)
                {
                    if (node.Status == Status.Open)
                    {
                        if (node.G > bestLookingHex.G +
                            (node.Owner == _playerSearchingFor 
                                ? costPerFriendlyNode 
                                : costPerOpenNode))
                        {
                            node.Parent = bestLookingHex;
                            node.G = bestLookingHex.G +
                                     (node.Owner == _playerSearchingFor 
                                         ? costPerFriendlyNode 
                                         : costPerOpenNode);
                            
                            node.H =
                                (_playerSearchingFor == 1 
                                    ? _searchSpace.Size - 1 - node.Row 
                                    : _searchSpace.Size - 1 - node.Column) *  costPerNodeLeft;
                        }
                    }
                    else if (node.Status == Status.Untested)
                    {
                        node.Status = Status.Open;
                        node.Parent = bestLookingHex;
                        node.G = bestLookingHex.G +
                                 (node.Owner == _playerSearchingFor 
                                     ? costPerFriendlyNode 
                                     : costPerOpenNode);

                        node.H = (_playerSearchingFor == 1 
                                     ? _searchSpace.Size - 1 - node.Row 
                                     : _searchSpace.Size - 1 - node.Column) * costPerNodeLeft;
                    }
                }

            }
        
            return PathBetween(start, end, currentBest);
        }

        public void SetMap(Board map)
        {
            _searchSpace = map;
        }

        public void SetPlayer(int player)
        {
            _playerSearchingFor = player;
        }

        private void AddLogLine(string text)
        {
            if (IsLogging)
            {
                AddLog(text + Environment.NewLine);
            }
        }

        private void AddLog(string text)
        {
            if (IsLogging)
            {
                Log += text;
            }
        }

        private void ClearLog()
        {
            if (IsLogging)
            {
                Log = "";
            }
        }

        public void OutputLogToConsole()
        {
            if (IsLogging)
            {
                Console.WriteLine(Log);
            }
        }
        public string GetLog()
        {
            return Log;
        }

    }
}
