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

     
        public List<Hex> GetBestPathForPlayer()
        {
            //ClearLog();
            foreach (var hex in _searchSpace.Hexes)
            {
                hex.ClearPathingVariables();
            }

            opponent = _playerSearchingFor == 1 ? -1 : 1;
       
            var startHexes = GetStartingHexes(_playerSearchingFor).OrderBy(x => x.RandomValue);
            var endHexes = GetEndingHexes(_playerSearchingFor).OrderBy(x => x.RandomValue);
            var path = new List<Hex>();

            var pathEase = _searchSpace.Size * _searchSpace.Size * costPerOpenNode;
            
            foreach (var startSpot in startHexes)
            {
           
                foreach (var endSpot in endHexes)
                {
                    foreach (var hex in _searchSpace.Hexes)
                    {
                        hex.ClearPathingVariables();
                    }

                    startSpot.G = startSpot.Owner == 0 ? costPerOpenNode : costPerFriendlyNode;
                    var newPath = PathBetween(startSpot, endSpot, pathEase).ToList();
         

                    newPath = newPath.OrderByDescending(x => x.F()).ToList();
                    if (newPath.Any() && ((newPath.First().F() < pathEase) 
                        || (newPath.First().F() == pathEase && newPath.Count < path.Count)))
                    {
                        pathEase = newPath.First().F();
                        path = newPath;
                    }
                }
            }
            return path;
        }

        public List<Hex> GetStartingHexes(int player)
        {
            var opponenNumber = player == 1 ? -1 : 1;
            if (player == 1)
            {
                return _searchSpace.Hexes.Where(x => x.Row == 0 && x.Owner != opponenNumber).ToList();
            }
            return _searchSpace.Hexes.Where(x => x.Column == 0 && x.Owner != opponenNumber).ToList();

        }

        public List<Hex> GetEndingHexes(int player)
        {
            var opponentNumber = player == 1 ? -1 : 1;
            if (player == 1)
            {
                return _searchSpace.Hexes.Where(x => x.Row == _searchSpace.Size - 1 && x.Owner != opponentNumber).ToList();
            }
            return _searchSpace.Hexes.Where(x => x.Column ==  _searchSpace.Size - 1 && x.Owner != opponentNumber).ToList();

        }

        public List<Hex> PathBetween(Hex start, Hex end, int currentBest)
        {
            // Get the best looking node
            var bestLookingHex = _searchSpace.Hexes
                .OrderBy(x => x.F())
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
