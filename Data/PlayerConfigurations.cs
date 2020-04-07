using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Data
{
    [JsonArray]
    public class PlayerConfigurations
    {
        public List<Config> Configurations;

        public PlayerConfigurations()
        {
        }
    }
}
