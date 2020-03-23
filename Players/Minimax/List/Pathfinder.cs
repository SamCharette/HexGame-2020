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

            var startHexes = GetStartingHexes(_playerSearchingFor.Me);
            var endHexes = GetEndingHexes(_playerSearchingFor.Me);
            var path = new List<ListHex>();

            var pathEase = _searchSpace.Size * _searchSpace.Size * Math.Max(_playerSearchingFor.CostPerNodeTillEnd, _playerSearchingFor.CostToMoveToUnclaimedNode);

            foreach (var startSpot in startHexes)
            {
                foreach (var endSpot in endHexes)
                {
                    foreach (var hex in _searchSpace.Board)
                    {
                        hex.ClearPathingVariables();
                    }
                    var newPath = PathBetween(startSpot, endSpot, pathEase);
                    if (newPath.Any() && newPath.First().F() < pathEase)
                    {
                        pathEase = newPath.First().F();
                        path = newPath;
                    }
                }
            }

            return path;
        }

        private List<ListHex> GetStartingHexes(PlayerType player)
        {
            var opponent = player == PlayerType.Blue ? PlayerType.Red : PlayerType.Blue;
            if (player == PlayerType.Blue)
            {
                return _searchSpace.Board.Where(x => x.Row == 0 && x.Owner != opponent).ToList();
            }
            return _searchSpace.Board.Where(x => x.Column == 0 && x.Owner != player).ToList();

        }

        private List<ListHex> GetEndingHexes(PlayerType player)
        {
            var opponent = player == PlayerType.Blue ? PlayerType.Red : PlayerType.Blue;
            if (player == PlayerType.Blue)
            {
                return _searchSpace.Board.Where(x => x.Row == _searchSpace.Size - 1 && x.Owner != opponent).ToList();
            }
            return _searchSpace.Board.Where(x => x.Column ==  _searchSpace.Size - 1 && x.Owner != player).ToList();

        }

        public List<ListHex> PathBetween(ListHex start, ListHex end, int currentBest)
        {
            // Get the best looking node
            var bestLookingHex = _searchSpace.Board
                .OrderBy(x => x.F())
                .ThenBy(x => x.RandomValue)
                .FirstOrDefault(z => z.Status == Status.Open);

            if (bestLookingHex == null || bestLookingHex.F() > currentBest)
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


            var neighbours =  _searchSpace.GetNeighboursFrom(bestLookingHex, _playerSearchingFor.Me);

            foreach (var node in neighbours)
            {
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

            }
        
            return PathBetween(start, end, currentBest);
        }

    }
}
