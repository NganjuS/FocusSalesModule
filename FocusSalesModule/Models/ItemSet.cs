using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FocusSalesModule.Models
{
    public class ItemSet
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Status { get; set; } = false;
    }
}