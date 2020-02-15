using System;
using System.Collections.Generic;
using System.Linq;
using Players.Base;
using Players.Common;

namespace Players
{
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
        private List<BaseNode> _preferredPath;
//        private new List<BaseNode> _memory;
        private bool havePath = false;
        private BaseNode nodeIWant;
        private int costToMoveToClaimedNode = 10;
        private int costToMoveToUnclaimedNode = 100;
        private int costPerNodeTillEnd = 1000;
        private int EnemyPlayerNumber
        {
            get { return PlayerNumber == 1 ? 2 : 1; }
        }
      
        public DozerPlayer(int playerNumber, int boardSize) : base(playerNumber, boardSize)
        {
            _preferredPath = new List<BaseNode>();
            SetUpInMemoryBoard();
        }

        public string PlayerName()
        {
            return "Dumb Dozer";
        }
       
        public override string PlayerType()
        {
            return "Dozer";
        }
        public bool IsAvailableToPlay()
        {
            return true;
        }

        public override Tuple<int, int> SelectHex(Tuple<int, int> opponentMove)
        {
            if (!_memory.Any())
            {
                if (opponentMove != null)
                {
                    Quip("What the hell?  Where'd my memory go?!?");
                }
                SetUpInMemoryBoard();
            }
            if (opponentMove != null)
            {
                // Let's note the enemy's movement
                BaseNode enemyHex =
                    (BaseNode) _memory
                        .FirstOrDefault(hex => hex.X == opponentMove.Item1 
                                               && hex.Y == opponentMove.Item2);

                if (enemyHex != null)
                {
                    enemyHex.Owner = EnemyPlayerNumber;
                    enemyHex.Status = Status.Closed;
                    enemyHex.Parent = null;
                    Quip("Enemy took hex [" + enemyHex.X + "," + enemyHex.Y + "]");
                }
                else
                {
                    Quip("Hmm... No move from opponent?");
                }
            }

            

            if (!havePath || _preferredPath.Any(x => x.Owner == EnemyPlayerNumber ))
            {
                Quip("I need a path...");
                StartOver();
                LookForPath();
                _preferredPath.Reverse();
            }

            if (!_preferredPath.Any())
            {
                Quip("Whelp.  Couldn't find a path.");
            }
            nodeIWant = _preferredPath.FirstOrDefault(x => x.Owner == 0);
            
            if (nodeIWant != null)
            {
                nodeIWant.Owner = PlayerNumber;
                return new Tuple<int, int>(nodeIWant.X, nodeIWant.Y);
            }

            Quip("Pfft.  I give up!  Let's just get a random hex");
            var choice = JustGetARandomHex();
            return new Tuple<int, int>(choice.X, choice.Y);
        }

        private void StartOver()
        {
            
            // Clear the parents
            _memory.ForEach(x =>x.Parent = null);
            // Set everything to untested again
            _memory.ForEach(x => x.Status = Status.Untested);

            Quip("Can't see any open hexes.  Let's make one.");
            // Grab a random opening hex
            BaseNode startingHex = null;

            // Get all the hexes that are unowned
            var availableHexes = _memory
                .Where(x => x.Owner == 0);

            // Now of these hexes, we'd like to start at our board edge
            IEnumerable<BaseNode> availableStartingHexes;
            if (PlayerNumber == 1)
            {
                availableStartingHexes = availableHexes.Where(hex => hex.X == 0 || hex.Owner == PlayerNumber);
            }
            else
            {
                availableStartingHexes = availableHexes.Where(hex => hex.Y == 0 || hex.Owner == PlayerNumber);

            }

            startingHex = availableStartingHexes.OrderByDescending(x => x.Owner).ThenBy(x => x.uniqueness)
                .FirstOrDefault();

            if (startingHex != null)
            {
                startingHex.Status = Status.Open;
                Quip("We's gunna start with [" + startingHex.X + "," + startingHex.Y + "]");
            }
        }

        private bool IsNodeAtBeginning(BaseNode node)
        {
            if (_isHorizontal)
            {
                return node.Y == 0;
            }

            return node.X == 0;
        }

        private bool IsNodeAtEnd(BaseNode node)
        {
            if (_isHorizontal)
            {
                return node.Y == _size - 1;
            }

            return node.X == _size - 1;
        }

        private void LookForPath()
        {
            BaseNode bestLookingNode = null;
            Quip("Looking...");
            
            // GEt the best looking node
            bestLookingNode = _memory
                .OrderBy(x => x.F)
                .ThenBy(x => x.uniqueness)
                .FirstOrDefault(z => z.Status == Status.Open);

            if (bestLookingNode == null)
            {
                return;
            }

            Quip("This node looks promising: [" + bestLookingNode.X + "," + bestLookingNode.Y + "]");

            // CLOSE IT
            bestLookingNode.Status = Status.Closed;

            if (IsNodeAtEnd(bestLookingNode))
            {
                _preferredPath = new List<BaseNode>();
                Quip("Aha!  I found me a path!");
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

        private bool IsMine(BaseNode node)
        {
            return node.Owner == PlayerNumber;
        }

       
        

        protected sealed override void SetUpInMemoryBoard()
        {
            Quip("Ok, let's start this up!");
            _memory = new List<BaseNode>();

            for (int x = 0; x < _size; x++)
            {
                for (int y = 0; y < _size; y++)
                {
                    var newNode = new BaseNode();
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
