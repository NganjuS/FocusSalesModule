using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FocusSalesModule.Queries
{
    public class PaymentDetailsQueries
    {
        public static string GetCreditNotes(int memberId, string rmaList)
        {
            return $"select 0 IsSelected,* from (SELECT concat(h.sVoucherNo, '|',rma.sRmaNo) Reference, abs(sum(ind.mGross))- sum(sd.mVal0)-sum(sd.mVal1)  Amount FROM tCore_Header_0 h JOIN tCore_Data_0 d ON h.iHeaderId = d.iHeaderId left join tCore_Rma_0 rma on rma.iBodyId = d.iBodyId JOIN tCore_Indta_0 ind  ON d.iBodyId = ind.iBodyId join mcore_product pr on pr.imasterid = ind.iProduct left join muCore_Product_Units uts on uts.iMasterId = pr.iMasterId left join mCore_Units muts on muts.iMasterId = uts.iDefaultSalesUnit left join tCore_Data_Tags_0 tg on tg.iBodyId = d.iBodyId left join  tCore_IndtaBodyScreenData_0 sd on sd.iBodyId = d.iBodyId left join mPos_Member mbr on mbr.iMasterId = tg.iTag1106 WHERE rma.sRmaNo in ({rmaList}) and mbr.iMasterId = {memberId} and h.iVoucherType = 1793  group by h.sVoucherNo, rma.sRmaNo ) dt where not exists  (select iBodyId from tCore_Data4100_0 where ReferenceNo = Reference)";
        }
        public static string GetDiscountCodes(string itemList,int txndate)
        {
            return $"select isnull(disc.sCode,'') Code, isnull(disc.sName,'') Name,dscmst.StartDate, dscmst.EndDate,dscmst.NoofQuantity, dscmst.DiscountValue,dscmst.Item ItemId  ,dscmst.DiscountAccount SelectedAccount from mCore_discountmaster disc join muCore_discountmaster dscmst on disc.iMasterId = dscmst.iMasterId where  dscmst.Item in ({itemList}) and disc.iMasterId <> 0   and dscmst.NoofQuantity > (select count(ReferenceNo) from tCore_Data4100_0 where ReferenceNo = disc.sCode) and   dscmst.StartDate <= {txndate} and dscmst.EndDate >= {txndate}";
        }
        public static string GetDetails(int outletid)
        {
            return $"select top 10 mp.TransactionReference,mp.RetrievalReferenceNumber, mp.TerminalSerial, mp.TransactionTime,mp.Amount  from MoniePointData mp where TerminalSerial in (select  trm.sCode from mCore_terminalmachines trm join muCore_terminalmachines trm1 on trm.iMasterId = trm1.iMasterId where trm1.Outlet = {outletid}) and TransactionStatus = 'SUCCESSFUL' and TransactionType = 'PURCHASE' order by mp.transactiontime desc ";
        }
        public static string GetManualSearchPayment(int outletid, string reference)
        {
            return $"select top 1 mp.TransactionReference,mp.RetrievalReferenceNumber, mp.TerminalSerial, mp.TransactionTime,mp.AmountInLocalCur Amount from MoniePointData mp where TerminalSerial in (select  trm.sCode from mCore_terminalmachines trm join muCore_terminalmachines trm1 on trm.iMasterId = trm1.iMasterId where trm1.Outlet = {outletid}) and  ((TransactionType = 'PURCHASE' and TransactionStatus = 'SUCCESSFUL' ) or (TransactionType ='POS_TRANSFER' and  TransactionStatus = 'APPROVED') )  and  isnull(mp.IsAllocatedToSale,0) = 0 and mp.TransactionReference = '{reference}' order by mp.transactiontime desc";
        }
        public static string GetOutstandingAmtDetails(int outletid, bool manualvalidate, string reference,decimal outstandingamt)
        {
            if (manualvalidate)
            {
                return $"select top 1 mp.TransactionReference,mp.RetrievalReferenceNumber, mp.TerminalSerial, mp.TransactionTime,mp.AmountInLocalCur Amount from MoniePointData mp where TerminalSerial in (select  trm.sCode from mCore_terminalmachines trm join muCore_terminalmachines trm1 on trm.iMasterId = trm1.iMasterId where trm1.Outlet = {outletid}) and  ((TransactionType = 'PURCHASE' and TransactionStatus = 'SUCCESSFUL' ) or (TransactionType ='POS_TRANSFER' and  TransactionStatus = 'APPROVED') )  and  isnull(mp.IsAllocatedToSale,0) = 0 and mp.TransactionReference = '{reference}' order by mp.transactiontime desc ";
            }
            else
            {
                return $"select top 1 mp.TransactionReference,mp.RetrievalReferenceNumber, mp.TerminalSerial, mp.TransactionTime,mp.AmountInLocalCur Amount  from MoniePointData mp where TerminalSerial in (select  trm.sCode from mCore_terminalmachines trm join muCore_terminalmachines trm1 on trm.iMasterId = trm1.iMasterId where trm1.Outlet = {outletid})  and ((TransactionType = 'PURCHASE' and TransactionStatus = 'SUCCESSFUL' ) or (TransactionType ='POS_TRANSFER' and  TransactionStatus = 'APPROVED') )  and mp.AmountInLocalCur ={outstandingamt} and isnull(mp.IsAllocatedToSale,0) = 0 order by mp.transactiontime desc ";
            }
               
        }
        public static string GetOutstandingPaymentList(int outletid)
        {
            return $"select mp.TransactionReference,mp.RetrievalReferenceNumber, mp.TerminalSerial, mp.TransactionTime,mp.AmountInLocalCur Amount from MoniePointData mp where TerminalSerial in (select  trm.sCode from mCore_terminalmachines trm join muCore_terminalmachines trm1 on trm.iMasterId = trm1.iMasterId where trm1.Outlet = {outletid}) and  ((TransactionType = 'PURCHASE' and TransactionStatus = 'SUCCESSFUL' ) or (TransactionType ='POS_TRANSFER' and  TransactionStatus = 'APPROVED') )  and  isnull(mp.IsAllocatedToSale,0) = 0  order by mp.transactiontime desc ";
        }
        public static string GetPaymentListFullPagedCount( string searchfilter)
        {
            return $"select count(mp.TransactionReference) id from MoniePointData mp left join mCore_terminalmachines trm on trm.sCode = mp.TerminalSerial left join muCore_terminalmachines trmu on trmu.iMasterId= trm.iMasterId left join mPos_Outlet outl on outl.iMasterId = trmu.Outlet where {searchfilter}   ";
        }
        public static string GetPaymentListFullPaged( string searchfilter)
        {
            return $"select isnull(outl.imasterid,0) OutletId,isnull(outl.sname,'') OutletName, mp.* from MoniePointData mp left join mCore_terminalmachines trm on trm.sCode = mp.TerminalSerial left join muCore_terminalmachines trmu on trmu.iMasterId= trm.iMasterId left join mPos_Outlet outl on outl.iMasterId = trmu.Outlet where  {searchfilter}  ";
        }
        public static string GetOutstandingPaymentListPagedByOutletCount(int outletid, string searchfilter)
        {
            return $"select count(mp.TransactionReference) id from MoniePointData mp where TerminalSerial in (select  trm.sCode from mCore_terminalmachines trm join muCore_terminalmachines trm1 on trm.iMasterId = trm1.iMasterId where trm1.Outlet = {outletid}) and  ((TransactionType = 'PURCHASE' and TransactionStatus = 'SUCCESSFUL' ) or (TransactionType ='POS_TRANSFER' and  TransactionStatus = 'APPROVED') )  and  isnull(mp.IsAllocatedToSale,0) = 0 {searchfilter}   ";
        }
        public static string GetOutstandingPaymentListPaged(int outletid,  string searchfilter)
        {
            return $"select mp.TransactionReference,mp.RetrievalReferenceNumber, mp.TerminalSerial, mp.TransactionTime,mp.AmountInLocalCur Amount from MoniePointData mp where TerminalSerial in (select  trm.sCode from mCore_terminalmachines trm join muCore_terminalmachines trm1 on trm.iMasterId = trm1.iMasterId where trm1.Outlet = {outletid}) and  ((TransactionType = 'PURCHASE' and TransactionStatus = 'SUCCESSFUL' ) or (TransactionType ='POS_TRANSFER' and  TransactionStatus = 'APPROVED') )  and  isnull(mp.IsAllocatedToSale,0) = 0 {searchfilter}  ";
        }
    }
}