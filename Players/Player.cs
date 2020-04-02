using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;


namespace Players
{
    public abstract class Player
    {
        protected List<BaseNode> Memory { get; set; }
        protected int Size { get; set; }
        private ConcurrentDictionary<string, int> Monitors { get; set; }
        protected int Talkative { get; set; }
        private string Log { get; set; }
        
        protected Player(int playerNumber, int boardSize, Config playerConfig)
        {
            PlayerNumber = playerNumber;
            Size = boardSize;
            Monitors = new ConcurrentDictionary<string, int>();
            RelayPerformanceInformation();
        }

        protected Player()
        {
            throw new NotImplementedException();
        }

        public string Name { get; set; }
        public int PlayerNumber { get; set; }
        protected int EnemyPlayerNumber => PlayerNumber == 1 ? 2 : 1;

        protected bool IsHorizontal => PlayerNumber == 2;

        public event EventHandler RelayInformation;

        protected void RelayPerformanceInformation()
        {
            var args = new PerformanceEventArgs
            {
                PlayerNumber = PlayerNumber, 
                Counters = new ConcurrentDictionary<string, int>()
            };
            foreach (var item in Monitors)
            {
                args.Counters[item.Key] = item.Value;
            }
            RelayInformation?.Invoke(this, args);
        }

        public virtual void GameOver(int winningPlayerNumber)
        {
            Memory = null;
        }

        protected int GetDefault(Config playerConfig, string settingName, int defaultValue)
        {
            var setting = playerConfig?.settings?.FirstOrDefault(x => x.key == settingName);
            var parseWorked = int.TryParse(setting?.value, out var value);
            if (parseWorked)
                return value;
            return defaultValue;
        }

        protected virtual void SetUpInMemoryBoard()
        {
            Memory = new List<BaseNode>();

            for (var x = 0; x < Size; x++)
            for (var y = 0; y < Size; y++)
            {
                var newNode = new BaseNode {Row = x, Column = y, Owner = 0};

                Memory.Add(newNode);
            }
        }

        protected virtual string GetLog()
        {
            var tempLog = Log;
            Log = "";
            return tempLog;
        }

        public virtual Tuple<int, int> SelectHex(Tuple<int, int> opponentMove)
        {
            if (opponentMove != null) UpdateBoard(EnemyPlayerNumber, opponentMove.Item1, opponentMove.Item2);
            var choice = MakeChoice();
            if (choice != null) UpdateBoard(PlayerNumber, choice.Row, choice.Column);
            return choice == null ? null : new Tuple<int, int>(choice.Row, choice.Column);
        }

        private void UpdateBoard(int playerNumber, int x, int y)
        {
            var playerChoice = Memory.FirstOrDefault(node => node.Row == x && node.Column == y);
            if (playerChoice != null) playerChoice.Owner = playerNumber;
        }

        private BaseNode MakeChoice()
        {
            var choice = JustGetARandomHex();

            return choice;
        }

        protected virtual BaseNode JustGetARandomHex(List<BaseNode> board = null)
        {
            var searchSpace = board ?? Memory;
            var openNodes = searchSpace.Where(x => x.Owner == 0);
            var selectedNode = openNodes.OrderBy(x => Guid.NewGuid()).FirstOrDefault();


            return selectedNode;
        }

        public virtual string PlayerType()
        {
            return "Base Player";
        }

        public bool IsAvailableToPlay()
        {
            return false;
        }

        protected void Quip(string expressionToSay, int level = 1, bool hasNewLine = true)
        {
            if (Talkative >= level)
            {
                Log += expressionToSay + (hasNewLine ? Environment.NewLine : "");
                #if DEBUG
                    Console.Write(Name + " " + PlayerType() + " (player " + PlayerNumber + ") : " + expressionToSay);
                    if (hasNewLine)
                    {
                        Console.Write(Environment.NewLine);
                    }
                #endif
            }

        }
    }
}