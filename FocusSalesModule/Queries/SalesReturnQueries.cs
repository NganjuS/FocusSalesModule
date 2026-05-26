using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FocusSalesModule.Queries
{
    public class SalesReturnQueries
    {
        public static string CheckIfSalesReturnUsed(string docno)
        {
            return $"select count(dt.iBodyId) from tCore_Data4100_0 dt join tCore_Data_0 d on d.iBodyId = dt.iBodyId left join  tCore_Data_Tags_0 tg on tg.iBodyId = d.iBodyId left join mCore_paymenttype ptyp on ptyp.iMasterId = tg.itag3012 left join muCore_paymenttype putyp on putyp.iMasterId = ptyp.iMasterId  where dt.ReferenceNo = '{docno}'  and putyp.TypeSelect = 5";
        }
        public static string GetSalesReturnStatusQry(string referenceno)
        {
            return $"select 0 IsSelected, Reference, Amount-PostedAmt Amount, PostedAmt,Amount FullAmt   from (select *, PostedAmt = isnull((select abs(sum(mAmount2)) from tCore_Data4100_0 dt join tCore_Data_0 d on d.iBodyId = dt.iBodyId  where dt.ReferenceNo = srtn.Reference),0) from (SELECT h.iHeaderId,h.idate, h.sVoucherNo Reference,abs(sum(ind.mGross))- sum(sd.mVal0)-sum(sd.mVal1)- max(ft.mVal0) Amount FROM tCore_Header_0 h JOIN tCore_Data_0 d ON h.iHeaderId = d.iHeaderId  JOIN tCore_Indta_0 ind  ON d.iBodyId = ind.iBodyId join mcore_product pr on pr.imasterid = ind.iProduct left join muCore_Product_Units uts on uts.iMasterId = pr.iMasterId left join mCore_Units muts on muts.iMasterId = uts.iDefaultSalesUnit left join tCore_Data_Tags_0 tg on tg.iBodyId = d.iBodyId left join  tCore_IndtaBodyScreenData_0 sd on sd.iBodyId = d.iBodyId left join mPos_Member mbr on mbr.iMasterId = tg.iTag1106  left join tCore_IndtaFooterScreenData_0 ft on ft.iHeaderId = h.iHeaderId\r\nWHERE   h.iVoucherType = 1793 and h.bSuspended = 0 and h.iAuth = 1 and h.svoucherno = '{referenceno}' group by h.iHeaderId, h.idate, h.sVoucherNo\r\n\r\n) srtn ) fndt  ";
        }
    }
}