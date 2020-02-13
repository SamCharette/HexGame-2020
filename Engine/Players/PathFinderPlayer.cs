using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.GameTypes;
using Engine.Interfaces;

namespace Engine.Players
{
    public class PathfinderNode : BaseNode
    {
       
        public Status Status;
        public int G;
        public int H;
        public Node Parent = null;

        public int F => G + H;

    }

    public enum Status
    {
        Open,
        Closed,
        Untested
    };

    /*
     *  PATH FINDER PLAYER
     *
     * This player type is only basically intelligent.  It will:
     *
     * 1) Select a random hex on the 0 row/column to begin with
     *
     * 2) It will keep track of game state
     *
     * 3) It will Update its own representation of the board with enemy moves
     *
     * 4) It will find best path based on A*
     *
     * 5) It will select the next hex available on the path it finds
     *
     * It will NOT:
     *
     * 1) plan ahead
     *
     * 2) React to the enemy in a thoughtful way
     */

    public class PathFinderPlayer : Player
    {
        private List<PathfinderNode> _preferredPath;
        private bool havePath = false;
      
        public PathFinderPlayer(int playerNumber, int boardSize) : base(playerNumber, boardSize)
        {
            _preferredPath = new List<PathfinderNode>();
        }

        public string PlayerType()
        {
            return "Pathfinder AI";
        }
        public bool IsAvailableToPlay()
        {
            return true;
        }

        public override Tuple<int, int> SelectHex(Tuple<int, int> opponentMove)
        {
            if (opponentMove != null)
            {
                // Let's note the enemy's movement
                PathfinderNode enemyHex =
                    (PathfinderNode) _memory.FirstOrDefault(hex => hex.X == opponentMove.Item1 && hex.Y == opponentMove.Item2);

                if (enemyHex != null)
                {
                    enemyHex.Owner = PlayerNumber == 1 ? 2 : 1;
                    enemyHex.Status = Status.Closed;

                    // If we have a preferred path, but this is on it, we will need to recalculate
                    if (havePath && _preferredPath.Any(hex => hex.X == enemyHex.X && hex.Y == enemyHex.Y))
                    {
                        havePath = false;
                    }

                }
            }

            if (!havePath)
            {
                LookForPath();
            }

            return null;
        }

        private void LookForPath()
        {
            // If we don't have any open hexes to start with we's boneded :)
            //if ()
        }
        

        protected override void SetUpInMemoryBoard()
        {
            _memory = new List<BaseNode>();

            for (int x = 0; x < EndNodeLocation; x++)
            {
                for (int y = 0; y < EndNodeLocation; y++)
                {
                    var newNode = new PathfinderNode();
                    {
                        newNode.X = x;
                        newNode.Y = y;
                        newNode.Owner = 0;

                        if (_isHorizontal)
                        {
                            newNode.Status = y == 0 ? Status.Open : Status.Untested;
                            newNode.H = EndNodeLocation - y;
                        }
                        else
                        {
                            newNode.Status = x == 0 ? Status.Open : Status.Untested;
                            newNode.H = EndNodeLocation - x;
                        }
                    }
                }
            }
        }

    }
}
