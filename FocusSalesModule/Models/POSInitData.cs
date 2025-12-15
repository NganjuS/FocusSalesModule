using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FocusSalesModule.Models
{
    public class POSInitData
    {
        public List<dynamic> PaymentTypes { get; set; }
        public List<DiscountModel> DiscountVouchers { get; set; }
    }
}