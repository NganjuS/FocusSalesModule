using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FocusSalesModule.Models
{
    public class POSDto
    {
        public List<Product> Items { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Tax { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal Change { get; set; }
        public DateTime Timestamp { get; set; }
        public PaymentDTO Payments { get; set; }
    }
}