using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FocusSalesModule.Models
{
    public class HashData<T>
    {
        public int result { get; set; }
        public string message { get; set; }
        public string url { get; set; }
        public T data { get; set; }
        public List<T> datalist { get; set; }


    }
}
