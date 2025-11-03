using Focus.Common.DataStructs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FocusSalesModule.Queries
{
    public class ProductQueries
    {
        public static string GetRmaData(string rmano, int outletId)
        {
            return $"select uts.iDefaultSalesUnit UnitId,pr.iMasterId Id,pr.sCode Code,pr.sName Name,Price=(select top 1 iif(fval0=0, fval1, fval0) from mCore_SellingPriceBookDetails where iProductId = pr.iMasterId order by iStartDate desc),RmaNo, Qty Stock  from (select iProduct, RmaNo, sum(Qty)  Qty from(SELECT iProduct, rma.sRmaNo RmaNo, 1 Qty FROM tCore_Header_0 JOIN tCore_Data_0 ON tCore_Header_0.iHeaderId = tCore_Data_0.iHeaderId left join tCore_Rma_0 rma on rma.iBodyId = tCore_Data_0.iBodyId JOIN tCore_Indta_0 ON tCore_Data_0.iBodyId = tCore_Indta_0.iBodyId JOIN mCore_Product ON mCore_Product.iMasterId = tCore_Indta_0.iProduct\r\n\r\n    WHERE rma.sRmaNo = '{rmano}' and bUpdateStocks = 1 AND bSuspended = 0 AND bSuspendUpdateStocks = 0 AND tCore_Data_0.iAuthStatus < 2\r\n        AND iProductType<> 1 AND iProductType<> 5\r\n        AND iStatus<> 5 and fQuantityInBase > 0 and tCore_Data_0.iInvTag = {outletId} union all\r\nSELECT iProduct, rma.sRmaNo RmaNo, -1 ReceiptQty\r\n                    FROM tCore_Header_0\r\n                    JOIN tCore_Data_0 ON tCore_Header_0.iHeaderId = tCore_Data_0.iHeaderId\r\n\r\n                    left join tCore_Rma_0 rma on rma.iBodyId = tCore_Data_0.iBodyId\r\n\r\n                    JOIN tCore_Indta_0 ON tCore_Data_0.iBodyId = tCore_Indta_0.iBodyId\r\n                    JOIN mCore_Product ON mCore_Product.iMasterId = tCore_Indta_0.iProduct\r\n\r\n                    WHERE rma.sRmaNo = '{rmano}' and bUpdateStocks = 1 AND bSuspended = 0 AND bSuspendUpdateStocks = 0 AND tCore_Data_0.iAuthStatus < 2\r\n                       AND iProductType<> 1 AND iProductType<> 5\r\n                     AND iStatus<> 5 and fQuantityInBase< 0 and tCore_Data_0.iInvTag = {outletId}) dt group by iProduct, RmaNo\r\n  ) dt join mcore_product pr on pr.imasterid = dt.iProduct left join muCore_Product_Units uts on uts.iMasterId = pr.iMasterId   order by iProduct";
        }
        public static string GetRmaItemQry(string rmano)
        {
            return $"select p.iMasterId ItemId, p.sCode ItemCode, p.sName ItemName,'{rmano}' RmaNo,isnull(u.sName,'') Unit,0 as AvailableStock,SellingRate= (select top 1 fVal0  from mCore_SellingPriceBookDetails where bMarkDeleted = 0 and iProductId =  p.iMasterId order by iStartDate desc)  from mCore_Product p left join muCore_Product_Units uts on uts.iMasterId = p.iMasterId left join mCore_Units u on u.iMasterId = uts.iDefaultSalesUnit where p.imasterid = 5 ";
        }
    }
}
