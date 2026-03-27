using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FocusSalesModule.Config
{
    public class AppDefaults
    {
        public const int AdvanceReceiptVType = 4867;
        public enum PaymentGateway
        {
            Moniepoint = 1,
            EasyBuy = 2
        }
        public enum PaymentTypes { Cash = 1, Bank = 2, Integration = 3, DiscountVoucher = 4, CreditNote = 5, AdvanceReceipt = 6 }
        public enum IntegrationTypes
        {
            None = 0, Moniepoint = 1, Easybuy = 2, Sentinal = 3
        }
        
        public enum FieldTypes
        {
            Discount = 1
        }
    }
}