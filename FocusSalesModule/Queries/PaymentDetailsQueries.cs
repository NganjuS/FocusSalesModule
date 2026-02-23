using FocusSalesModule.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FocusSalesModule.Queries
{
    public class PaymentDetailsQueries
    {
        public static string GetPaymentTypeQry(int paymentTypeId)
        {
            return $"select p.iMasterId Id, p.sCode  Code, p.sName Name,pty.TypeSelect, pty.IntegrationType from mCore_paymenttype p join muCore_paymenttype pty on pty.iMasterId = p.iMasterId where p.iMasterId <> 0 and p.iMasterId  = {paymentTypeId}";
        }
        public static string GetCreditNotes(int memberId, string rmaList)
        {
            return $"select 0 IsSelected, Reference, Amount-PostedAmt Amount, PostedAmt,Amount FullAmt   from (select *, PostedAmt = isnull((select abs(sum(mAmount2)) from tCore_Data4100_0 dt join tCore_Data_0 d on d.iBodyId = dt.iBodyId  where dt.ReferenceNo = srtn.Reference),0) from (SELECT h.iHeaderId,h.idate, h.sVoucherNo Reference,abs(sum(ind.mGross))- sum(sd.mVal0)-sum(sd.mVal1)- max(ft.mVal0) Amount FROM tCore_Header_0 h JOIN tCore_Data_0 d ON h.iHeaderId = d.iHeaderId  JOIN tCore_Indta_0 ind  ON d.iBodyId = ind.iBodyId join mcore_product pr on pr.imasterid = ind.iProduct left join muCore_Product_Units uts on uts.iMasterId = pr.iMasterId left join mCore_Units muts on muts.iMasterId = uts.iDefaultSalesUnit left join tCore_Data_Tags_0 tg on tg.iBodyId = d.iBodyId left join  tCore_IndtaBodyScreenData_0 sd on sd.iBodyId = d.iBodyId left join mPos_Member mbr on mbr.iMasterId = tg.iTag1106  left join tCore_IndtaFooterScreenData_0 ft on ft.iHeaderId = h.iHeaderId\r\nWHERE   mbr.iMasterId = {memberId} and h.iVoucherType = 1793  group by h.iHeaderId, h.idate, h.sVoucherNo\r\n\r\n) srtn ) fndt where (Amount-PostedAmt ) > 0 order by idate desc";
        }
        public static string GetAdvanceReceipts(int memberId)
        {
            return $"select 0 IsSelected, Reference, Amount-PostedAmt Amount, PostedAmt,Amount FullAmt   from (select *, PostedAmt = isnull((select abs(sum(mAmount2)) from tCore_Data4100_0 dt join tCore_Data_0 d on d.iBodyId = dt.iBodyId  where dt.ReferenceNo = srtn.Reference),0) from (SELECT h.iHeaderId,h.idate, h.sVoucherNo Reference,abs(sum(d.mAmount2)) Amount FROM tCore_Header_0 h JOIN tCore_Data_0 d ON h.iHeaderId = d.iHeaderId  left join  tCore_IndtaBodyScreenData_0 sd on sd.iBodyId = d.iBodyId left join tCore_Data_Tags_0 tg on tg.iBodyId = d.iBodyId left join mPos_Member mbr on mbr.iMasterId = tg.iTag1106  left join tCore_IndtaFooterScreenData_0 ft on ft.iHeaderId = h.iHeaderId WHERE mbr.iMasterId = {memberId} and h.iVoucherType = 4611  group by h.iHeaderId, h.idate, h.sVoucherNo) srtn ) fndt where (Amount-PostedAmt ) > 0 order by idate desc";
        }
        public static string GetCreditNotesOld(int memberId, string rmaList)
        {
            return $"select 0 IsSelected,Reference, Amount-ft.mVal0 Amount from (SELECT h.iHeaderId,concat(h.sVoucherNo, '|',rma.sRmaNo) Reference, abs(sum(ind.mGross))- sum(sd.mVal0)-sum(sd.mVal1)  Amount FROM tCore_Header_0 h JOIN tCore_Data_0 d ON h.iHeaderId = d.iHeaderId left join tCore_Rma_0 rma on rma.iBodyId = d.iBodyId JOIN tCore_Indta_0 ind  ON d.iBodyId = ind.iBodyId join mcore_product pr on pr.imasterid = ind.iProduct left join muCore_Product_Units uts on uts.iMasterId = pr.iMasterId left join mCore_Units muts on muts.iMasterId = uts.iDefaultSalesUnit left join tCore_Data_Tags_0 tg on tg.iBodyId = d.iBodyId left join  tCore_IndtaBodyScreenData_0 sd on sd.iBodyId = d.iBodyId left join mPos_Member mbr on mbr.iMasterId = tg.iTag1106 WHERE rma.sRmaNo in ({rmaList}) and mbr.iMasterId = {memberId} and h.iVoucherType = 1793  group by h.iHeaderId,h.sVoucherNo, rma.sRmaNo )  dt left join tCore_IndtaFooterScreenData_0 ft on ft.iHeaderId = dt.iHeaderId  where not exists  (select iBodyId from tCore_Data4100_0 where ReferenceNo = Reference)";
        }
        public static string GetDiscountCodes(string itemList,int txndate)
        {
            return $"select isnull(disc.sCode,'') Code, isnull(disc.sName,'') Name,dscmst.StartDate, dscmst.EndDate,dscmst.NoofQuantity, dscmst.DiscountValue,dscmst.Item ItemId  ,dscmst.DiscountAccount SelectedAccount,dscmst.DiscountValue FullAmt,0 PostedAmt from mCore_discountmaster disc join muCore_discountmaster dscmst on disc.iMasterId = dscmst.iMasterId where  dscmst.Item in ({itemList}) and disc.iMasterId <> 0   and dscmst.NoofQuantity > (select count(ReferenceNo) from tCore_Data4100_0 where ReferenceNo = disc.sCode) and   dscmst.StartDate <= {txndate} and dscmst.EndDate >= {txndate}";
        }
        public static string GetDetails(int outletid)
        {
            return $"select top 10 mp.TransactionReference,mp.RetrievalReferenceNumber, mp.TerminalSerial, mp.TransactionTime,mp.Amount  from fpl_OnlinePayments mp where TerminalSerial in (select  trm.sCode from mCore_terminalmachines trm join muCore_terminalmachines trm1 on trm.iMasterId = trm1.iMasterId where trm1.Outlet = {outletid}) and TransactionStatus = 'SUCCESSFUL' and TransactionType = 'PURCHASE' order by mp.transactiontime desc ";
        }
        public static string GetManualSearchPayment(int outletid, int paymentType,string reference)
        {
            string outletStr = paymentType == (Int32)AppDefaults.PaymentGateway.Moniepoint ? $" TerminalSerial in (select  trm.sCode from mCore_terminalmachines trm join muCore_terminalmachines trm1 on trm.iMasterId = trm1.iMasterId where trm1.Outlet = {outletid})  " : " 1=1 ";

            return $"select top 1 mp.TransactionReference,mp.RetrievalReferenceNumber, mp.TerminalSerial, mp.TransactionTime,mp.AmountInLocalCur Amount from fpl_OnlinePayments mp where {outletStr} and  ((TransactionType = 'PURCHASE' and TransactionStatus = 'SUCCESSFUL' ) or (TransactionType ='POS_TRANSFER' and  TransactionStatus = 'APPROVED') )  and  isnull(mp.IsAllocatedToSale,0) = 0 and mp.InternalTxnCode = '{reference}' and  PaymentType = {paymentType} order by mp.transactiontime desc";
        }
        public static string GetOutstandingAmtDetails(int outletid, bool manualvalidate, string reference,decimal outstandingamt)
        {
            if (manualvalidate)
            {
                return $"select top 1 mp.TransactionReference,mp.RetrievalReferenceNumber, mp.TerminalSerial, mp.TransactionTime,mp.AmountInLocalCur Amount from fpl_OnlinePayments mp where TerminalSerial in (select  trm.sCode from mCore_terminalmachines trm join muCore_terminalmachines trm1 on trm.iMasterId = trm1.iMasterId where trm1.Outlet = {outletid}) and  ((TransactionType = 'PURCHASE' and TransactionStatus = 'SUCCESSFUL' ) or (TransactionType ='POS_TRANSFER' and  TransactionStatus = 'APPROVED') )  and  isnull(mp.IsAllocatedToSale,0) = 0 and mp.TransactionReference = '{reference}' order by mp.transactiontime desc ";
            }
            else
            {
                return $"select top 1 mp.TransactionReference,mp.RetrievalReferenceNumber, mp.TerminalSerial, mp.TransactionTime,mp.AmountInLocalCur Amount  from fpl_OnlinePayments mp where TerminalSerial in (select  trm.sCode from mCore_terminalmachines trm join muCore_terminalmachines trm1 on trm.iMasterId = trm1.iMasterId where trm1.Outlet = {outletid})  and ((TransactionType = 'PURCHASE' and TransactionStatus = 'SUCCESSFUL' ) or (TransactionType ='POS_TRANSFER' and  TransactionStatus = 'APPROVED') )  and mp.AmountInLocalCur ={outstandingamt} and isnull(mp.IsAllocatedToSale,0) = 0 order by mp.transactiontime desc ";
            }
               
        }
        public static string GetOutstandingPaymentList(int outletid)
        {
            return $"select mp.TransactionReference,mp.RetrievalReferenceNumber, mp.TerminalSerial, mp.TransactionTime,mp.AmountInLocalCur Amount from fpl_OnlinePayments mp where TerminalSerial in (select  trm.sCode from mCore_terminalmachines trm join muCore_terminalmachines trm1 on trm.iMasterId = trm1.iMasterId where trm1.Outlet = {outletid}) and  ((TransactionType = 'PURCHASE' and TransactionStatus = 'SUCCESSFUL' ) or (TransactionType ='POS_TRANSFER' and  TransactionStatus = 'APPROVED') )  and  isnull(mp.IsAllocatedToSale,0) = 0  order by mp.transactiontime desc ";
        }
        public static string GetPaymentListFullPagedCount( string searchfilter)
        {
            return $"select count(mp.TransactionReference) id from fpl_OnlinePayments mp left join mCore_terminalmachines trm on trm.sCode = mp.TerminalSerial left join muCore_terminalmachines trmu on trmu.iMasterId= trm.iMasterId left join mPos_Outlet outl on outl.iMasterId = trmu.Outlet where {searchfilter}   ";
        }
        public static string GetPaymentListFullPaged( string searchfilter)
        {
            return $"select isnull(outl.imasterid,0) OutletId,isnull(outl.sname,'') OutletName, mp.* from fpl_OnlinePayments mp left join mCore_terminalmachines trm on trm.sCode = mp.TerminalSerial left join muCore_terminalmachines trmu on trmu.iMasterId= trm.iMasterId left join mPos_Outlet outl on outl.iMasterId = trmu.Outlet where  {searchfilter}  ";
        }
        public static string GetOutstandingPaymentListPagedByOutletCount(int paymentType,int outletid, string searchfilter)
        {
            string outletStr = paymentType == (Int32)AppDefaults.PaymentGateway.Moniepoint ? $" TerminalSerial in (select  trm.sCode from mCore_terminalmachines trm join muCore_terminalmachines trm1 on trm.iMasterId = trm1.iMasterId where trm1.Outlet = {outletid})   " : " 1=1 ";

            return $"select count(mp.TransactionReference) id from fpl_OnlinePayments mp  where {outletStr} and not exists( select top 1 ReferenceNo from tCore_Data4100_0 dt  where dt.ReferenceNo = mp.TransactionReference) and  ((TransactionType = 'PURCHASE' and TransactionStatus = 'SUCCESSFUL' ) or (TransactionType ='POS_TRANSFER' and  TransactionStatus = 'APPROVED') )  and  isnull(mp.IsAllocatedToSale,0) = 0 {searchfilter}   ";
        }
        public static string GetOutstandingPaymentListPaged(int paymentType, int outletid,  string searchfilter)
        {
            string outletStr = paymentType == (Int32)AppDefaults.PaymentGateway.Moniepoint ? $" TerminalSerial in (select  trm.sCode from mCore_terminalmachines trm join muCore_terminalmachines trm1 on trm.iMasterId = trm1.iMasterId where trm1.Outlet = {outletid})  " : " 1=1 ";

            return $"select mp.TransactionReference,mp.RetrievalReferenceNumber, mp.TerminalSerial, mp.TransactionTime,mp.AmountInLocalCur Amount from fpl_OnlinePayments mp where {outletStr} and not exists( select top 1 ReferenceNo from tCore_Data4100_0 dt  where dt.ReferenceNo = mp.TransactionReference) and  ((TransactionType = 'PURCHASE' and TransactionStatus = 'SUCCESSFUL' ) or (TransactionType ='POS_TRANSFER' and  TransactionStatus = 'APPROVED') )  and  isnull(mp.IsAllocatedToSale,0) = 0 {searchfilter}  ";
        }
    }
}