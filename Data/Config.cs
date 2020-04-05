using System.Collections.Generic;
using LiteDB;

namespace Data
{
    public class Config
    {
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string PlayerNumber { get; set; }
        public List<Setting> Settings { get; set; }
        public string Talkative { get; set; }

        public Config()
        {
            Id = ObjectId.NewObjectId();
        }
    }
}
