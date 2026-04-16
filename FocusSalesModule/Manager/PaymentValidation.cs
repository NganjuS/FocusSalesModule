using FocusSalesModule.Data;
using FocusSalesModule.Models;
using FocusSalesModule.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static FocusSalesModule.Config.AppDefaults;

namespace FocusSalesModule.Manager
{
    public class PaymentValidation
    {
        public static void ValidateDiscounts(int compid, int txnDate,BillSettlement settlement)
        {
            if (settlement.TypeSelect == (Int32)PaymentTypes.DiscountVoucher)
            {
                foreach (var pay in settlement.PayList)
                {

                    if (pay.IsSelected)
                    {
                        string discQry = $"select isnull(disc.sCode,'') Code, isnull(disc.sName,'') Name,dscmst.StartDate, dscmst.NoofQuantity from mCore_discountmaster disc join muCore_discountmaster dscmst on disc.iMasterId = dscmst.iMasterId where  disc.sCode = '{pay.Reference}' and   disc.iMasterId <> 0   and dscmst.NoofQuantity > (select count(ReferenceNo) from tCore_Data4100_0 where ReferenceNo = disc.sCode) and   dscmst.StartDate <= {txnDate} and dscmst.EndDate >= {txnDate}";
                        dynamic discObj = DbCtx<dynamic>.GetObj(compid, discQry);
                        if (discObj == null)
                            throw new Exception($"Discount voucher {pay.Reference} is not valid for use, please check and try again !!");
                    }
                }
            }
        }
        public static void ValidateOnlinePayments(int compid, BillSettlement settlement)
        {
            if (settlement.TypeSelect == (Int32)PaymentTypes.Integration)
            {
                foreach (var pay in settlement.PayList)
                {

                    if (pay.IsSelected)
                    {
                        bool isUsed = DbCtx<bool>.GetScalar(compid, $"select IsAllocatedToSale from fpl_OnlinePayments where  TransactionReference = '{pay.Reference}' ");
                        if (isUsed)
                            throw new Exception($"Payment reference {pay.Reference} has already been utilised for another sale, please check and try again !!");
                    }
                }
            }

        }
        public static void ValidateCreditNotes(int compid, BillSettlement settlement)
        {
            if (settlement.TypeSelect == (Int32)PaymentTypes.CreditNote)
            {
                foreach (var pay in settlement.PayList)
                {

                    if (pay.IsSelected)
                    {
                       

                        GenericPayment payment = DbCtx<GenericPayment>.GetObj(compid, SalesReturnQueries.GetSalesReturnStatusQry(pay.Reference));
                        if (payment == null)
                        {
                            throw new Exception($"Credit note {pay.Reference} does not exist or has not been posted yet, please check and try again !!");
                        }

                        if ((pay.Amount + payment.PostedAmt) > payment.FullAmt)
                        {
                            throw new Exception($"Credit note {pay.Reference} has no available balance, cannot be used for payment !!");
                        }

                    }
                }

            }
        }
        public static void ValidateAdvancePayments(int compid, BillSettlement settlement)
        {

            if (settlement.TypeSelect == (Int32)PaymentTypes.AdvanceReceipt)
            {
                foreach (var pay in settlement.PayList)
                {
                    if (pay.IsSelected) {

                        GenericPayment payment = DbCtx<GenericPayment>.GetObj(compid, AdvanceReceiptQueries.GetAdvanceReceiptTxnQry(pay.Reference));
                        if (payment == null)
                        {
                            throw new Exception($"Advanced receipt {pay.Reference} does not exist or has not been posted yet, please check and try again !!");
                        }

                        if ((pay.Amount + payment.PostedAmt) > payment.FullAmt)
                        {
                            throw new Exception($"Advanced receipt {pay.Reference} has no available balance, cannot be used for payment !!");
                        }
                    }
                }
            }
        }
    }

}