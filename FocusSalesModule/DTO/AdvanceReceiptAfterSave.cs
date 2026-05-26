using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FocusSalesModule.DTO
{
    public class AdvanceReceiptAfterSave
    {
      public  TxnVoucherData AdvanceReceipt {  get; set; }
      public List<string> References { get; set; }
    }
}