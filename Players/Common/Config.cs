using System.Collections.Generic;

namespace Players.Common
{
    public class Setting
    {
        public string key;
        public string value;
    }
    public class Config
    {
        public string name;
        public string type;
        public List<Setting> settings;
    }
}
