using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TFSChangedfiles
{
    public class TaskInfo
    {
        public string ID { get; set; }
        public string Type { get; set; }
        public List<ChangeSet> changeSet = new List<ChangeSet>();

    }
}
