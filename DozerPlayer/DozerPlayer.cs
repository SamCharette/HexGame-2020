using System;
using System.Collections.Generic;
using System.Linq;
using Players;

namespace DozerPlayer
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
        private bool havePath = false;
        private BaseNode nodeIWant;
        private int costToMoveToClaimedNode;
        private int costToMoveToUnclaimedNode;
        private int costPerNodeTillEnd;
        private int EnemyPlayerNumber
        {
            get { return PlayerNumber == 1 ? 2 : 1; }
        }

        public override string CodeName()
        {
            return "Dozer";
        }

        public new void GameOver(int winningPlayerNumber)
        {
            base.GameOver(winningPlayerNumber);
            if (winningPlayerNumber == PlayerNumber)
            {
                Quip("Aww yeah, I win again!");
            } else
            {
                Quip("Did NOT see that coming.  Probably because I can't think ahead.  Thanks for that, SAM.");
            }

            _preferredPath = null;
        }
       
        public DozerPlayer(int playerNumber, int boardSize, Config playerConfig) : base(playerNumber, boardSize, playerConfig)
        {
            _preferredPath = new List<BaseNode>();
            costPerNodeTillEnd = GetDefault(playerConfig, "costPerNodeTillEnd", 1000);
            costToMoveToUnclaimedNode = GetDefault(playerConfig, "costToMoveToUnclaimedNode", 100);
            costToMoveToClaimedNode = GetDefault(playerConfig, "costToMoveToClaimedNode", 0);
            Talkative = Convert.ToInt32((string) playerConfig.talkative);
            Name = playerConfig.name;
            RelayPerformanceInformation();
            SetUpInMemoryBoard();
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
            if (!Memory.Any())
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
                    (BaseNode) Memory
                        .FirstOrDefault(hex => hex.Row == opponentMove.Item1 
                                               && hex.Column == opponentMove.Item2);

                if (enemyHex != null)
                {
                    enemyHex.Owner = EnemyPlayerNumber;
                    enemyHex.Status = Status.Closed;
                    enemyHex.Parent = null;
                    Quip("Enemy took hex [" + enemyHex.Row + "," + enemyHex.Column + "]");
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
                return new Tuple<int, int>(nodeIWant.Row, nodeIWant.Column);
            }

            Quip("Pfft.  I give up!  Let's just get a random hex");
            var choice = JustGetARandomHex();
            return new Tuple<int, int>(choice.Row, choice.Column);
        }

        private void StartOver()
        {
            
            // Clear the parents
            Memory.ForEach(x =>x.Parent = null);
            // Set everything to untested again
            Memory.ForEach(x => x.Status = Status.Untested);

            Quip("Can't see any open hexes.  Let's make one.");
            // Grab a random opening hex
            BaseNode startingHex = null;

            // Get all the hexes that are friendly
            var availableHexes = Memory
                .Where(x => x.Owner != EnemyPlayerNumber);

            // Now of these hexes, we'd like to start at our board edge
            IEnumerable<BaseNode> availableStartingHexes;
            if (PlayerNumber == 1)
            {
                availableStartingHexes = availableHexes.Where(hex => hex.Row == 0 || hex.Owner == PlayerNumber);
            }
            else
            {
                availableStartingHexes = availableHexes.Where(hex => hex.Column == 0 || hex.Owner == PlayerNumber);

            }

            startingHex = 
                availableStartingHexes
                .OrderByDescending(x => x.Owner)
                .ThenBy(x => x.RandomValue)
                .FirstOrDefault();

            if (startingHex != null)
            {
                startingHex.Status = Status.Open;
                Quip("We's gunna start with [" + startingHex.Row + "," + startingHex.Column + "]");
            }
        }

        private bool IsNodeAtBeginning(BaseNode node)
        {
            if (IsHorizontal)
            {
                return node.Column == 0;
            }

            return node.Row == 0;
        }

        private bool IsNodeAtEnd(BaseNode node)
        {
            if (IsHorizontal)
            {
                return node.Column == Size - 1;
            }

            return node.Row == Size - 1;
        }

        private void LookForPath()
        {
            BaseNode bestLookingNode = null;
            Quip("Looking...");
            
            // GEt the best looking node
            bestLookingNode = Memory
                .OrderBy(x => x.F())
                .ThenBy(x => x.RandomValue)
                .FirstOrDefault(z => z.Status == Status.Open);

            if (bestLookingNode == null)
            {
                return;
            }

            Quip("This node looks promising: [" + bestLookingNode.Row + "," + bestLookingNode.Column + "]");

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

            var neighbours = Memory
                .Where(x => bestLookingNode.CanWalkTo(x)).ToList();

            foreach (var node in neighbours)
            {
                if (node.Owner != bestLookingNode.EnemyPlayerNumber())
                {
                    if (node.Status == Status.Open)
                    {
                        if (node.G > bestLookingNode.G + (node.Owner == PlayerNumber ? costToMoveToClaimedNode : costToMoveToUnclaimedNode))
                        {
                            node.Parent = bestLookingNode;
                            node.G = bestLookingNode.G + (node.Owner == PlayerNumber ? costToMoveToClaimedNode : costToMoveToUnclaimedNode); ;
                            node.H = (IsHorizontal ? Size - 1 - node.Column : Size - 1 - node.Row) * costPerNodeTillEnd;
                        }
                    }
                    else if (node.Status == Status.Untested)
                    {
                        node.Status = Status.Open;
                        node.Parent = bestLookingNode;
                        node.G = bestLookingNode.G + (node.Owner == PlayerNumber ? costToMoveToClaimedNode : costToMoveToUnclaimedNode);
                        node.H = (IsHorizontal ? Size - 1 - node.Column : Size - 1 - node.Row) * costPerNodeTillEnd;
                    }
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
            Memory = new List<BaseNode>();

            for (int row = 0; row < Size; row++)
            {
                for (int column = 0; column < Size; column++)
                {
                    var newNode = new BaseNode();
                    {
                        newNode.Row = row;
                        newNode.Column = column;
                        newNode.Owner = 0;
                        newNode.Status = Status.Untested;
                        newNode.H = PlayerNumber == 1 ? Size - 1 - column : Size - 1 - row;
                        newNode.RandomValue = Guid.NewGuid();

                    }
                    Memory.Add(newNode);
                }
            }
        }

    }
}
