using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FocusSalesModule.Models
{
    public class ItemSetLine
    {
        public bool Status { get; set; }
        public int ItemId { get; set; }
        public string SetItem { get; set; }
        public string DocNo { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string RmaNo { get; set; }
        public decimal Qty { get; set; }
        public decimal Rate { get; set; }
        public decimal Gross { get; set; }
        public decimal VAT { get; set; }
        public decimal MinimumAdvance { get; set; }
        public decimal Discount { get; set; } = 0;
        public decimal TotalQty { get; set; } = 0;
    }
}