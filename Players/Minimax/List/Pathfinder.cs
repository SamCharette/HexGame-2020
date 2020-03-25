using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using MathNet.Numerics.LinearAlgebra;
using Players.Common;

namespace Players.Minimax.List
{
    /*
     * This class is what will find a path from one side to the other
     */
    public class Pathfinder
    {
        private ListMap _searchSpace;
        private ListPlayer _playerSearchingFor;
        private bool IsLogging;
        private string Log;

        public Pathfinder(ListMap searchThis, 
            ListPlayer searchForThisPlayer, 
            bool isLogging = false)
        {
            _searchSpace = searchThis;
            _playerSearchingFor = searchForThisPlayer;
            IsLogging = isLogging;
        }

     
        public List<ListHex> GetPathForPlayer()
        {
            ClearLog();
            AddLogLine(" ============ Starting new search for " + _playerSearchingFor.Me);
            var startHexes = GetStartingHexes(_playerSearchingFor.Me);
            var endHexes = GetEndingHexes(_playerSearchingFor.Me);
            var path = new List<ListHex>();

            var pathEase = _searchSpace.Size * _searchSpace.Size * Math.Max(_playerSearchingFor.CostPerNodeTillEnd, _playerSearchingFor.CostToMoveToUnclaimedNode);
            AddLogLine("Need to move around " + _searchSpace.Board.Count(x => x.Owner == _playerSearchingFor.Opponent()) + " hexes.");
            _searchSpace.Board.Where(x => x.Owner == _playerSearchingFor.Opponent()).ToList().ForEach(x => AddLog(x + " "));
            AddLogLine("");

            foreach (var startSpot in startHexes)
            {
                startSpot.G = startSpot.Owner == _playerSearchingFor.Me
                    ? _playerSearchingFor.CostToMoveToClaimedNode
                    : _playerSearchingFor.CostToMoveToUnclaimedNode;

                
                foreach (var endSpot in endHexes)
                {
                    AddLogLine("Best score is " + pathEase);
                    AddLogLine("---------- Searching between " + startSpot + " and " + endSpot);
                    foreach (var hex in _searchSpace.Board)
                    {
                        hex.ClearPathingVariables();
                    }
                    var newPath = PathBetween(startSpot, endSpot, pathEase);


                    if (newPath.Any() && ((newPath.First().F() < pathEase) 
                        || (newPath.First().F() == pathEase && newPath.Count < path.Count)))
                    {
                        pathEase = newPath.First().F();
                        path = newPath;
                        AddLogLine("");
                        AddLogLine("(" + path.Count + ") Better path found with score : " + pathEase);
                        AddLog("Path found : ");
                        newPath.ForEach(x => AddLog(" " + x));
                        AddLogLine("");
                    }
                }
            }
            AddLogLine("---------- ");
            AddLogLine("Final score is " + pathEase);
            AddLog("Path found : ");
            path.ForEach(x => AddLog(" " + x));
            AddLogLine("");
            return path;
        }

        public List<ListHex> GetStartingHexes(PlayerType player)
        {
            var opponent = player == PlayerType.Blue ? PlayerType.Red : PlayerType.Blue;
            if (player == PlayerType.Blue)
            {
                return _searchSpace.Board.Where(x => x.Row == 0 && x.Owner != opponent).ToList();
            }
            return _searchSpace.Board.Where(x => x.Column == 0 && x.Owner != opponent).ToList();

        }

        public List<ListHex> GetEndingHexes(PlayerType player)
        {
            var opponent = player == PlayerType.Blue ? PlayerType.Red : PlayerType.Blue;
            if (player == PlayerType.Blue)
            {
                return _searchSpace.Board.Where(x => x.Row == _searchSpace.Size - 1 && x.Owner != opponent).ToList();
            }
            return _searchSpace.Board.Where(x => x.Column ==  _searchSpace.Size - 1 && x.Owner != opponent).ToList();

        }

        public List<ListHex> PathBetween(ListHex start, ListHex end, int currentBest)
        {
            // Get the best looking node
            var bestLookingHex = _searchSpace.Board
                .OrderBy(x => x.F())
                .ThenBy(x => x.RandomValue)
                .FirstOrDefault(z => z.Status == Status.Open);

            if (bestLookingHex == null)
            {
                if (start.Status == Status.Untested || start.Status == Status.Open)
                    bestLookingHex = start;
                else
                    return new List<ListHex>();
            }

            if (bestLookingHex.Equals(end))
            {
                var preferredPath = new List<ListHex>();

                var parent = bestLookingHex;
                while (parent != null)
                {
                    preferredPath.Add(parent);
                    parent = parent.Parent;
                }

                return preferredPath;
            }

            bestLookingHex.Status = Status.Closed;


            var neighbours =  _searchSpace.GetNeighboursFrom(bestLookingHex, _playerSearchingFor.Me);

            foreach (var node in neighbours)
            {
                if (node.Owner != _playerSearchingFor.Opponent())
                {
                    if (node.Status == Status.Open)
                    {
                        if (node.G > bestLookingHex.G +
                            (node.Owner == _playerSearchingFor.Me 
                                ? _playerSearchingFor.CostToMoveToClaimedNode 
                                : _playerSearchingFor.CostToMoveToUnclaimedNode))
                        {
                            node.Parent = bestLookingHex;
                            node.G = bestLookingHex.G +
                                     (node.Owner == _playerSearchingFor.Me 
                                         ? _playerSearchingFor.CostToMoveToClaimedNode 
                                         : _playerSearchingFor.CostToMoveToUnclaimedNode);
                            
                            node.H =
                                (_playerSearchingFor.Me == Common.PlayerType.Blue 
                                    ? _searchSpace.Size - 1 - node.Row 
                                    : _searchSpace.Size - 1 - node.Column) *  _playerSearchingFor.CostPerNodeTillEnd;
                        }
                    }
                    else if (node.Status == Status.Untested)
                    {
                        node.Status = Status.Open;
                        node.Parent = bestLookingHex;
                        node.G = bestLookingHex.G +
                                 (node.Owner == _playerSearchingFor.Me 
                                     ? _playerSearchingFor.CostToMoveToClaimedNode 
                                     : _playerSearchingFor.CostToMoveToUnclaimedNode);

                        node.H = (_playerSearchingFor.Me == Common.PlayerType.Blue 
                                     ? _searchSpace.Size - 1 - node.Row 
                                     : _searchSpace.Size - 1 - node.Column) * _playerSearchingFor.CostPerNodeTillEnd;
                    }
                }

            }
        
            return PathBetween(start, end, currentBest);
        }

        public void SetMap(ListMap map)
        {
            _searchSpace = map;
        }

        public void SetPlayer(ListPlayer player)
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

        public string GetLog()
        {
            return Log;
        }

    }
}
