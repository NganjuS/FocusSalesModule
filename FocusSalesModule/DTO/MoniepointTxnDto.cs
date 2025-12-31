using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FocusSalesModule.DTO
{
    public class MoniepointTxnDto
    {
        
            public int Id { get; set; }
            public int OutletId { get; set; }
                
            public string WebhookId { get; set; }
            public string OutletName { get; set; }
            public long WebhookTimestamp { get; set; }
            public decimal Amount { get; set; }
            public string BusinessId { get; set; }
            public string ResponseCode { get; set; }
            public string TerminalSerial { get; set; }
            public string BusinessOwnerId { get; set; }
            public string ResponseMessage { get; set; }
            public string TransactionTime { get; set; }
            public string TransactionType { get; set; }
            public string MerchantReference { get; set; }
            public string TransactionStatus { get; set; }
            public string TransactionReference { get; set; }
            public string RetrievalReferenceNumber { get; set; }
            public string InvoiceID { get; set; }
            public string EventId { get; set; }
            public string Domain { get; set; }
            public string ReqResource { get; set; }
            public int ResourceId { get; set; }
            public string CreatedAt { get; set; }
            public string EventType { get; set; }
            public bool IsAllocatedToSale { get; set; }
            public decimal AmountInLocalCur { get; set; }
            public bool? IsValidTxn { get; set; }
        
    }
}