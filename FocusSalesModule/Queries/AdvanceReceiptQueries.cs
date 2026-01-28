using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FocusSalesModule.Queries
{
    public class AdvanceReceiptQueries
    {
        public static string UpdateOnlinePaymentsStatus(int vtype, string docno, int status)
        {
            return $"update  fpl_OnlinePayments set IsAllocatedToSale = {status} where TransactionReference in (select dta.ReferenceNo from tCore_Header_0 h join tCore_Data_0 d on d.iHeaderId = h.iHeaderId join tCore_Data_Tags_0 tg on tg.iBodyId = d.iBodyId  left join mCore_paymenttype py on py.iMasterId = tg.itag3012 left join tCore_Data{vtype}_0 dta on dta.iBodyId = d.iBodyId left join muCore_paymenttype pye on pye.iMasterId = py.iMasterId where h.iVoucherType = {vtype} and h.svoucherno = '{docno}' and pye.IntegrationType = 1 and dta.ReferenceNo is not null) ";
        }
    }
}