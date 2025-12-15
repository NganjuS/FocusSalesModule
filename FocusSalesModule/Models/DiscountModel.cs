using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FocusSalesModule.Models
{
    public class DiscountModel
    {
        public int Outletid { get; set; }
        public int ItemId { get; set; } = 0;
        public string Code { get; set; } =  String.Empty;
        public decimal DiscountValue { get; set; } = 0;
    }
}