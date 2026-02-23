using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FocusSalesModule.DTO
{
    public class AdvanceReceiptDelLine
    {
        public int CompanyId   { get; set; }
        public string SessionId { get; set; }
        public int Vtype { get; set; }
        public string DocNo { get; set; }
        public int PaymentType { get; set; }
        public int Account { get; set; }
        public decimal Amount { get; set; }
        public string ReferenceNo { get; set; }
    }
}