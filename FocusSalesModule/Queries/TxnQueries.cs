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
            return "Select TOP 1 h.sVoucherNo FROM tCore_Header_0 h with(readuncommitted) where h.iVoucherType = 3334   Order by LEN(h.sVoucherNo) desc,h.sVoucherNo  desc";
        }
        public static string GetDiscVoucher(int txnDate,string voucherCode)
        {
            return $"select dsc.iMasterId MasterId, dsc.sCode Code, dsc.sName DiscName, mdsc.* from mCore_discountmaster dsc join muCore_discountmaster mdsc on mdsc.iMasterId = dsc.iMasterId where dsc.iMasterId <> 0 and dsc.sCode='{voucherCode}' and mdsc.StartDate >= {txnDate} and mdsc.EndDate <= {txnDate}";
        }
        public static string GetOutletPaymentTypesQry(int outletId)
        {
            return $"select ptyp.sName PaymentType,putyp.* from muPos_Outlet_PaymentType_Details pdet join mCore_paymenttype ptyp on ptyp.iMasterId = pdet.TypeOfPayment  join  muCore_paymenttype putyp on putyp.iMasterId = ptyp.iMasterId  where pdet.iMasterId = {outletId} and pdet.TypeOfPayment <> 0 order by putyp.OrderField ";
        }
        public static string GetPOSData(int outletId, int dateFrom, int dateTo)
        {
            return $"select  h.iHeaderId HeaderId,h.sVoucherNo DocNo, dbo.IntToDate(h.iDate) DocDate,cust.iMasterId CustAccId ,cust.sName CustomerName,mem.iMasterId MemberId, mem.sName MemberName,outl.iMasterId OutletId, outl.sName OutletName,cst.iMasterId CostCenterId, cst.sName CostCenterName, abs(sum(stk.fQuantity)) Quantity,  abs(sum(stk.mGross)) GrossAmt,abs(max(h.fNet)) Amount,he.sNarration Narration, usr.sLoginName CreatedBy from tCore_Header_0 h join tCore_HeaderData3334_0 he on he.iHeaderId = h.iHeaderId join tCore_Data_0 d on d.iHeaderId = h.iHeaderId join tCore_Indta_0 stk on stk.iBodyId = d.iBodyId join mCore_Product pr on pr.iMasterId = stk.iProduct  left join mCore_Account cust on cust.iMasterId = d.iBookNo left join tCore_Data_Tags_0 tg on tg.iBodyId = d.iBodyId left join mPos_Outlet outl on outl.iMasterId = d.iInvTag  left join mCore_CostCenter cst on cst.iMasterId = d.iFaTag  left join mSec_Users usr on usr.iUserId = h.iUserId left join  mPos_Member mem on mem.iMasterId = tg.iTag1106  where h.iVoucherType = 3334  and h.idate >= {dateFrom} and h.idate <= '{dateTo}' and outl.iMasterId = {outletId}  group by h.iHeaderId ,h.sVoucherNo , h.iDate,cust.iMasterId  ,cust.sName ,mem.iMasterId , mem.sName ,outl.iMasterId , outl.sName ,cst.iMasterId , cst.sName, usr.sLoginName, he.sNarration";
        }
        public static string GetPOSDataByHeaderId(int headerId)
        {
            return $"select  iif(h.bSuspended <> 0, 4, iif(h.iAuth < 2, 2, iif(h.iAuth=3, 1, 3)) )DocStatus,h.iHeaderId HeaderId,h.sVoucherNo DocNo, dbo.IntToDate(h.iDate) DocDate,cust.iMasterId CustAccId ,cust.sName CustomerName,mem.iMasterId MemberId, mem.sName MemberName,outl.iMasterId OutletId, outl.sName OutletName,cst.iMasterId CostCenterId, cst.sName CostCenterName, abs(sum(stk.fQuantity)) Quantity,  abs(sum(stk.mGross)) GrossAmt,abs(max(h.fNet)) Amount,he.sNarration Narration, usr.sLoginName CashierName from tCore_Header_0 h join tCore_HeaderData3334_0 he on he.iHeaderId = h.iHeaderId join tCore_Data_0 d on d.iHeaderId = h.iHeaderId join tCore_Indta_0 stk on stk.iBodyId = d.iBodyId join mCore_Product pr on pr.iMasterId = stk.iProduct  left join mCore_Account cust on cust.iMasterId = d.iBookNo left join tCore_Data_Tags_0 tg on tg.iBodyId = d.iBodyId left join mPos_Outlet outl on outl.iMasterId = d.iInvTag  left join mCore_CostCenter cst on cst.iMasterId = d.iFaTag  left join mSec_Users usr on usr.iUserId = h.iUserId left join  mPos_Member mem on mem.iMasterId = tg.iTag1106  where h.iVoucherType = 3334  and h.iHeaderId= {headerId} group by h.iHeaderId ,h.sVoucherNo , h.iDate,cust.iMasterId  ,cust.sName ,mem.iMasterId , mem.sName ,outl.iMasterId , outl.sName ,cst.iMasterId , cst.sName, usr.sLoginName, he.sNarration, h.bSuspended, h.iAuth";
        }
        public static string GetPOSLinesByHeaderId(int headerId)
        {
            return $"SELECT d.iBodyId LineId ,pr.iMasterId ItemId,pr.sCode ItemCode,pr.sName ItemName,muts.iMasterId UnitId, muts.sName UnitName,RmaNo=(SELECT STRING_AGG(sRmaNo, ', ') FROM tCore_Rma_0 rm where rm.iBodyId = d.iBodyId), abs(stk.fQuantity) Qty,abs(stk.mRate) Price, abs(stk.mGross) as Gross FROM tCore_Header_0 h JOIN tCore_Data_0 d ON h.iHeaderId = d.iHeaderId  JOIN tCore_Indta_0 stk ON d.iBodyId = stk.iBodyId JOIN mCore_Product pr ON pr.iMasterId = stk.iProduct  left join muCore_Product_Units uts on uts.iMasterId = pr.iMasterId left join mCore_Units muts on muts.iMasterId = uts.iDefaultSalesUnit where  h.iHeaderId= {headerId}  order by d.iSerialNo ";
        }
        public static string GetRmaNoListQry(int lineId)
        {
            return $"select sRmaNo from tCore_Rma_0 where iBodyId = {lineId}";
        }
    }
}