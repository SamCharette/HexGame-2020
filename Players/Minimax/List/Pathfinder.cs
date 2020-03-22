using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            //var start = _playerSearchingFor.PlayerNumber == 1 ? _searchSpace.Top : _searchSpace.Left;
            //var end = _playerSearchingFor.PlayerNumber == 1 ? _searchSpace.Bottom : _searchSpace.Right;

            //return PathBetween(start, end);
            return null;
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

            if (bestLookingHex.IsAttachedToBothEnds(_playerSearchingFor.Me))
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
