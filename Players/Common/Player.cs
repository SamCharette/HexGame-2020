using System;
using System.Collections.Generic;
using System.Linq;

namespace Players.Base
{
    public abstract class Player
    {
        public string Name { get; set; }
        public int PlayerNumber { get; set; }
        private int EnemyPlayerNumber => PlayerNumber == 1 ? 2 : 1;
        protected int _size;
        protected bool _isHorizontal => PlayerNumber == 2;
        protected List<BaseNode> _memory;
        public int WaitTime = 50;


        protected Player(int playerNumber, int boardSize)
        {
            PlayerNumber = playerNumber;
            _size = boardSize;
            SetUpInMemoryBoard();
        }

        public void GameOver(int winningPlayerNumber)
        {
            _memory = null;

        }

        protected virtual void SetUpInMemoryBoard()
        {
            _memory = new List<BaseNode>();

            for (int x = 0; x < _size; x++)
            {
                for (int y = 0; y < _size; y++)
                {
                    var newNode = new BaseNode();
                    
                    newNode.Row = x;
                    newNode.Column = y;
                    newNode.Owner = 0;
                    _memory.Add(newNode);
                }
            }
        }

        public virtual Tuple<int,int> SelectHex(Tuple<int,int> opponentMove)
        {
            if (opponentMove != null)
            {
                UpdateBoard(EnemyPlayerNumber, opponentMove.Item1, opponentMove.Item2);
            }
            var choice = MakeChoice();
            if (choice != null)
            {
                UpdateBoard(PlayerNumber, choice.Row, choice.Column);
            }
            return choice == null ? null : new Tuple<int, int>(choice.Row, choice.Column);
        }

        private void UpdateBoard(int playerNumber, int x, int y)
        {
            var playerChoice = _memory.FirstOrDefault(node => node.Row == x && node.Column == y);
            if (playerChoice != null)
            {
                playerChoice.Owner = playerNumber;
            }
        }

        private BaseNode MakeChoice()
        {
            var choice =  JustGetARandomHex();
         
            return choice;
        }

        public BaseNode JustGetARandomHex()
        {
            var openNodes = _memory.Where(x => x.Owner == 0);
            System.Threading.Thread.Sleep(WaitTime);
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
            Console.WriteLine(PlayerType() + " (player " + PlayerNumber +") : " + expressionToSay);
        }

    }
}
