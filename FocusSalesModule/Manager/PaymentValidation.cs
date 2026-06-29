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
using System.Web.SessionState;
using System.Windows.Forms;
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
        public static int UpdatePaymentStatus(int compId,int vtype, string docNo,  string refFilterList)
        {
            
            //Check if payment reference if used in another transaction
            //string refCheckQry = $"select count(Id) from vwPaymentTxns where  TransactionReference in ({refFilterList}) and TxnDocNo is not null and  TxnDocNo <> ''  and TxnDocNo <> '{beforeSaveDto.DocNo}' ";
            string wemarefQuery = $"UPDATE fpl_WemaBankTxns\r\nSET TxnDocNo = @DocNo,Vtype = @Vtype, IsAllocatedToSale = 1, ReservedAt = SYSUTCDATETIME(), IsConfirmed = 0\r\nWHERE transactionId IN ({refFilterList})\r\n    AND (\r\n        TxnDocNo IS NULL OR TxnDocNo = ''\r\n        OR TxnDocNo = @DocNo\r\n        OR (IsConfirmed = 0 AND ReservedAt < DATEADD(MINUTE, -3, SYSUTCDATETIME()))\r\n      );\r\nSELECT @@ROWCOUNT;";
            string onlinePayQry = $"\r\nUPDATE fpl_OnlinePayments\r\nSET TxnDocNo = @DocNo,Vtype = @Vtype, IsAllocatedToSale = 1, ReservedAt = SYSUTCDATETIME(), IsConfirmed = 0\r\nWHERE TransactionReference IN ({refFilterList})\r\n  AND   (\r\n        TxnDocNo IS NULL OR TxnDocNo = ''\r\n        OR TxnDocNo = @DocNo\r\n        OR (IsConfirmed = 0 AND ReservedAt < DATEADD(MINUTE, -3, SYSUTCDATETIME()))\r\n      );\r\nSELECT @@ROWCOUNT;";

            //UPDATE fpl_WemaBankTxns SET IsConfirmed = 1
            //WHERE TxnDocNo = @DocNo AND transactionId IN @refs;
            int count = 0;
            count += DbCtx<Int32>.GetScalar(compId, wemarefQuery, new { refs = refFilterList, docNo, vtype });
            count += DbCtx<Int32>.GetScalar(compId, onlinePayQry, new { refs = refFilterList, docNo, vtype });
            Logger.writeLog($"{count} rows affected");
            return count;
        }

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
        public static void ValidateAllPayments(PosBeforeSaveDto beforeSaveDto, string docTagId)
        {
            foreach (var settlement in beforeSaveDto.BillSettlement)
            {
                
                ValidateDiscounts(beforeSaveDto.CompId, beforeSaveDto.DocDate, settlement);

                //ValidateOnlinePayments(beforeSaveDto.CompId, settlement);

                ValidateCreditNotes(beforeSaveDto.CompId, docTagId, settlement);

                ValidateAdvancePayments(beforeSaveDto.CompId, docTagId, settlement);

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
        public static void ValidateCreditNotes(int compid, string docNo,BillSettlement settlement)
        {
            if (settlement.TypeSelect == (Int32)PaymentTypes.CreditNote)
            {
                foreach (var curpayment in settlement.PayList)
                {

                    if (curpayment.IsSelected)
                    {
                       

                        GenericPayment dbpayment = DbCtx<GenericPayment>.GetObj(compid, SalesReturnQueries.GetSalesReturnStatusQry(curpayment.Reference));
                        if (dbpayment == null)
                        {
                            throw new Exception($"Credit note {curpayment.Reference} does not exist or has not been posted yet, please check and try again !!");
                        }
                        SaveTempPayment(compid, docNo, curpayment, dbpayment);
                        //if ((pay.Amount + payment.PostedAmt) > payment.FullAmt)
                        //{
                        //    throw new Exception($"Credit note {pay.Reference} has no available balance, cannot be used for payment !!");
                        //}

                    }
                }

            }
        }
        public static void ValidateAdvancePayments(int compid, string docNo,BillSettlement settlement)
        {

            if (settlement.TypeSelect == (Int32)PaymentTypes.AdvanceReceipt)
            {
                foreach (var curpayment in settlement.PayList)
                {
                    if (curpayment.IsSelected) {

                        GenericPayment dbpayment = DbCtx<GenericPayment>.GetObj(compid, AdvanceReceiptQueries.GetAdvanceReceiptTxnQry(curpayment.Reference));
                        if (dbpayment == null)
                        {
                            throw new Exception($"Advanced receipt {curpayment.Reference} does not exist or has not been posted yet, please check and try again !!");
                        }
                        SaveTempPayment(compid, docNo, curpayment, dbpayment);
                        //if ((pay.Amount + payment.PostedAmt) > payment.FullAmt)
                        //{
                        //    throw new Exception($"Advanced receipt {pay.Reference} has no available balance, cannot be used for payment !!");
                        //}
                    }
                }
            }
        }
        static void SaveTempPayment(int compid,string docNo, GenericPayment curpayment, GenericPayment dbposted)
        {
            string sql = "INSERT INTO fsm_PaymentsReservations (Reference, DocNo, Amount, ReservedAt, IsConfirmed)\r\nSELECT @Reference, @DocNo, @Amount, SYSUTCDATETIME(), 0\r\nWHERE (\r\n    -- ERP posted balance\r\n    @FullAmt\r\n    - @PostedAmt\r\n    -- minus other live reservations (confirmed, or still-fresh provisional)\r\n    - ISNULL((\r\n        SELECT SUM(Amount) FROM fsm_PaymentsReservations\r\n        WHERE Reference = @Reference\r\n          AND DocNo <> @DocNo\r\n          AND (IsConfirmed = 1\r\n               OR ReservedAt > DATEADD(MINUTE, -2, SYSUTCDATETIME()))\r\n      ), 0)\r\n  ) >= @Amount;\r\n\r\nSELECT @@ROWCOUNT;";

            int reserved = DbCtx<Int32>.GetScalar(compid,sql, new
            {
                curpayment.Reference,
                DocNo = docNo,
                curpayment.Amount,
                dbposted.FullAmt,
                 dbposted.PostedAmt
            });

            if (reserved == 0)
            {
                // insufficient balance once concurrent reservations are counted -> tell UI to STOP
                throw new Exception($" {curpayment.Reference} has no available balance !!");
            }
        }

    }

}