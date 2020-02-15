using Players.Base;

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
        public RandomPlayer(int playerNumber, int boardSize) : base(playerNumber, boardSize)
        {
        }
    }
}
