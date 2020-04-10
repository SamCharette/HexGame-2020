using Data;
using Players;

namespace RandomPlayer
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
        public RandomPlayer(int playerNumber, int boardSize, GamePlayer playerConfig) : base(playerNumber, boardSize, playerConfig)
        {
            RelayPerformanceInformation();
            Name = playerConfig?.Name ?? "Random";
        }
    }
}
