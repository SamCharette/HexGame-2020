
using Players.Base;
using Players.Common;

namespace Players.NeuralNet.DNN
{
    public class DNNPlayer : Player
    {
        public DNNPlayer(int playerNumber, int boardSize, Config playerConfig) : base(playerNumber, boardSize, playerConfig)
        {
            _size = boardSize;
            
        }
     

    }
}
