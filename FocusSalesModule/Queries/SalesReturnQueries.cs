using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FocusSalesModule.Queries
{
    public class SalesReturnQueries
    {
        public static string CheckIfSalesReurnUsed(string docno)
        {
            return $"select count(dt.iBodyId) from tCore_Data4100_0 dt join tCore_Data_0 d on d.iBodyId = dt.iBodyId left join  tCore_Data_Tags_0 tg on tg.iBodyId = d.iBodyId left join mCore_paymenttype ptyp on ptyp.iMasterId = tg.itag3012 left join muCore_paymenttype putyp on putyp.iMasterId = ptyp.iMasterId  where dt.ReferenceNo = '{docno}'  and putyp.TypeSelect = 5";
        }
    }
}