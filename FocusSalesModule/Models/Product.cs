using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FocusSalesModule.Models
{
    public class Product
    {
        public int LineId { get; set; }       
        public int ItemId { get; set; }       
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public int BaseUnitId { get; set; }
        public int PostedRma { get; set; }
        public int UnitId { get; set; }
        public string UnitName { get; set; } = String.Empty;
        public string BaseUnit { get; set; } = String.Empty;
        public string RmaNo { get; set; } = String.Empty; 
        public List<string> RmaNoList { get; set; }
        public decimal TaxRateId { get; set; } = 0;
        public decimal TaxRate { get; set; } = 0;
        public decimal Price { get; set; } = 0;
        public decimal Qty { get; set; } = 1;
        public decimal Gross { get; set; }
        public decimal Stock { get; set; }
        public decimal DiscountPct { get; set; } = 0m;
        public decimal FixedDiscountAmt { get; set; } = 0m;
        public bool IsPriceExcl { get; set; } = false;
        public bool AllowZeroPrice { get; set; }
        public int TxnId { get; set; }
        public int RefId { get; set; }
        public int LinkId { get; set; }
        public bool Base { get; set; }
        public bool Closed { get; set; }
        public string LinkVoucherNo { get; set; }
        public string DocNo { get; set; }
        public int LinkVoucherType { get; set; }
    }
}
