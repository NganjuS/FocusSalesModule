using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FocusSalesModule.Models
{
    enum docStates { New = 0 , Unapproved  = 1, Approved =2, Rejected = 3, Suspended = 4 }
    public class POSDTO
    {

        public int HeaderId { get; set; }
        public string DocNo { get; set; }
        public int DocStatus { get; set; } = 0; //0 - New, 1 - Unapproved, 2 - Approved, 3 Rejected, 4- Suspended
        public string DocStatusDesc
        {
            get
            {
                return Enum.GetName(typeof(docStates), DocStatus);
            }
        }
        public DateTime DocDate { get; set; }
        public int OutletId { get; set; } = 0;
        public string OutletName { get; set; }
        public string OutletAddress { get; set; }
        public int SalesAccId { get; set; } = 0;
        public int CustAccId { get; set; } = 0;
        public string CustomerName { get; set; }
        public int MemberId { get; set; } = 0;
        public string MemberName { get; set; }
        public int CostCenterId { get; set; } = 0;
        public string CostCenterName { get; set; } 
        public string Narration { get; set; }     
        public string CreatedCashierName { get; set; }     
             
        public decimal SubTotal { get; set; }
        public decimal Tax { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal Change { get; set; }
        public decimal Amount { get; set; }
        public DateTime Timestamp { get; set; }
        public List<dynamic> Payments { get; set; }
        public List<Product> Items { get; set; }
    }
}