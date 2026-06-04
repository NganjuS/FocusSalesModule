using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FocusSalesModule.DTO
{
    public class PaymentLine
    {
        public int PaymentType { get; set; }
        public string Account { get; set; }
        public decimal Amount { get; set; }
        public string ReferenceNo { get; set; }

    }
    public class ItemLine
    {
        public int Item { get; set; }
        public decimal Quantity { get; set; }
        public decimal Rate { get; set; }
        public decimal Gross { get; set; }
        public List<string> RMA { get; set; }
        public string SchemeItem { get; set; }
        public int LinkedItem { get; set; }
        public string SchemeDocNo { get; set; }

    }
    public class SchemeItemLine
    {
        public int ItemId { get; set; }
        public string Item { get; set; }

        public string SchemeItem { get; set; }
        public int LinkedItem { get; set; }
        public string SchemeDocNo { get; set; }

    }
    public class TxnVoucherData
    {
        public int CompanyId { get; set; }
        public string SessionId { get; set; }
        public int Vtype { get; set; }
        public string DocNo { get; set; }
        public string POSDocNo { get; set; }
        public string DocumentTagId { get; set; } = String.Empty;
        public List<string> ReferenceList { get; set; } 
        public List<PaymentLine> DocLines { get; set; } 
        public List<ItemLine> DocItemLines { get; set; } 
    }
}