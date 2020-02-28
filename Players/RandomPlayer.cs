using Players.Base;
using Players.Common;

namespace Players
{
    public class RandomPlayer : Player
    {

        public new string PlayerType()
        {
            return "Random AI";
        }
        public new bool IsAvailableToPlay()
        {
            return true;
        }
        public RandomPlayer(int playerNumber, int boardSize, Config playerConfig) : base(playerNumber, boardSize, playerConfig)
        {
            Name = playerConfig?.name ?? "Random";
        }
    }
}
