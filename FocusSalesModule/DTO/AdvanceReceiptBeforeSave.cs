using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FocusSalesModule.DTO
{
    public class AdvanceReceiptBeforeSave
    {
        public int CompanyId { get; set; }
        public string SessionId { get; set; }
        public int Vtype { get; set; }
        public string DocNo { get; set; }
        public string DocumentTagId { get; set; } = String.Empty;
    }
}