using System;
using System.Collections.Generic;
using System.Text;
using Players;

namespace Engine
{
    [Serializable]
    public class AppConfig
    {
        public int NumberOfGames { get; set; }
        public int BoardSize { get; set; }
        public List<Config> PlayerConfigs { get; set; }
    }
}
