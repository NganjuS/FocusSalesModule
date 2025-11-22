using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FocusSalesModule.Models
{
    public class AdvancedPaymentDTO
    {
        public int OutletId { get; set; }
        public int MemberId { get; set; }
        public DateTime DocDate { get; set; }
        public decimal Amount { get; set; }
        public string ReferenceNo { get; set; }
    }
}