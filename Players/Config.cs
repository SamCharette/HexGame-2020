using System.Collections.Generic;

namespace Players
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
        public string playerNumber;
        public List<Setting> settings;
        public string talkative;
    }
}
