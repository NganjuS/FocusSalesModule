using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FocusSalesModule.Models
{
    public class Product
    {
        public int Id { get; set; }
        public int OutletId { get; set; } = 0;
        public int SalesAccId { get; set; } = 0;
        public int CustAccId { get; set; } = 0;
        public int MemberId { get; set; } = 0;
        public int CostCenterId { get; set; } = 0;
        public string Code { get; set; }
        public string Name { get; set; }
        public int BaseUnitId { get; set; }
        public int UnitId { get; set; }
        public string BaseUnit { get; set; }
        public string RmaNo { get; set; }
        public string Narration { get; set; }
        public List<string> RmaNoList { get; set; }
        public decimal TaxRateId { get; set; } = 0;
        public decimal TaxRate { get; set; } = 0;
        public decimal Price { get; set; } = 0;
        public string Unit { get; set; } = string.Empty;
        public decimal Qty { get; set; } = 1;
        public decimal Gross { get; set; }
        public decimal Stock { get; set; }
        public decimal DiscountPct { get; set; } = 0m;
        public decimal FixedDiscountAmt { get; set; } = 0m;
        public bool IsPriceExcl { get; set; } = false;
        public bool AllowZeroPrice { get; set; }
    }
}
