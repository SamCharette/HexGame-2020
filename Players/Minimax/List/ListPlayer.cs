using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Players.Common;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Threading.Tasks;
using System.Xml;

namespace Players.Minimax.List
{
    /*
    *  This player is currently (Mar-16-2020) doing ok.  It's slow though.
    *
    *  Things to look at:
    *
    *  Scoring - better scoring function?
    *  Pathfinding - called way too often, I bet.  Should only restart pathfinding
    *      if current path is broken (like dozer)
    *  Final Move Recognition - Should be able to identify when there is a final move it can
    *       make so that it doesn't lose by choosing a different one
    *  Threatened Game State - should be able to identify when the opponent is one
    *       move away from winning so that it can stop them.
    *
    *
    */
    public class ListPlayer : MinimaxPlayer
    {
        public ListMap Memory { get; set; }
        public Tuple<int,int> CurrentChoice { get; set; }
        public Queue<ListHex> MoveQueue { get; set; }
        public List<ListHex> ProposedPath { get; set; }
        public HashSet<Tuple<ListHex,int>> MoveScores { get; set; }

        public int MaximumThreads => Environment.ProcessorCount / 2;
        private object WorkingLock;
        public List<Task> Threads { get; set; }

        public int CurrentThreadsInUse { get; set; }
        public ListPlayer(int playerNumber, int boardSize, Config playerConfig) : base(playerNumber, boardSize, playerConfig)
        {
            PlayerNumber = playerNumber;
            Me = PlayerNumber == 1 ? Common.PlayerType.Blue : Common.PlayerType.Red;
            Size = boardSize;
            
            MaxLevels = GetDefault(playerConfig, "maxLevels", 5);
            CostPerNodeTillEnd = GetDefault(playerConfig, "costPerNodeTillEnd", 5);
            CostToMoveToUnclaimedNode = GetDefault(playerConfig, "costToMoveToUnclaimedNode", 2);
            CostToMoveToClaimedNode = GetDefault(playerConfig, "costToMoveToClaimedNode", 0);
            talkative = Convert.ToInt32(playerConfig.talkative);
            
            Name = playerConfig.name;
            Monitors.Add(MovesExamined, 0);
            Monitors.Add(MovesExaminedThisTurn, 0);
            Monitors.Add(CurrentScore, 0);
            Monitors.Add(AverageTimeToDecision, 0);
            Monitors.Add(TotalTimeThinking, 0);
            Monitors.Add(NumberOfPlannedMoves, 0);
            Monitors.Add(NumberOfRandomMoves, 0);
            Monitors.Add(NumberOfNodesChecked, 0);
            Monitors.Add(NumberOfPrunesMade, 0);
            WorkingLock = new object();

            Startup();
        }
        public override string PlayerType()
        {
            return "Minimax AI";
        }
        public void Startup()
        {
            Memory = new ListMap(Size);
            Memory.Name = "Memory";
            Monitors[MovesExamined] = 0;
            Monitors[MovesExaminedThisTurn] = 0;
            Monitors[CurrentScore] = 0;
            Monitors[AverageTimeToDecision] = 0;
            Monitors[TotalTimeThinking] = 0;
            Monitors[NumberOfPlannedMoves] = 0;
            Monitors[NumberOfRandomMoves] = 0;
            Monitors[NumberOfNodesChecked] = 0;
            Monitors[NumberOfPrunesMade] = 0;
            CurrentChoice = null;
        }

        public override Tuple<int, int> SelectHex(Tuple<int, int> opponentMove)
        {
            if (opponentMove != null)
            {
                Memory.TakeHex(Opponent(), opponentMove.Item1, opponentMove.Item2);
            }
            
            var turnStartTime = DateTime.Now;

            CurrentChoice = null;

            if (CurrentChoice == null)
            {
                MoveQueue = new Queue<ListHex>();
                var threadsAllowed = Math.Min(Environment.ProcessorCount / 2, 1);
                CurrentThreadsInUse = 0;
                Monitors[MovesExaminedThisTurn] = 0;
                Threads = new List<Task>();
                ProposedPath = GetAPathForMe(Memory);
                MoveScores = new HashSet<Tuple<ListHex, int>>();
                var theirPath = GetAPathForOpponent(Memory);
                var possibleMoves = GetPossibleMovesFrom(ProposedPath, theirPath, true);
               
                foreach (var move in possibleMoves)
                {
                    Quip("Enqueueing move: " + move);
                    MoveQueue.Enqueue(move);
                }

                var i = 0;
                while (MoveQueue.Any())
                {
                    i++;
                    var move = MoveQueue.Dequeue();
                    var newMap = new ListMap(Memory);
                    newMap.Name = "Thread Memory " + i;
                    var newMapMove = newMap.Board.FirstOrDefault(x => x.Row == move.Row && x.Column == move.Column);
                    Threads.Add(Task.Factory.StartNew(() =>StartSearchingForScore(newMap, newMapMove)));
                }

                Task.WaitAll(Threads.ToArray());

                CurrentChoice = MoveScores?.OrderByDescending(x => x.Item2)?.FirstOrDefault()?.Item1.ToTuple();
                Monitors[MovesExamined] += Monitors[MovesExaminedThisTurn];
            }
            RelayPerformanceInformation();

            if (CurrentChoice == null)
            {
                Monitors[NumberOfRandomMoves]++;
                var hex = RandomHex();
                CurrentChoice = new Tuple<int, int>(hex.Row, hex.Column);
            } else
            {
                Monitors[NumberOfPlannedMoves]++;
            }
            // When in doubt, choose random
            Memory.TakeHex(Me, CurrentChoice);
            var timeTaken =(int) DateTime.Now.Subtract(turnStartTime).TotalMilliseconds;

            Monitors[TotalTimeThinking] += timeTaken;
            Monitors[AverageTimeToDecision] = Monitors[TotalTimeThinking] / (Monitors[NumberOfRandomMoves] + Monitors[NumberOfPlannedMoves]);
            RelayPerformanceInformation();
            return CurrentChoice;
        }

        public void StartSearchingForScore(ListMap searchInThisMap, ListHex move)
        {

            RelayPerformanceInformation();
            Monitors[MovesExaminedThisTurn]++;
            //CurrentThreadsInUse++;

        
            Quip("I'm pretty sure I got in now.");
            var score = ThinkAboutTheNextMove(searchInThisMap, ProposedPath, move, MaxLevels, AbsoluteWorst, AbsoluteBest, false);
            MoveScores.Add(new Tuple<ListHex, int>(move, score));
            //CurrentThreadsInUse--;
            
        }

        public ListHex RandomHex()
        {
            var openNodes = Memory.Board.Where(x => x.Owner == Common.PlayerType.White);
            var selectedNode = openNodes.OrderBy(x => x.RandomValue).FirstOrDefault();
            return selectedNode;
        }
        public int ThinkAboutTheNextMove(
            ListMap map,
            List<ListHex> path,
            ListHex currentMove,
            int depth, 
            int alpha, 
            int beta, 
            bool isMaximizing)
        {
            if (depth == 0 || Memory.Board.All(x => x.Owner != Common.PlayerType.White))
            {
                return ScoreFromBoard(map);
            }
            Memory.TakeHex(isMaximizing ? Me : Opponent(), currentMove);

            var myPath = GetAPathForPlayer(map, isMaximizing);

            var possibleMoves = GetPossibleMovesFrom( path, myPath, isMaximizing);
            if (!possibleMoves.Any())
            {
                Memory.ReleaseHex(currentMove);
                return ScoreFromBoard(map);
            }

            if (isMaximizing)
            {

                foreach (var move in possibleMoves)
                {
                    alpha = Math.Max(alpha, ThinkAboutTheNextMove(map, myPath, move, depth - 1, alpha, beta, false));
                    Monitors[NumberOfNodesChecked]++;
                    if (beta <= alpha)
                    {
                        Monitors[NumberOfPrunesMade]++;
                        break;
                    }

                }
                Memory.ReleaseHex(currentMove);

                return alpha;
            }
            else
            {
                foreach (var move in possibleMoves)
                {
                    beta = Math.Min(beta, ThinkAboutTheNextMove(map, myPath, move, depth - 1, alpha, beta, true));
                    Monitors[NumberOfNodesChecked]++;

                    if (beta <= alpha)
                    {
                        Monitors[NumberOfPrunesMade]++;
                        break;
                    }
                }

                Memory.ReleaseHex(currentMove);
                return beta;
            }
            

        }

        private List<ListHex> GetPossibleMovesFrom(List<ListHex> myPath, List<ListHex> theirPath, bool isMaximizing)
        {
            var possibleMoves = new List<ListHex>();
            var bothLike = myPath.Where(theirPath.Contains).ToList();

            possibleMoves.AddRange(bothLike);
            possibleMoves.AddRange(myPath.Where(x => !bothLike.Contains(x)));
            possibleMoves.AddRange(theirPath.Where(x => !bothLike.Contains(x)));
            return possibleMoves.Where(x => x.Owner == Common.PlayerType.White).ToList();
        }

        public PlayerType CurrentlySearchingAs(bool isMaximizing)
        {
            return isMaximizing ? Me : Opponent();
        }
        public override void GameOver(int winningPlayerNumber)
        {
            RelayPerformanceInformation();
            Memory = null;
        }

        public PlayerType Opponent()
        {
            if (Me == Common.PlayerType.Blue)
            {
                return Common.PlayerType.Red;
            }

            return Common.PlayerType.Blue;
        }

        public List<ListHex> GetAPathForPlayer(ListMap map, bool isMaximizing)
        {
            map.CleanPathingVariables();

            return isMaximizing ? GetAPathForMe(map) : GetAPathForOpponent(map);
        }

        public List<ListHex> GetAPathForMe(ListMap map)
        {
            var start = Me == Common.PlayerType.Blue ? map.Top : map.Left;
            var end = Me == Common.PlayerType.Blue ? map.Bottom : map.Right;
            return FindPath(map, start, end, Me);
        }

        public List<ListHex> GetAPathForOpponent(ListMap map)
        {
            var start = Opponent() == Common.PlayerType.Blue ? map.Top : map.Left;
            var end = Opponent() == Common.PlayerType.Blue ? map.Bottom : map.Right;
            return FindPath(map, start, end, Opponent());
        }

        public int ScoreFromBoard(ListMap map)
        {
            // Get the player score
            var playerScore = 0;
            var opponentScore = 0;


            //if (Me == Common.PlayerType.Blue)
            //{
            //    var path = FindPath(Memory.Top, Memory.Bottom, Me);
            //    playerScore = Size - path.Count(x => x.Owner == Common.PlayerType.White);
            //    var opponentPath = FindPath(Memory.Left, Memory.Right, Opponent());
            //    opponentScore = Size - opponentPath.Count(x => x.Owner == Common.PlayerType.White);
            //} else
            //{
            //    var path = FindPath(Memory.Left, Memory.Right, Me);
            //    playerScore = Size - path.Count(x => x.Owner == Common.PlayerType.White);
            //    var opponentPath = FindPath(Memory.Top, Memory.Bottom, Opponent());
            //    opponentScore = Size - opponentPath.Count(x => x.Owner == Common.PlayerType.White);

            //}
            var path = FindPath(map, map.Top, map.Bottom, Me);
            playerScore = Size - path.Count(x => x.Owner == Common.PlayerType.White);
            var opponentPath = FindPath(map, map.Left, map.Right, Opponent());
            opponentScore = Size - opponentPath.Count(x => x.Owner == Common.PlayerType.White);

            return playerScore - opponentScore;
        }

        private bool CanIWinWithThisMove(ListHex hex)
        {

            if (hex != null)
            {
                Memory.TakeHex(Me, hex.Row, hex.Column);
                var canIWinHere = IsWinningMove(Me);
                Memory.ReleaseHex( hex.Row, hex.Column);
                if (canIWinHere)
                {
                    Quip("Yes, I CAN win if I get here (" + hex.Row + ", " + hex.Column + ")");
                }
                return canIWinHere;
            }

            return false;
        }

        private bool CanILoseIfIDontTakeThisHex(ListHex hex)
        {
           
            if (hex != null)
            {
                Memory.TakeHex(Opponent(), hex.Row, hex.Column);
                var canILoseHere = IsWinningMove(Opponent());
                Memory.ReleaseHex(hex.Row, hex.Column);
                if (canILoseHere)
                {
                    Quip("Bleh, THEY can win if they get here (" + hex.Row + ", " + hex.Column + ")");
                }
                return canILoseHere;
            }

            return false;
        }

        public bool IsWinningMove(PlayerType player)
        {
            var start = player == Common.PlayerType.Blue ? Memory.Top : Memory.Left;
            var end = player == Common.PlayerType.Blue ? Memory.Bottom : Memory.Right;
           
            return start.IsAttachedTo(end);
        }

        public List<ListHex> FindPath(ListMap map, ListHex start, ListHex end, PlayerType player)
        {

            map.CleanPathingVariables();
       
            var neighbours = map.GetTraversablePhysicalNeighbours(start, player);
            neighbours.ForEach(x => x.Status = Status.Open);
            var path = PathBetween(map, start, end, player);

            return path;

        }

        public List<ListHex> PathBetween(ListMap map, ListHex start, ListHex end, Common.PlayerType player)
        {
            // Get the best looking node
            var bestLookingHex = map.Board
                .OrderBy(x => x.F)
                .ThenBy(x => x.RandomValue)
                .FirstOrDefault(z => z.Status == Status.Open);

            if (bestLookingHex == null)
            {
                if (start.Status == Status.Untested || start.Status == Status.Open)
                {
                    bestLookingHex = start;
                }
                else
                {
                    return new List<ListHex>();
                }
               
            }

            if (map.ArePhysicalNeighbours(bestLookingHex, end))
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

           
            var neighbours = map.GetTraversablePhysicalNeighbours(bestLookingHex, Me);

            foreach (var node in neighbours)
            {
                if (node.Owner != Opponent())
                {
                    if (node.Status == Status.Open)
                    {
                        if (node.G > bestLookingHex.G + (node.Owner == player ? CostToMoveToClaimedNode : CostToMoveToUnclaimedNode))
                        {
                            node.Parent = bestLookingHex;
                            node.G = bestLookingHex.G + (node.Owner == player ? CostToMoveToClaimedNode : CostToMoveToUnclaimedNode); ;
                            node.H = (player == Common.PlayerType.Red ? _size - 1 - node.Column : _size - 1 - node.Row) * CostPerNodeTillEnd;
                        }
                    }
                    else if (node.Status == Status.Untested)
                    {
                        node.Status = Status.Open;
                        node.Parent = bestLookingHex;
                        node.G = bestLookingHex.G + (node.Owner == player ? CostToMoveToClaimedNode : CostToMoveToUnclaimedNode);
                        node.H = (player == Common.PlayerType.Red ? _size - 1 - node.Column : _size - 1 - node.Row) * CostPerNodeTillEnd;
                    }
                }


            }
            return PathBetween(map, start, end, player);
        }

    }
}
