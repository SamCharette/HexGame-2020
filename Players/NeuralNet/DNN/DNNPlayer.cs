
using Players.Base;
using Players.Common;
using Players.NeuralNet.DNN.Network;

namespace Players.NeuralNet.DNN
{
    public class DNNPlayer : Player
    {
        private Network.Network Brain;
        public DNNPlayer(int playerNumber, int boardSize, Config playerConfig) : base(playerNumber, boardSize, playerConfig)
        {
            _size = boardSize;
            Brain = new Network.Network();
            Brain.AddLayer(new Layer(_size * _size, 0.1, "INPUT"));
            Brain.AddLayer(new Layer(_size, 0.1, "HIDDEN"));
            Brain.AddLayer(new Layer(1, 0.1, "OUTPUT"));

        }
     

    }
}
