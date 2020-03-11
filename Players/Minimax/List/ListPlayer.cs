using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Players.Common;

namespace Players.Minimax.List
{
    public class ListPlayer : MinimaxPlayer
    {
        public ListMap Memory { get; set; }
        public Tuple<int,int> CurrentChoice { get; set; }
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

            Startup();
        }

        public void Startup()
        {
            Memory = new ListMap(Size);
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

            CurrentChoice = null;
            
            Monitors[CurrentScore] = ThinkAboutTheNextMove(MaxLevels, AbsoluteWorst, AbsoluteBest, true);
            if (CurrentChoice == null)
            { 
                var myPath = GetAPathForMe();
                var hex = myPath.OrderBy(x => x.RandomValue).FirstOrDefault(x => x.Owner == Common.PlayerType.White);
                if (hex != null)
                {
                    CurrentChoice = new Tuple<int, int>(hex.Row, hex.Column);
                }

            }
            if (CurrentChoice == null)
            {
                Monitors[NumberOfRandomMoves]++;
                var hex = RandomHex();
                CurrentChoice = new Tuple<int, int>(hex.Row, hex.Column);
            } else
            {
                Monitors[NumberOfPlannedMoves]++;
            }
            RelayPerformanceInformation();
            // When in doubt, choose random
            Memory.TakeHex(Me, CurrentChoice.Item1, CurrentChoice.Item2);
            return new Tuple<int, int>(CurrentChoice.Item1, CurrentChoice.Item2);
        }
        public ListHex RandomHex()
        {
            var openNodes = Memory.Board.Where(x => x.Owner == Common.PlayerType.White);
            var selectedNode = openNodes.OrderBy(x => x.RandomValue).FirstOrDefault();
            return selectedNode;
        }
        public int ThinkAboutTheNextMove(int depth, int alpha, int beta, bool isMaximizing)
        {
            var score = 0;
            if (depth == 0 || Memory.Board.All(x => x.Owner != Common.PlayerType.White))
            {
                score = ScoreFromBoard();
                Monitors[CurrentScore] = ScoreFromBoard();
                return score;
            }

            var possibleMoves = GetAPathForPlayer(isMaximizing)
                .Where(x => x.Owner == Common.PlayerType.White).ToList();
            possibleMoves.AddRange(GetAPathForPlayer(!isMaximizing)
                .Where(x => x.Owner == Common.PlayerType.White).ToList());
            if (possibleMoves.Any())
            {

                if (isMaximizing)
                {
                    var bestValue = AbsoluteWorst;

                    foreach (var move in possibleMoves.Where(x => x.Owner == Common.PlayerType.White))
                    {
                        Memory.TakeHex(Me, move.Row, move.Column);
                        bestValue = Math.Max(bestValue, ThinkAboutTheNextMove(depth - 1, alpha, beta, false));
                        if (bestValue > alpha)
                        {
                            alpha = bestValue;
                            CurrentChoice = new Tuple<int, int>(move.Row, move.Column);
                        }
                        Monitors[NumberOfNodesChecked]++;
                        Memory.ReleaseHex(move.Row, move.Column);
                        if (beta <= alpha)
                        {
                            Monitors[NumberOfPrunesMade]++;
                            break;
                        }

                    }

                    return bestValue;
                }
                else
                {
                    var bestValue = AbsoluteBest;
                    foreach (var move in possibleMoves.Where(x => x.Owner == Common.PlayerType.White))
                    {

                        Memory.TakeHex(Opponent(), move.Row, move.Column);
                        bestValue = Math.Min(bestValue, ThinkAboutTheNextMove(depth - 1, alpha, beta, true));
                        beta = Math.Min(beta, bestValue);
                        Monitors[NumberOfNodesChecked]++;
                        Memory.ReleaseHex(move.Row, move.Column);
                        if (beta <= alpha)
                        {
                            Monitors[NumberOfPrunesMade]++;
                            break;
                        }
                    }
                    return bestValue;
                }
            }

            score = ScoreFromBoard();
            Monitors[CurrentScore] = ScoreFromBoard();
            return score;
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

        public List<ListHex> GetAPathForPlayer(bool isMaximizing)
        {
            return isMaximizing ? GetAPathForMe() : GetAPathForOpponent();
        }

        public List<ListHex> GetAPathForMe()
        {
            var start = Me == Common.PlayerType.Blue ? Memory.Top : Memory.Left;
            var end = Me == Common.PlayerType.Blue ? Memory.Bottom : Memory.Right;
            return FindPath(start, end, Me);
        }

        public List<ListHex> GetAPathForOpponent()
        {
            var start = Opponent() == Common.PlayerType.Blue ? Memory.Top : Memory.Left;
            var end = Opponent() == Common.PlayerType.Blue ? Memory.Bottom : Memory.Right;
            return FindPath(start, end, Opponent());
        }

        public int ScoreFromBoard()
        {
            // Get the player score
            var playerScore = 0;
            var opponentScore = 0;
            if (Me == Common.PlayerType.Blue)
            {
                var path = FindPath(Memory.Top, Memory.Bottom, Me);
                playerScore = Size - path.Count(x => x.Owner == Common.PlayerType.White);
                var opponentPath = FindPath(Memory.Left, Memory.Right, Opponent());
                opponentScore = Size - opponentPath.Count(x => x.Owner == Common.PlayerType.White);
            } else
            {
                var path = FindPath(Memory.Left, Memory.Right, Me);
                playerScore = Size - path.Count(x => x.Owner == Common.PlayerType.White);
                var opponentPath = FindPath(Memory.Top, Memory.Bottom, Opponent());
                opponentScore = Size - opponentPath.Count(x => x.Owner == Common.PlayerType.White);

            }

            return playerScore - opponentScore;
        }

        public List<ListHex> FindPath(ListHex start, ListHex end, PlayerType player)
        {
        
            Memory.CleanPathingVariables();
       
            var neighbours = Memory.GetTraversablePhysicalNeighbours(start, player);
            neighbours.ForEach(x => x.Status = Status.Open);
            return PathBetween(start, end, player);

        }

        public List<ListHex> PathBetween(ListHex start, ListHex end, Common.PlayerType player)
        {
            // Get the best looking node
            var bestLookingHex = Memory.Board
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

            if (Memory.ArePhysicalNeighbours(bestLookingHex, end))
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

           
            var neighbours = Memory.GetTraversablePhysicalNeighbours(bestLookingHex, Me);

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
            return PathBetween(start, end, player);
        }

    }
}
