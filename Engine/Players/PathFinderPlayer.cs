using System;
using System.Collections.Generic;
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

    public class PathFinderPlayer : Player
    {

      
        public PathFinderPlayer(int playerNumber, int boardSize) : base(playerNumber, boardSize)
        {
        }

        public string PlayerType()
        {
            return "Pathfinder AI";
        }
        public bool IsAvailableToPlay()
        {
            return true;
        }

        protected new void SetUpInMemoryBoard()
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
