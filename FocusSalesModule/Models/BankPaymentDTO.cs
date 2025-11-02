using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FocusSalesModule.Models
{
    public class BankPaymentDTO
    {
        public int BankAccountId { get; set; }
        public  string Method { get; set; }
        public string Reference { get; set; }
        public decimal Amount { get; set; }
    }
}