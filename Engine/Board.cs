using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Engine
{
    //public class Hex
    //{
    //    public int X;
    //    public int Y;
    //    public int Owner;
    //    public int G;
    //    public int H;
    //    public int F => G + H;
    //    public HexState State;
    //    public Hex Parent;

    //    public Hex(int x, int y)
    //    {
    //        X = x;
    //        Y = y;
    //        Owner = 0;
    //        State = HexState.Untested;
    //    }
    //}

    //public enum HexState
    //{
    //    Open,
    //    Closed,
    //    Untested
    //}
    //public enum Direction
    //{
    //    TopRight,
    //    Right,
    //    BottomRight,
    //    BottomLeft,
    //    Left,
    //    TopLeft
    //}
    //public class Board
    //{
    //    public List<Hex> Spaces;
    //    public List<Hex> BestPath;
    //    public int Size;
    //    public Hex ClickedHex;
    //    private int _costToMove = 1;
    //    public Board(int size = 11)
    //    {
    //        Size = size;
    //        Spaces = new List<Hex>();
    //        for (var i = 0; i < size; i++)
    //        {
    //            for (var j = 0; j < size; j++)
    //            {
    //                var hex = new Hex(i, j);
    //                Spaces.Add(hex);
    //            }
    //        }
    //    }

    //    public bool IsHexAtBeginning(Hex hex, bool isHorizontal)
    //    {
    //        if (isHorizontal)
    //        {
    //            return hex.Y == 0;
    //        }

    //        return hex.X == 0;
    //    }

    //    public bool IsHexAtEnd(Hex hex, bool isHorizontal)
    //    {
    //        if (isHorizontal)
    //        {
    //            return hex.Y == Size - 1;
    //        }

    //        return hex.X == Size - 1;
    //    }

    //    public List<Hex> GetFriendlyNeighbours(int x, int y, int player)
    //    {
    //        var allNeighbours = GetNeighbours(x, y);
    //        return allNeighbours?.Where(hex => hex.Owner == player)
    //            .ToList();
    //    }

    //    public List<Hex> GetFriendlyPathableNeighbours(int x, int y, int player)
    //    {
    //        var allNeighbours = GetNeighbours(x, y);
    //        return allNeighbours?.Where(hex => hex.Owner == player && hex.State != HexState.Closed)
    //            .ToList();
    //    }

    //    public List<Hex> GetEnemyNeighbours(int x, int y, int player)
    //    {
    //        var allNeighbours = GetNeighbours(x, y);
    //        return allNeighbours?.Where(hex => hex.Owner != player)
    //            .ToList();
    //    }

    //    public Hex GetNeighbourAt(Direction direction, int x, int y)
    //    {
    //        switch (direction)
    //        {
    //            case Direction.TopRight:
    //                return HexAt(x + 1, y - 1);
    //            case Direction.Right:
    //                return HexAt(x + 1, y);
    //            case Direction.BottomRight:
    //                return HexAt(x , y + 1);
    //            case Direction.BottomLeft:
    //                return HexAt(x - 1, y + 1);
    //            case Direction.Left:
    //                return HexAt(x - 1, y );
    //            case Direction.TopLeft:
    //                return HexAt(x , y - 1);

    //            default:
    //                return null;
    //        }

    //    }

    //    public List<Hex> GetNeighbours(int x, int y)
    //    {
    //        var neighbours = new List<Hex>();

    //        var neighbour = GetNeighbourAt(Direction.TopRight, x, y);
    //        if (neighbour != null)
    //        {
    //            neighbours.Add(neighbour);
    //        }
    //        neighbour = GetNeighbourAt(Direction.Right, x, y);
    //        if (neighbour != null)
    //        {
    //            neighbours.Add(neighbour);
    //        }
    //        neighbour = GetNeighbourAt(Direction.BottomRight, x, y);
    //        if (neighbour != null)
    //        {
    //            neighbours.Add(neighbour);
    //        }
    //        neighbour = GetNeighbourAt(Direction.BottomLeft, x, y);
    //        if (neighbour != null)
    //        {
    //            neighbours.Add(neighbour);
    //        }
    //        neighbour = GetNeighbourAt(Direction.Left, x, y);
    //        if (neighbour != null)
    //        {
    //            neighbours.Add(neighbour);
    //        }
    //        neighbour = GetNeighbourAt(Direction.TopLeft, x, y);
    //        if (neighbour != null)
    //        {
    //            neighbours.Add(neighbour);
    //        }
            
    //        return neighbours.ToList();
    //    }

    //    public Hex HexAt(int x, int y)
    //    {
            
    //        if (!(x >= 0 && x < Size && y >= 0 && y < Size))
    //        {
    //            return null;
    //        }
    //        return Spaces.FirstOrDefault(hex => hex.X == x && hex.Y == y);
    //    }

    //    public bool TakeHex(int x, int y, int playerNumber)
    //    {
    //        var hexToTake = HexAt(x, y);
    //        if (hexToTake != null && hexToTake.Owner == 0)
    //        {
    //            hexToTake.Owner = playerNumber;
    //            return true;
    //        }

    //        return false;
    //    }

    //    public bool CheckHex(int x, int y)
    //    {
    //        return HexAt(x, y) != null;
    //    }

    //    public bool CheckHexForPlayer(int x, int y, int player)
    //    {

    //        var hex = HexAt(x, y);
    //        return hex?.Owner != null && hex.Owner == player;
    //    }

    //    public void FindBestPath(bool isHorizontal)
    //    {
    //        var startHexes = new List<Hex>();
    //        foreach (var hex in Spaces)
    //        {
    //            hex.State = HexState.Untested;
    //            hex.Parent = null;
    //            hex.G = 0;
    //            hex.H = 0;
    //            if (isHorizontal)
    //            {
    //                hex.H = Size - 1 - hex.Y;
    //                if (hex.Y == 0 && hex.Owner == 2)
    //                {
    //                    startHexes.Add(hex);
    //                }
    //            }
    //            else
    //            {
    //                hex.H = Size - 1 - hex.X;
    //                if (hex.X == 0 && hex.Owner == 1)
    //                {
    //                    startHexes.Add(hex);
    //                }
    //            }
    //        }
    //        BestPath = new List<Hex>();
    //        foreach (var hex in startHexes)
    //        {
    //            hex.State = HexState.Open;
    //        }

    //        KeepLooking(isHorizontal);
    //    }

    //    public void KeepLooking(bool isHorizontal)
    //    {
    //        if (!Spaces.Any(hex => hex.State == HexState.Open))
    //        {
    //            Console.WriteLine("Something happened.  No more open spaces to look into");
    //            BestPath = new List<Hex>();
    //            return;
    //        }

    //        var bestHex = Spaces.OrderBy(hex => hex.F).FirstOrDefault(y => y.State == HexState.Open);
    //        bestHex.State = HexState.Closed;

    //        if (IsHexAtEnd(bestHex, isHorizontal))
    //        {
    //            Console.WriteLine("Found da bestest path");
    //            var parentHex = bestHex;
    //            while (parentHex != null)
    //            {
    //                BestPath.Add(parentHex);
    //                parentHex = parentHex.Parent;
    //            }

    //            return;
    //        }

    //        var neighbours = GetFriendlyPathableNeighbours(bestHex.X, bestHex.Y, bestHex.Owner);
    //        foreach (var neighbour in neighbours)
    //        {
    //            if (neighbour.State == HexState.Open && neighbour.G > bestHex.G + _costToMove)
    //            {
    //                neighbour.Parent = bestHex;
    //                neighbour.G = bestHex.G + _costToMove;
    //            }
    //            else
    //            {
    //                neighbour.State = HexState.Open;
    //                neighbour.Parent = bestHex;
    //                neighbour.G = bestHex.G + _costToMove;
                    
    //            }
    //        }
    //        KeepLooking(isHorizontal);

    //    }

    //}
}
