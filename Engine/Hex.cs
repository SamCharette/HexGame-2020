namespace Engine
{

    public enum HexOwner
    {
        Empty,
        Player1,
        Player2
    }

    public class Hex
    {
        public int X;
        public int Y;
        public HexOwner Owner;

        public Hex(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}