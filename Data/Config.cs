using System.Collections.Generic;
using Newtonsoft.Json;

namespace Data
{
    public class Config
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string PlayerNumber { get; set; }
        public List<Setting> Settings { get; set; }
        public string Talkative { get; set; }

    }
}
