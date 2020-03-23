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


        public Pathfinder(ListMap searchThis, 
            ListPlayer searchForThisPlayer)
        {
            _searchSpace = searchThis;
            _playerSearchingFor = searchForThisPlayer;
        }

     
        public List<ListHex> GetPathForPlayer()
        {
            var matrix = _searchSpace.GetPlayerMatrix(_playerSearchingFor.Me);
            List<ListHex> start = new List<ListHex>();
            if (_playerSearchingFor.Me == PlayerType.Blue)
            {

                var list = matrix.EnumerateIndexed(Zeros.AllowSkip);

                var rows =   matrix.EnumerateRowsIndexed(0, 1);
                
            }
            else
            {

            }

            var startHexes = _playerSearchingFor.PlayerNumber == 1
                ? _searchSpace.GetPlayerMatrix(_playerSearchingFor.Me).Row(0).AsEnumerable()
                : _searchSpace.GetPlayerMatrix(_playerSearchingFor.Me).Column(0).AsEnumerable();
            var endHexes = _playerSearchingFor.PlayerNumber == 1
                ? _searchSpace.GetPlayerMatrix(_playerSearchingFor.Me).Row(_searchSpace.Size - 1).AsEnumerable()
                : _searchSpace.GetPlayerMatrix(_playerSearchingFor.Me).Column(_searchSpace.Size - 1).AsEnumerable();

            var path = new List<ListHex>();

            var pathSize = _searchSpace.Size * _searchSpace.Size + 1;

            foreach (var startSpot in startHexes)
            {
                foreach (var endSpot in endHexes)
                {
                    //var newPath = PathBetween(startSpot, endSpot);
                    //if (newPath.Count < pathSize)
                    //{
                    //    path = newPath;
                    //}
                }
            }

            return path;
        }


        public List<ListHex> PathBetween(ListHex start, ListHex end)
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

            if (bestLookingHex.IsAttachedToBothEnds())
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


            var neighbours = new List<ListHex>(); // _searchSpace.GetTraversablePhysicalNeighbours(bestLookingHex, _playerSearchingFor.Me);

            foreach (var node in neighbours)
                if (node.Owner != _playerSearchingFor.Opponent())
                {
                    if (node.Status == Status.Open)
                    {
                        if (node.G > bestLookingHex.G +
                            (node.Owner == _playerSearchingFor.Me ? _playerSearchingFor.CostToMoveToClaimedNode : _playerSearchingFor.CostToMoveToUnclaimedNode))
                        {
                            node.Parent = bestLookingHex;
                            node.G = bestLookingHex.G +
                                     (node.Owner == _playerSearchingFor.Me ? _playerSearchingFor.CostToMoveToClaimedNode : _playerSearchingFor.CostToMoveToUnclaimedNode);
                            ;
                            node.H =
                                (_playerSearchingFor.Me == Common.PlayerType.Red ? _searchSpace.Size - 1 - node.Column : _searchSpace.Size - 1 - node.Row) *  _playerSearchingFor.CostPerNodeTillEnd;
                        }
                    }
                    else if (node.Status == Status.Untested)
                    {
                        node.Status = Status.Open;
                        node.Parent = bestLookingHex;
                        node.G = bestLookingHex.G +
                                 (node.Owner == _playerSearchingFor.Me ? _playerSearchingFor.CostToMoveToClaimedNode : _playerSearchingFor.CostToMoveToUnclaimedNode);
                        node.H = (_playerSearchingFor.Me == Common.PlayerType.Red ? _searchSpace.Size - 1 - node.Column : _searchSpace.Size - 1 - node.Row) * _playerSearchingFor.CostPerNodeTillEnd;
                    }
                }

            return PathBetween(start, end);
        }

    }
}
