using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FocusSalesModule.DTO
{
    public class TemporaryPaymentDataDto
    {
        public int Id { get; set; }
        public string DocumentTagId { get; set; }
        public string ProbableDocNo { get; set; }
        public int Vtype { get; set; }
        public int CustomerId { get; set; }
        public int MemberId { get; set; }
        public int OutletId { get; set; }
        public int CostCenterId { get; set; }
        public int DocDate { get; set; }
        public int LoginId { get; set; }
        public bool IsValidated { get; set; }
        public decimal Amount { get; set; }
        public string Reference { get; set; }
        public int PaymentMethodId { get; set; }
        public int PaymentType { get; set; }
        public bool ShowReference { get; set; }
        public bool ShowBank { get; set; }
        public int SelectedAccount { get; set; }
        public DateTime TxnDate { get; set; }
        public DateTime CreatedOn { get; set; }

    }
}