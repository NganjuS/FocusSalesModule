using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FocusSalesModule.Queries
{
    public class DiscountVoucherQry
    {
        public static string GetDiscountVoucherQuery(string voucherno, int salevtype, int rcptvtype)
        {
            return $"select stk.iProduct ItemId,d.iInvTag Outletid,discv.* from tCore_Header_0 h join tCore_Data_0 d on d.iHeaderId = h.iHeaderId \r\njoin tCore_Indta_0 stk on stk.iBodyId = d.iBodyId\r\nouter apply(\r\nselect isnull(disc.sCode,'') Code, isnull(disc.sName,'') Name,dscmst.StartDate, dscmst.EndDate,dscmst.NoofQuantity, dscmst.DiscountValue  from mCore_discountmaster disc join muCore_discountmaster dscmst on disc.iMasterId = dscmst.iMasterId where  dscmst.Item = stk.iProduct and disc.iMasterId <> 0  and  h.iDate >= dscmst.StartDate and h.iDate <= dscmst.EndDate and dscmst.NoofQuantity > (select count(ReferenceNo) from tCore_Data{rcptvtype}_0 where ReferenceNo = disc.sCode)) discv\r\nwhere  sVoucherNo = '{voucherno}' and h.ivouchertype = {salevtype}  ";
        }
    }
}