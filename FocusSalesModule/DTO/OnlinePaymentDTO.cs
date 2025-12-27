using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FocusSalesModule.DTO
{
    public class OnlinePaymentDTO
    {
        public string  TransactionReference { get; set; }
        public string RetrievalReferenceNumber { get; set; }
        public string TerminalSerial { get; set; }
        public DateTime TransactionTime { get; set; }
        public decimal Amount { get; set; }
    }
}