using FocusSalesModule.Config;
using FocusSalesModule.Data;
using FocusSalesModule.DTO;
using FocusSalesModule.Logs;
using FocusSalesModule.Models;
using FocusSalesModule.Queries;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using static FocusSalesModule.Config.AppDefaults;

namespace FocusSalesModule.Manager
{
    public class TempProcessingTxn
    {
        public int Vtype { get; set; }
        public string DocNo { get; set; }
        public string Reference { get; set; }
        public int PaymentType { get; set; } 

    }
    public class PaymentValidation
    {
        public static new Dictionary<string, List<TempProcessingTxn>> _inProcessingTxns { get; set; } = new Dictionary<string, List<TempProcessingTxn>>();

        public static void SetOnlineTransactionStatusAsUsed(PosBeforeSaveDto beforeSaveDto)
        {
            foreach (var settlement in beforeSaveDto.BillSettlement)
            {
                if (settlement.TypeSelect == (Int32)PaymentTypes.Integration)
                {
                    foreach (var pay in settlement.PayList)
                    {
                        if (pay.IsSelected)
                        {
                            string insertQry = $"insert fsm_TempDocCheck (vtype, docno , reference,createdtime) values ({beforeSaveDto.Vtype}, '{beforeSaveDto.DocNo}', '{pay.Reference}', getdate())";
                            //Update payments to in process status
                            string updateQry = "";
                            if (settlement.IntegrationType == (Int32)AppDefaults.IntegrationTypes.Moniepoint)
                            {
                                updateQry = $"update  fpl_OnlinePayments set IsAllocatedToSale = 1,TxnDocNo='{beforeSaveDto.DocNo}', Vtype={beforeSaveDto.Vtype} where IsAllocatedToSale = 0 and  TransactionReference  = '{pay.Reference}'";
                            }
                            else
                            {
                                updateQry = $"update  fpl_WemaBankTxns set IsAllocatedToSale = 1,TxnDocNo='{beforeSaveDto.DocNo}', Vtype={beforeSaveDto.Vtype} where IsAllocatedToSale = 0 and  transactionId  = '{pay.Reference}'";
                            }
                           
                            DbCtx<Int32>.ExecuteNonQry(beforeSaveDto.CompId, updateQry);
                        }
                    }
                }
            }
        }
        public static void AddTxnsToProcessingQueue(PosBeforeSaveDto beforeSaveDto)
        {
            LogInProcessTxns();
            Logger.writeLog($"Attempting to add document {beforeSaveDto.DocNo} to processing queue with payment references: {string.Join(", ", GetFlattenedTxnList(beforeSaveDto).Select(t => $"{t.PaymentType}:{t.Reference}"))}");
            //Add if no coflict
            ValidateInProcessPayment(beforeSaveDto);

            _inProcessingTxns[beforeSaveDto.UniqueId] = GetFlattenedTxnList(beforeSaveDto);

        }
        static List<TempProcessingTxn> GetFlattenedTxnList(PosBeforeSaveDto beforeSaveDto)
        {

            var tempTransactionList = new List<TempProcessingTxn>();

            foreach (var settlement in beforeSaveDto.BillSettlement)
            {
                if (settlement.TypeSelect != (Int32)PaymentTypes.Cash)
                {
                    foreach (var pay in settlement.PayList)
                    {

                        if (pay.IsSelected)
                        {
                            tempTransactionList.Add(new TempProcessingTxn
                            {
                                DocNo = beforeSaveDto.DocNo,
                                PaymentType = settlement.TypeSelect,
                                Reference = pay.Reference,
                                Vtype = beforeSaveDto.Vtype
                            });
                        }
                    }
                }
            }

            return tempTransactionList;
        }
        public static void LogInProcessTxns()
        {
            foreach (var doc in _inProcessingTxns.Keys)
            {
                var txns = _inProcessingTxns[doc];
                string logStr = $"Document {doc} is currently processing the following payment references: {string.Join(", ", txns.Select(t => $"{t.PaymentType}:{t.Reference}"))}";
                Logger.writeLog(logStr);
            }
        }
        public static void RemoveTxnFromProcessingQueue(PosBeforeSaveDto beforeSaveDto)
        {
            if (_inProcessingTxns.ContainsKey(beforeSaveDto.UniqueId))
                _inProcessingTxns.Remove(beforeSaveDto.UniqueId);
        }
        static void ValidateInProcessPayment(PosBeforeSaveDto beforeSaveDto)
        {
            Logger.writeLog($"Validating in-process payments, current records: { _inProcessingTxns.Count}");
            var currentTxn =  GetFlattenedTxnList(beforeSaveDto);

            foreach (var currentDocument in _inProcessingTxns.Keys)
            {
                Logger.writeLog($"Found ...{currentDocument}");

                var underProcessingTxns = _inProcessingTxns[currentDocument];


                foreach (TempProcessingTxn currntTxn in currentTxn)
                {
                    
                    bool isalreadyProcessing = underProcessingTxns.Any(t => t.PaymentType == currntTxn.PaymentType && t.Reference == currntTxn.Reference  && t.DocNo != currntTxn.DocNo);

                    if(isalreadyProcessing)
                        throw new Exception($"Payment reference {currntTxn.Reference} is currently being processed in another transaction, please check and try again !!");
                }

            }
        }
        public static void ValidateAllPayments(PosBeforeSaveDto beforeSaveDto)
        {
            foreach (var settlement in beforeSaveDto.BillSettlement)
            {
                
                ValidateDiscounts(beforeSaveDto.CompId, beforeSaveDto.DocDate, settlement);

                ValidateOnlinePayments(beforeSaveDto.CompId, settlement);

                ValidateCreditNotes(beforeSaveDto.CompId, settlement);

                ValidateAdvancePayments(beforeSaveDto.CompId, settlement);

            }
        }
        public static void ValidateDiscounts(int compid, int txnDate,BillSettlement settlement)
        {
            if (settlement.TypeSelect == (Int32)PaymentTypes.DiscountVoucher)
            {
                foreach (var pay in settlement.PayList)
                {

                    if (pay.IsSelected)
                    {
                        string discQry = $"select isnull(disc.sCode,'') Code, isnull(disc.sName,'') Name,dscmst.StartDate, dscmst.NoofQuantity from mCore_discountmaster disc join muCore_discountmaster dscmst on disc.iMasterId = dscmst.iMasterId where  disc.sName = '{pay.Reference}' and   disc.iMasterId <> 0   and dscmst.NoofQuantity > (select count(ReferenceNo) from tCore_Data4100_0 where ReferenceNo = disc.sCode) and   dscmst.StartDate <= {txnDate} and dscmst.EndDate >= {txnDate}";
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
                        string qry = $"select IsAllocatedToSale from vwPaymentTxns  where  TransactionReference = '{pay.Reference}' ";
                        bool isUsed = DbCtx<bool>.GetScalar(compid,qry );
                        Logger.writeLog($"Online payment reference {pay.Reference} has IsAllocatedToSale status: {isUsed}");
                        Logger.writeLog(qry);

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