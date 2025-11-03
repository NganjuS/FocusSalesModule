using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FocusSalesModule.Queries
{
    public class TxnQueries
    {
        public static string GetPOSNextDocNo()
        {
            return "Select TOP 1 h.sVoucherNo FROM tCore_Header_0 h with(readuncommitted) where h.iVoucherType = 3335   Order by LEN(h.sVoucherNo) desc,h.sVoucherNo  desc";
        }
    }
}