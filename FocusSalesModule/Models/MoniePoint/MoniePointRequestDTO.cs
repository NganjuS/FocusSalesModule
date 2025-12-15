using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FocusSalesModule.Models.MoniePoint
{
    public class MoniePointRequestDTO
    {         
        public EventData data { get; set; }
        public string eventId { get; set; } 
        public Subject subject { get; set; }
        public string createdAt { get; set; }
        public string eventType { get; set; }
    }
    public class EventData
    {
        public decimal amount { get; set; }
        public int businessId { get; set; }
        public CustomFields customFields { get; set; }
        public string responseCode { get; set; }
        public string terminalSerial { get; set; }
        public int businessOwnerId { get; set; }
        public string responseMessage { get; set; }
        public string transactionTime { get; set; }
        public string transactionType { get; set; }
        public string merchantReference { get; set; }
        public string transactionStatus { get; set; }
        public string transactionReference { get; set; }
        public string retrievalReferenceNumber { get; set; }

    }
    public class CustomFields
    {
        public string InvoiceID { get; set; }
    }
    public class Subject
    {
        public string domain { get; set; }
        public string resource { get; set; }
        public string resourceId { get; set; }
    }
}