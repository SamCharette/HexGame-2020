using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Players.Common
{
    public abstract class Player
    {
        protected List<BaseNode> _memory;
        protected int _size;
        public ConcurrentDictionary<string, int> Monitors = new ConcurrentDictionary<string, int>();
        public int talkative;
        public int WaitTime = 50;


        protected Player(int playerNumber, int boardSize, Config playerConfig)
        {
            PlayerNumber = playerNumber;
            _size = boardSize;
            RelayPerformanceInformation();
            SetUpInMemoryBoard();
        }

        public string Name { get; set; }
        public int PlayerNumber { get; set; }
        private int EnemyPlayerNumber => PlayerNumber == 1 ? 2 : 1;
        protected bool _isHorizontal => PlayerNumber == 2;
        public event EventHandler RelayInformation;

        public void RelayPerformanceInformation()
        {
            var args = new PerformanceEventArgs();
            args.PlayerNumber = PlayerNumber;
            args.Counters = new ConcurrentDictionary<string, int>();
            foreach (var item in Monitors) args.Counters[item.Key] = item.Value;
            RelayInformation?.Invoke(this, args);
        }

        public virtual void GameOver(int winningPlayerNumber)
        {
            _memory = null;
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
            _memory = new List<BaseNode>();

            for (var x = 0; x < _size; x++)
            for (var y = 0; y < _size; y++)
            {
                var newNode = new BaseNode();

                newNode.Row = x;
                newNode.Column = y;
                newNode.Owner = 0;
                _memory.Add(newNode);
            }
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
            var playerChoice = _memory.FirstOrDefault(node => node.Row == x && node.Column == y);
            if (playerChoice != null) playerChoice.Owner = playerNumber;
        }

        private BaseNode MakeChoice()
        {
            var choice = JustGetARandomHex();

            return choice;
        }

        public virtual BaseNode JustGetARandomHex()
        {
            var openNodes = _memory.Where(x => x.Owner == 0);
            Thread.Sleep(WaitTime);
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

        public void Quip(string expressionToSay)
        {
            if (talkative == 1)
                Console.WriteLine(Name + " " + PlayerType() + " (player " + PlayerNumber + ") : " + expressionToSay);
        }
    }
}