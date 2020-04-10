using System;
using System.Collections.Generic;
using System.Text;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Complex;
using Players;

namespace MonteCarloPlayer
{
    public class Pathfinder
    {
        //public bool WinningPathForPlayerExists(Matrix<int> board, int playerNumber)
        //{
        //    //ClearLog();
        //    foreach (var hex in _searchSpace.Hexes)
        //    {
        //        hex.ClearPathingVariables();
        //    }


        //    var List<Tuple<int,int,int>> = GetStartingHexes(playerNumber);
        //    var endHexes = GetEndingHexes(_playerSearchingFor).OrderBy(x => x.RandomValue);
        //    var path = new List<Hex>();

        //    var pathEase = _searchSpace.Size * _searchSpace.Size * costPerOpenNode;

        //    foreach (var startSpot in startHexes)
        //    {

        //        foreach (var endSpot in endHexes)
        //        {
        //            foreach (var hex in _searchSpace.Hexes)
        //            {
        //                hex.ClearPathingVariables();
        //            }

        //            startSpot.G = startSpot.Owner == 0 ? costPerOpenNode : costPerFriendlyNode;
        //            var newPath = PathBetween(startSpot, endSpot, pathEase).ToList();


        //            newPath = newPath.OrderByDescending(x => x.F()).ToList();
        //            if (newPath.Any() && ((newPath.First().F() < pathEase)
        //                || (newPath.First().F() == pathEase && newPath.Count < path.Count)))
        //            {
        //                pathEase = newPath.First().F();
        //                path = newPath;
        //            }
        //        }
        //    }
        //    return path;
        //}

        //public List<Tuple<int,int,int>> GetStartingHexes(Matrix<int> board, int player)
        //{
            

        //    if (player == 1)
        //    {
        //        var rows = board.EnumerateRowsIndexed(0, board.RowCount);
        //        return _searchSpace.Hexes.Where(x => x.Row == 0 && x.Owner != opponenNumber).ToList();
        //    }
        //    return _searchSpace.Hexes.Where(x => x.Column == 0 && x.Owner != opponenNumber).ToList();

        //}

        //public List<Hex> GetEndingHexes(int player)
        //{
        //    var opponentNumber = player == 1 ? -1 : 1;
        //    if (player == 1)
        //    {
        //        return _searchSpace.Hexes.Where(x => x.Row == _searchSpace.Size - 1 && x.Owner != opponentNumber).ToList();
        //    }
        //    return _searchSpace.Hexes.Where(x => x.Column == _searchSpace.Size - 1 && x.Owner != opponentNumber).ToList();

        //}

        //public List<Hex> PathBetween(Hex start, Hex end, int currentBest)
        //{
        //    // Get the best looking node
        //    var bestLookingHex = _searchSpace.Hexes
        //        .OrderBy(x => x.F())
        //        .ThenBy(x => x.RandomValue)
        //        .FirstOrDefault(z => z.Status == Status.Open);

        //    if (bestLookingHex == null)
        //    {
        //        if (start.Status == Status.Untested || start.Status == Status.Open)
        //            bestLookingHex = start;
        //        else
        //            return new List<Hex>();
        //    }

        //    if (bestLookingHex.Equals(end))
        //    {

        //        var preferredPath = new List<Hex>();

        //        var parent = bestLookingHex;
        //        while (parent != null)
        //        {
        //            if (!preferredPath.Contains(parent))
        //            {
        //                preferredPath.Add(parent);
        //                parent = parent.Parent;
        //            }
        //            else
        //            {
        //                parent = null;
        //            }
        //        }

        //        return preferredPath;
        //    }

        //    bestLookingHex.Status = Status.Closed;


        //    var neighbours = _searchSpace.GetNeighboursFrom(bestLookingHex, _playerSearchingFor);

        //    foreach (var node in neighbours)
        //    {
        //        if (node.Owner != opponent)
        //        {
        //            if (node.Status == Status.Open)
        //            {
        //                if (node.G > bestLookingHex.G +
        //                    (node.Owner == _playerSearchingFor
        //                        ? costPerFriendlyNode
        //                        : costPerOpenNode))
        //                {
        //                    node.Parent = bestLookingHex;
        //                    node.G = bestLookingHex.G +
        //                             (node.Owner == _playerSearchingFor
        //                                 ? costPerFriendlyNode
        //                                 : costPerOpenNode);

        //                    node.H =
        //                        (_playerSearchingFor == 1
        //                            ? _searchSpace.Size - 1 - node.Row
        //                            : _searchSpace.Size - 1 - node.Column) * costPerNodeLeft;
        //                }
        //            }
        //            else if (node.Status == Status.Untested)
        //            {
        //                node.Status = Status.Open;
        //                node.Parent = bestLookingHex;
        //                node.G = bestLookingHex.G +
        //                         (node.Owner == _playerSearchingFor
        //                             ? costPerFriendlyNode
        //                             : costPerOpenNode);

        //                node.H = (_playerSearchingFor == 1
        //                             ? _searchSpace.Size - 1 - node.Row
        //                             : _searchSpace.Size - 1 - node.Column) * costPerNodeLeft;
        //            }
        //        }

        //    }

        //    return PathBetween(start, end, currentBest);
        //}

    }
}
