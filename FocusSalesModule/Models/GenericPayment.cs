using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FocusSalesModule.Models
{
    public class GenericPayment
    {
        public string Code { get; set; }
        public int AccountId { get; set; }
        public string Reference { get; set; }
        public decimal Amount { get; set; }
        public decimal PostedAmt { get; set; }
        public decimal FullAmt { get; set; }
        public bool IsSelected { get; set; }
    }
}