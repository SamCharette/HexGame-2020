using System;
using System.Collections.Generic;

namespace Data
{
    [Serializable]
    public class AppConfig
    {
        public int NumberOfGames { get; set; }
        public int BoardSize { get; set; }
        public List<Config> PlayerConfigs { get; set; }
    }
}
