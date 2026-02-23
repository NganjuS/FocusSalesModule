using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FocusSalesModule.Queries
{
    public class AdvanceReceiptQueries
    {
        public static string CheckIfAdvancedReceiptUsed(string refno)
        {
            return $"select count(dt.iBodyId) from tCore_Data4100_0 dt join tCore_Data_0 d on d.iBodyId = dt.iBodyId left join  tCore_Data_Tags_0 tg on tg.iBodyId = d.iBodyId left join mCore_paymenttype ptyp on ptyp.iMasterId = tg.itag3012 left join muCore_paymenttype putyp on putyp.iMasterId = ptyp.iMasterId  where dt.ReferenceNo = '{refno}'  and putyp.TypeSelect = 6";
        }
        public static string AdvanceReceiptExistsQry(string docno)
        {
            return $"select count(h.iHeaderId) from tCore_Header_0 h  where h.iVoucherType = 4611 and h.sVoucherNo = '{docno}'";
        }
        public static string GetAdvanceReceiptReferenceNo(int vtype, string docno)
        {
            return $"select dd.ReferenceNo from tCore_Header_0 h join tCore_Data_0 d on d.iHeaderId = h.iHeaderId left join tCore_Data4611_0 dd on dd.iBodyId = d.iBodyId left join  tCore_Data_Tags_0 tg on tg.iBodyId = d.iBodyId left join muCore_paymenttype pyu on pyu.imasterid = py.imasterid where  pyu.TypeSelect = 3 and h.iVoucherType = 4611 and h.sVoucherNo = '{docno}' ";
        }
        public static string UpdateOnlinePaymentsStatus(int vtype, string docno, int status)
        {
            return $"update  fpl_OnlinePayments set IsAllocatedToSale = {status} where TransactionReference in (select dta.ReferenceNo from tCore_Header_0 h join tCore_Data_0 d on d.iHeaderId = h.iHeaderId join tCore_Data_Tags_0 tg on tg.iBodyId = d.iBodyId  left join mCore_paymenttype py on py.iMasterId = tg.itag3012 left join tCore_Data{vtype}_0 dta on dta.iBodyId = d.iBodyId left join muCore_paymenttype pye on pye.iMasterId = py.iMasterId where h.iVoucherType = {vtype} and h.svoucherno = '{docno}' and pye.IntegrationType = 1 and dta.ReferenceNo is not null) ";
        }
        public static string UpdatePaymentsStatus(int status, string txnref)
        {
            return $"update  fpl_OnlinePayments set IsAllocatedToSale = {status} where TransactionReference  = '{txnref}' ";
        }
    }
}