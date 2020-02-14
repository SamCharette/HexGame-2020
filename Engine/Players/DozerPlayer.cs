using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using Engine.GameTypes;

namespace Engine.Players
{
    public class DozerNode : BaseNode
    {
       
        public Status Status;
        public int G;
        public int H;
        public DozerNode Parent = null;
        public Guid uniqueness;

        public int F => G + H;

        public bool CanWalkTo(DozerNode possibleNeighbour)
        {
            // Can't be a neighbour to itself
            if (X == possibleNeighbour.X && Y == possibleNeighbour.Y)
            {
                return false;
            }

            if (possibleNeighbour.Status == Status.Closed)
            {
                return false;
            }
            // Can't walk if enemy owned
            if (possibleNeighbour.Owner != Owner && possibleNeighbour.Owner != 0)
            {
                return false;
            }
            // Top right
            if (X == possibleNeighbour.X + 1 && Y == possibleNeighbour.Y - 1)
            {
                return true;
            }
            // Right
            if (X == possibleNeighbour.X + 1 && Y == possibleNeighbour.Y)
            {
                return true;
            }
            // Bottom right
            if (X == possibleNeighbour.X && Y == possibleNeighbour.Y + 1)
            {
                return true;
            }
            // Bottom left
            if (X == possibleNeighbour.X - 1 && Y == possibleNeighbour.Y + 1)
            {
                return true;
            }
            // Left
            if (X == possibleNeighbour.X - 1 && Y == possibleNeighbour.Y)
            {
                return true;
            }
            // Top Left
            if (X == possibleNeighbour.X && Y == possibleNeighbour.Y - 1)
            {
                return true;
            }
            return false;
        }

    }

    

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
     * 6) It will restart its pathing should the enemy pick a hex on its path
     *  (this is rather dumb, but we want to start with something only a little smart)
     *
     * It will NOT:
     *
     * 1) plan ahead
     *
     * 2) React to the enemy in a thoughtful way
     */

    public class DozerPlayer : Player
    {
        private List<DozerNode> _preferredPath;
        private new List<DozerNode> _memory;
        private bool havePath = false;
        private DozerNode nodeIWant;
        private int costToMoveToClaimedNode = 5;
        private int costToMoveToUnclaimedNode = 10;
        private int costPerNodeTillEnd = 30;
        private int EnemyPlayerNumber
        {
            get { return PlayerNumber == 1 ? 2 : 1; }
        }
      
        public DozerPlayer(int playerNumber, int boardSize) : base(playerNumber, boardSize)
        {
            _preferredPath = new List<DozerNode>();
        }

        public string PlayerName()
        {
            return "Dumb Dozer";
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
            if (!_memory.Any())
            {
                
                SetUpInMemoryBoard();
            }
            if (opponentMove != null)
            {
                // Let's note the enemy's movement
                DozerNode enemyHex =
                    (DozerNode) _memory
                        .FirstOrDefault(hex => hex.X == opponentMove.Item1 
                                               && hex.Y == opponentMove.Item2);

                if (enemyHex != null)
                {
                    enemyHex.Owner = EnemyPlayerNumber;
                    enemyHex.Status = Status.Closed;
                    enemyHex.Parent = null;
                    Console.WriteLine("Enemy took hex [" + enemyHex.X + "," + enemyHex.Y + "]");
                }
                else
                {
                    Console.WriteLine("Hmm... No move from opponent?");
                }
            }

            

            if (!havePath || _preferredPath.Any(x => x.Owner == EnemyPlayerNumber ))
            {
                Console.WriteLine("I need a path...");
                StartOver();
                LookForPath();
                _preferredPath.Reverse();
            }

            if (!_preferredPath.Any())
            {
                Console.WriteLine("Whelp.  Couldn't find a path.");
            }
            nodeIWant = _preferredPath.FirstOrDefault(x => x.Owner == 0);
            
            if (nodeIWant != null)
            {
                nodeIWant.Owner = PlayerNumber;
                return new Tuple<int, int>(nodeIWant.X, nodeIWant.Y);
            }

            Console.WriteLine("Pfft.  I give up!");
            return null;
        }

        private void StartOver()
        {
            
            // Clear the parents
            _memory.ForEach(x =>x.Parent = null);
            // Set everything to untested again
            _memory.ForEach(x => x.Status = Status.Untested);

            Console.WriteLine("Can't see any open hexes.  Let's make one.");
            // Grab a random opening hex
            DozerNode startingHex = null;

            // Get all the hexes that are unowned
            var availableHexes = _memory
                .Where(x => x.Owner == 0);

            // Now of these hexes, we'd like to start at our board edge
            IEnumerable<DozerNode> availableStartingHexes;
            if (PlayerNumber == 1)
            {
                availableStartingHexes = availableHexes.Where(hex => hex.X == 0);
            }
            else
            {
                availableStartingHexes = availableHexes.Where(hex => hex.Y == 0);

            }

            startingHex = availableStartingHexes.OrderBy(x => x.uniqueness)
                .FirstOrDefault();

            if (startingHex != null)
            {
                startingHex.Status = Status.Open;
                Console.WriteLine("We's gunna start with [" + startingHex.X + "," + startingHex.Y + "]");
            }
        }

        private bool IsNodeAtBeginning(DozerNode node)
        {
            if (_isHorizontal)
            {
                return node.Y == 0;
            }

            return node.X == 0;
        }

        private bool IsNodeAtEnd(DozerNode node)
        {
            if (_isHorizontal)
            {
                return node.Y == _size - 1;
            }

            return node.X == _size - 1;
        }

        private void LookForPath()
        {
            DozerNode bestLookingNode = null;
            Console.WriteLine("Looking...");
            
            // GEt the best looking node
            bestLookingNode = _memory
                .OrderBy(x => x.F)
                .ThenBy(x => x.uniqueness)
                .FirstOrDefault(z => z.Status == Status.Open);

            if (bestLookingNode == null)
            {
                return;
            }

            Console.WriteLine("This node looks promising: [" + bestLookingNode.X + "," + bestLookingNode.Y + "]");

            // CLOSE IT
            bestLookingNode.Status = Status.Closed;

            if (IsNodeAtEnd(bestLookingNode))
            {
                _preferredPath = new List<DozerNode>();
                Console.WriteLine("Aha!  I found me a path!");
                var parent = bestLookingNode;
                while (parent != null)
                {
                    _preferredPath.Add(parent);
                    parent = parent.Parent;
                }

                havePath = true;
                return;
            }

            var neighbours = _memory
                .Where(x => bestLookingNode.CanWalkTo(x)).ToList();

            foreach (var node in neighbours)
            {
                if (node.Status == Status.Open)
                {
                    if (node.G > bestLookingNode.G + (node.Owner == PlayerNumber ? costToMoveToClaimedNode : costToMoveToUnclaimedNode))
                    {
                        node.Parent = bestLookingNode;
                        node.G = bestLookingNode.G + (node.Owner == PlayerNumber ? costToMoveToClaimedNode : costToMoveToUnclaimedNode); ;
                        node.H = (_isHorizontal ? _size - 1 - node.Y : _size - 1 - node.X) * costPerNodeTillEnd;
                    }
                }
                else
                {
                    node.Status = Status.Open;
                    node.Parent = bestLookingNode;
                    node.G = bestLookingNode.G + (node.Owner == PlayerNumber ? costToMoveToClaimedNode : costToMoveToUnclaimedNode);
                    node.H = (_isHorizontal ? _size - 1 - node.Y : _size - 1 - node.X) * costPerNodeTillEnd;
                }

            }
            LookForPath();

        }

        private bool IsMine(DozerNode node)
        {
            return node.Owner == PlayerNumber;
        }

       
        

        protected override void SetUpInMemoryBoard()
        {
            Console.WriteLine("Ok, let's start this up!");
            _memory = new List<DozerNode>();

            for (int x = 0; x < _size; x++)
            {
                for (int y = 0; y < _size; y++)
                {
                    var newNode = new DozerNode();
                    {
                        newNode.X = x;
                        newNode.Y = y;
                        newNode.Owner = 0;
                        newNode.Status = Status.Untested;
                        newNode.H = PlayerNumber == 1 ? _size - 1 - x: _size - 1 - y;
                        newNode.uniqueness = Guid.NewGuid();

                    }
                    _memory.Add(newNode);
                }
            }
        }

    }
}
