using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TFSChangedfiles
{
    public class ChangeSet
    {
        public int ID { get; set; }
        public List<string> fileNames = new List<string>();
        public string type { get; set; }
        public DateTime checkIn { get; set; }
        public string user { get; set; }
    }
}
