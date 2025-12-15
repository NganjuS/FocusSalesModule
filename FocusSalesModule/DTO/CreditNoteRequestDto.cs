using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FocusSalesModule.DTO
{
    public class CreditNoteRequestDto
    {
        public int TxnDate { get; set; }
        public List<string> RmaNoList { get; set; }
    }
}