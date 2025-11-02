using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FocusSalesModule.Models
{
    public class RmaItem
    {
        public int ItemId { get; set; } = 0;
        public string ItemCode { get; set; } = string.Empty;
        public string ItemName { get; set; } = string.Empty;
        public string RmaNo { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public decimal Qty { get; set; } = 1;
        public decimal AvailableStock { get; set; } = 0m;
        public decimal SellingRate { get; set; } = 0m;
        public decimal DiscountPct { get; set; } = 0m;
        public decimal FixedDiscountAmt { get; set; } = 0m;
    }
}
