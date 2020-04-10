using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Data
{

    public class GamePlayer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string PlayerNumber { get; set; }
        public List<Setting> Settings { get; set; }
        public string Talkative { get; set; }
 
        public string GeneralInfo { get; set; }

     
    }
}
