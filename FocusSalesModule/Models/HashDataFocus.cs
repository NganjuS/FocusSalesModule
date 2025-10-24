using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FocusSalesModule.Models
{
    public class HashDataFocus
    {
        public int result { get; set; }
        public string message { get; set; }
        public string url { get; set; }
        public List<Hashtable> data { get; set; }
    }
}
