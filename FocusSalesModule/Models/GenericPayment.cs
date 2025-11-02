using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FocusSalesModule.Models
{
    public class GenericPayment
    {
        public string Code { get; set; }
        public string Reference { get; set; }
        public decimal Amount { get; set; }
    }
}