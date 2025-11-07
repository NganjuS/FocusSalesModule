using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FocusSalesModule.Models
{
    public class PaymentDTO
    {
        public decimal CashAmount { get; set; }
        public List<BankPaymentDTO> BankPayments { get; set; }
        public List<DiscountVoucher> DiscountVouchers { get; set; }
        public List<GenericPayment> MoniePayments { get; set; }
        public List<GenericPayment> CreditNote { get; set; }
    }
}