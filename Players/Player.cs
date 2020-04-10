using Data;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;


namespace Players
{
    public abstract class Player
    {
        protected string _name;
        protected string _codeName;
        protected string _type;
        protected string _defaultName;
        protected string _version;
        protected string _description;

        protected List<BaseNode> Memory { get; set; }
        protected int Size { get; set; }
        public ConcurrentDictionary<string, int> Monitors { get; set; }
        protected int Talkative { get; set; }
        protected string Log { get; set; }
        protected GamePlayer Configuration { get; set; }

        protected void SetVersionNumber(string assemblyName)
        {
            Assembly assembly = Assembly.LoadFrom(assemblyName);
            Version ver = assembly.GetName().Version;
            _version += ver.Major + "." + ver.Minor + "." + ver.Revision;

        }

        public string GetInformation()
        {
            var info = "Code name: " + _codeName + Environment.NewLine
                   + "Version: " + _version + Environment.NewLine
                   + "Type: " + _type + Environment.NewLine
                   + "Description: " + _description + Environment.NewLine;

            foreach (var setting in Configuration.Settings)
            {
                info += setting.Key + ": " + setting.Value + Environment.NewLine;
            }

            return info;
        }
        protected Player(int playerNumber, int boardSize, GamePlayer playerConfig)
        {
            PlayerNumber = playerNumber;
            Size = boardSize;
            Monitors = new ConcurrentDictionary<string, int>();
            Configuration = playerConfig;
            SetVersionNumber(Assembly.GetAssembly(typeof(Player)).ManifestModule.Name);
            RelayPerformanceInformation();
        }

        protected Player()
        {
            throw new NotImplementedException();
        }

        public string DefaultName => _defaultName;
        public string CodeName => _codeName;
        public string Type => _type;
        public string Description => _description;
        public string Version => _version;

        public int PlayerNumber { get; set; }
        public int EnemyPlayerNumber => PlayerNumber == 1 ? 2 : 1;

        protected bool IsHorizontal => PlayerNumber == 2;

        public event EventHandler RelayInformation;

        public string Name
        {
            get => String.IsNullOrEmpty(_name) ? _defaultName : _name;
            set => _name = value;
        }

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

        protected int GetDefault(GamePlayer playerConfig, string settingName, int defaultValue)
        {
            if (playerConfig != null)
            {
                var setting = playerConfig.Settings.FirstOrDefault(x => x.Key == settingName);
                var parseWorked = int.TryParse(setting?.Value, out var value);
                if (parseWorked)
                    return value;
            }
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

        public virtual string GetLog(bool clearAfter = true)
        {
            var tempLog = Log;
            if (clearAfter)
            {
                Log = "";
            }
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
            return _codeName;
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