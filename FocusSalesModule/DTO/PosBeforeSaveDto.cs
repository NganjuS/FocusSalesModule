using FocusSalesModule.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FocusSalesModule.DTO
{
    public class PosBeforeSaveDto
    {
       public int LoginId {  get; set; }
       public int Vtype { get; set; }
        public int CompId { get; set; }
        public string SessionId { get; set; }
        public string DocNo { get; set; }
        public int DocDate { get; set; }
        public int OutletId { get; set; }
        public int CostCenterId { get; set; }
        public int CustomerId { get; set; }
        public int MemberId { get; set; }
        public decimal Amount { get; set; }
        public List<BillSettlement> BillSettlement { get; set; }


    }
}