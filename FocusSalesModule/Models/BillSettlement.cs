using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FocusSalesModule.Models
{
    public class BillSettlement
    {
        public decimal Amount { get; set; } 
        public string Reference { get; set; }
        public string PaymentType { get; set; }
        public int iMasterId { get; set; }
        public bool ShowReference { get; set; }
        public bool ShowBank { get; set; }
        public int DefaultAccount { get; set; }
        public int DefaultCashAccount { get; set; }
        public int DefaultBankAccount { get; set; }
        public int DefaultOnlineAccount { get; set; }
        public int DefaultCreditNoteAccount { get; set; }
        public int DefaultDiscountAccount { get; set; }
        public int TypeSelect { get; set; }
        public List<GenericPayment> PayList { get; set; }
    }
}