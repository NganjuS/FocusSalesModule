using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FocusSalesModule.DTO
{
    public class OnlinePaymentDTO
    {
        public int  PaymentType { get; set; }
        public int  AccountId { get; set; }
        public string  TransactionReference { get; set; }
        public string RetrievalReferenceNumber { get; set; }
        public string TerminalSerial { get; set; }
        public DateTime TransactionTime { get; set; }
        public string InternalTxnCode { get; set; }
        public string Narration { get; set; }
        
        public decimal Amount { get; set; }
        public bool IsAllocatedToSale { get; set; }
        public string EntityName { get; set; }
    }
}