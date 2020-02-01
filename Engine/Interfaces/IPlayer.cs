using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Interfaces
{
    public interface IPlayer
    {
        string Name { get; set; }
        int PlayerNumber { get; set; }
    }
}
