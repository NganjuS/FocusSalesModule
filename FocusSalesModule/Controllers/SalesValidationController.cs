using FocusSalesModule.Config;
using FocusSalesModule.Data;
using FocusSalesModule.DTO;
using FocusSalesModule.Helpers;
using FocusSalesModule.Logs;
using FocusSalesModule.Manager;
using FocusSalesModule.Models;
using FocusSalesModule.Queries;
using FocusSalesModule.Screens;
using FocusSalesModule.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;

using System.Web.Configuration;
using System.Web.Http;
using System.Web.UI.WebControls;
using static FocusSalesModule.Helpers.AppUtilities;

namespace FocusSalesModule.Controllers
{
    [RoutePrefix("api/salespayments")]
    public class SalesValidationController : ApiController
    {

      
        [HttpGet]
        [Route("validatesale")]
        public HashData<dynamic> GetValidateSale(int compid, int outletid, bool manualvalidate,string reference,decimal outstandingamt)
        {
            HashData<dynamic> resp = new HashData<dynamic>();
            try
            {
                dynamic retrievedData = DbCtx<dynamic>.GetObj(compid, PaymentDetailsQueries.GetOutstandingAmtDetails(outletid, manualvalidate,  reference, outstandingamt));
                if(retrievedData == null)
                {
                    resp.result = -1;
                    resp.message = "No valid payment found. ";
                    return resp;
                }

                resp.data = retrievedData;
                resp.result = 1;

            }
            catch (Exception ex)
            {
                Logger.writeLog(ex.Message);
                Logger.writeLog(ex.StackTrace);
                resp.result = -1;
                resp.message = ex.Message;
            }
            return resp;
        }
        [HttpGet]
        [Route("manualonlinepayment")]
        public HashData<OnlinePaymentDTO> GetOnlinePaymentList(int compid, int integrationtype, int outletid,  string reference)
        {
            HashData<OnlinePaymentDTO> resp = new HashData<OnlinePaymentDTO>();
            try
            {

                resp.datalist = DbCtx<OnlinePaymentDTO>.GetObjList(compid, PaymentDetailsQueries.GetManualSearchPayment(outletid, integrationtype,reference));
                resp.result = 1;

            }
            catch (Exception ex)
            {
                Logger.writeLog(ex.Message);
                Logger.writeLog(ex.StackTrace);

                resp.result = -1;
                resp.message = ex.Message;
            }
            return resp;

        }
        [HttpGet]
        [Route("onlinepaymentlist")]
        public HashData<PagingStatus<OnlinePaymentDTO>> GetOnlinePaymentList(int compid, int maxmin, int integrationtype, int outletid, int pageno = 1, int pagesize = 10, string searchval = "")
        {
            HashData<PagingStatus<OnlinePaymentDTO>> resp = new HashData<PagingStatus<OnlinePaymentDTO>>();
            try
            {
               
                    resp.data = OnlinePaymentVM.GetPagedOnlinePayments(compid, outletid, integrationtype, maxmin,pageno, pagesize, searchval);
               

                    resp.result = 1;

            }
            catch (Exception ex)
            {
                Logger.writeLog(ex.Message);
                Logger.writeLog(ex.StackTrace);
    
                resp.result = -1;
                resp.message = ex.Message;
            }
            return resp;
        }
        [Route("validateadvancepaymentadd")]
        public HashData<dynamic> ValidateAdvancePaymentAdd(PosBeforeSaveDto beforeSaveDto)
        {
            HashData<dynamic> resp = new HashData<dynamic>();
            try
            {
                // Perform validation logic here
                // If validation fails, set resp.result = -1 and resp.message accordingly


                //Validate if two transactions are not being processed at the same time for the same document
                Logger.writeLog("Processing advanced payments ...");
               
                //Find if payment has been utilised

                PaymentValidation.ValidateAllPayments(beforeSaveDto);
                PaymentValidation.SetOnlineTransactionStatusAsUsed(beforeSaveDto);

                resp.result = 1;
            }
            catch (Exception ex)
            {
                Logger.writeLog(ex.Message);
                Logger.writeLog(ex.StackTrace);
                resp.result = -1;
                resp.message = ex.Message;
            }
            finally
            {
                PaymentValidation.RemoveTxnFromProcessingQueue(beforeSaveDto);
            }
            return resp;
        }
        [HttpPost]
        [Route("availablediscount")]
        public HashData<dynamic> GetAvailableDiscount(int compid, int vtype, int outletid, DiscountRequestDto discountRequest)
        {
            HashData<dynamic> resp = new HashData<dynamic>();
            try
            {
                string itemJoin = String.Join(",",discountRequest.Item);

                resp.datalist = DbCtx<dynamic>.GetObjList(compid, PaymentDetailsQueries.GetDiscountCodes(itemJoin, discountRequest.TxnDate));

                resp.result = 1;

            }
            catch (Exception ex)
            {
                Logger.writeLog(ex.Message);
                Logger.writeLog(ex.StackTrace);
                resp.result = -1;
                resp.message = ex.Message;
            }
            return resp;
        }
        [HttpPost]
        [Route("availablecreditnotes")]
        public HashData<dynamic> GetAvailableCreditNotes(int compid, int vtype, int outletid, int memberid, CreditNoteRequestDto requestDto)
        {
            HashData<dynamic> resp = new HashData<dynamic>();
            try
            {


                string rmaQuery = String.Join(",", requestDto.RmaNoList.Select(r => $"'{r}'"));

                resp.datalist = DbCtx<dynamic>.GetObjList(compid, PaymentDetailsQueries.GetCreditNotes(memberid, rmaQuery));

                resp.result = 1;

            }
            catch (Exception ex)
            {
                Logger.writeLog(ex.Message);
                Logger.writeLog(ex.StackTrace);
                resp.result = -1;
                resp.message = ex.Message;
            }
            return resp;
        }
        [HttpPost]
        [Route("availableadvancereceipts")]
        public HashData<dynamic> GetAvailableAdvanceReceipts(int compid, int vtype, int outletid, int memberid, CreditNoteRequestDto requestDto)
        {
            HashData<dynamic> resp = new HashData<dynamic>();
            try
            {
            
                resp.datalist = DbCtx<dynamic>.GetObjList(compid, PaymentDetailsQueries.GetAdvanceReceipts(memberid));

                resp.result = 1;

            }
            catch (Exception ex)
            {
                Logger.writeLog(ex.Message);
                Logger.writeLog(ex.StackTrace);
                resp.result = -1;
                resp.message = ex.Message;
            }
            return resp;
        }
        [HttpGet]
        [Route("paymentdetails")]
        public HashData<dynamic> GetPaymentDetails(int compid, int outletid)
        {
            HashData<dynamic> resp = new HashData<dynamic>();
            try
            {

                resp.datalist = DbCtx<dynamic>.GetObjList(compid, PaymentDetailsQueries.GetDetails(outletid));
                resp.result = 1;

            }
            catch (Exception ex)
            {
                Logger.writeLog(ex.Message);
                Logger.writeLog(ex.StackTrace);
                resp.result = -1;
                resp.message = ex.Message;
            }
            return resp;
        }
        [HttpPost]
        [Route("updateadvancepayment")]
        public HashData<string> GetUpdateAdvancePayment(AdvanceReceiptAfterSave aftersaveadvance)
        {
            var advance = aftersaveadvance.AdvanceReceipt;

            HashData<string> resp = new HashData<string>();
            try
            {

                
                //Clear references for previous transaction
                if (aftersaveadvance.AdvanceReceipt.DocLines.Count > 0)
                {
                    string refFilterList = String.Join(",", aftersaveadvance.AdvanceReceipt.DocLines.Select(r => $"'{r}'"));
                   


                    string qry = $"update  fpl_OnlinePayments set IsAllocatedToSale = 0 , TxnDocNo='', Vtype=0 where IsAllocatedToSale = 1 and TransactionReference in ({refFilterList})";
                    DbCtx<Int32>.ExecuteNonQry(advance.CompanyId, qry);                    

                }



                //if(!String.IsNullOrEmpty(advance.DocumentTagId))
                //{

                //    string qry = $"update  fpl_OnlinePayments set IsAllocatedToSale = 0  where TransactionReference in (select ReferenceNo from fsm_TemporaryAdvancePayments where DocumentTagId= '{advance.DocumentTagId}')";
                //    DbCtx<Int32>.ExecuteNonQry(advance.CompanyId, qry);


                //}

                //Check if document is deleted
                int docCount = DbCtx<Int32>.GetScalar(advance.CompanyId, AdvanceReceiptQueries.AdvanceReceiptExistsQry(advance.DocNo, advance.Vtype));
                Logger.writeLog($"Document is deleted : {docCount}");
                if(docCount > 0)
                {
                    string onlineqry = AdvanceReceiptQueries.UpdateOnlinePaymentsStatus(advance.Vtype, advance.DocNo, 1);
                  
                    DbCtx<Int32>.ExecuteNonQry(advance.CompanyId, onlineqry);
                }
                else
                {
                    string onlineqry = AdvanceReceiptQueries.ClearPaymentsStatusAfterDel( advance.DocNo, advance.Vtype);
                 
                    DbCtx<Int32>.ExecuteNonQry(advance.CompanyId, onlineqry);
                }
                resp.result = 1;



            }
            catch (Exception ex)
            {
                Logger.writeLog(ex.Message);
                Logger.writeLog(ex.StackTrace);
                resp.result = -1;
                resp.message = ex.Message;
            }
            return resp;
        }

        [HttpPost]
        [Route("checkreturnused")]
        public HashData<String> CheckReturnedUsed(TxnVoucherData salesrtndata)
        {
            HashData<String> resp = new HashData<String>();
            try
            {
                int usedcount = DbCtx<Int32>.GetScalar(salesrtndata.CompanyId, SalesReturnQueries.CheckIfSalesReturnUsed(salesrtndata.DocNo));
                if (usedcount > 0)
                {
                    resp.result = -1;
                    resp.message = "Sales return has already been used in a sale !!";
                    return resp;
                }

                foreach(var line in salesrtndata.DocItemLines)
                {
                    //Find if all schemes items exist
                    //if(line.LinkedItem == 0 && !String.IsNullOrEmpty(line.SchemeDocNo))
                    //{
                    //    List<Int32> freeItems = salesrtndata.DocItemLines.Where(x => x.LinkedItem == line.Item).Select(x => x.Item).ToList();

                    //    List<SchemeItemLine> schemeItems = DbCtx<SchemeItemLine>.GetObjList(salesrtndata.CompanyId, SalesReturnQueries.GetSchemeItems(salesrtndata.POSDocNo, line.Item));

                    //    foreach(var sch in schemeItems)
                    //    {
                    //        if(!freeItems.Contains(sch.ItemId))
                    //        {
                    //            throw new Exception($"Free Item '{sch.Item}' is missing in the sales return lines !!!");
                    //        }
                    //    }
                    //}

                    //if (line.LinkedItem != 0)
                    //{
                    //    if(!salesrtndata.DocItemLines.Any(x => x.Item == line.LinkedItem))
                    //    {
                    //        throw new Exception($"Free item is missing its main item cannot continue !!!");
                    //    }
                    //}
                    //Validate that that the POS / Set Checkout has been done and RMA is not returned in another sales return
                    foreach (var rma in line.RMA)
                    {
                        var hashData = DbCtx<dynamic>.GetObj(salesrtndata.CompanyId, ProductQueries.GetSalesReturnRmaData(rma, $" and hh.svoucherno <> '{salesrtndata.DocNo}' "));

                        if (hashData == null)
                        {
                            throw new Exception($"RMA {rma} has been utilised or not valid !!!");
                        }

                    }
                }


                resp.result = 1;
            }
            catch (Exception ex)
            {
                Logger.writeLog(ex.Message);
                Logger.writeLog(ex.StackTrace);
                resp.result = -1;
                resp.message = ex.Message;
            }
            return resp;
        }
        [HttpGet]
        [Route("issalesreturnposted")]
        public HashData<String> IsSalesReturnPosted(int compid, string docno)
        {
            HashData<String> resp = new HashData<String>();
            try
            {
                string qry = $"select count(POSDocNo) from tCore_HeaderData1793_0 where POSDocNo='{docno}'";
                int reccount = DbCtx<Int32>.GetScalar(compid, qry);
                resp.result = reccount == 0 ? 1 : -1;
                resp.message = reccount > 0 ? "Sales return has been posted for this checkout, it cannot be editted or deleted" : "Sales return has not been posted for this document";


            }
            catch(Exception ex)
            {
                Logger.writeLog(ex.Message);
                Logger.writeLog(ex.StackTrace);
                resp.result = -1;
                resp.message = ex.Message;
            }
            return resp;
        }

        [HttpGet]
        [Route("delposreceipt")]
        public HashData<String> DelPosReceipt(int compid, string sessionid,string docno)
        {
            HashData<String> resp = new HashData<String>();
            try
            {
                string qry = $"select h.sVoucherNo from tCore_HeaderData4100_0 he join tCore_Header_0 h on h.iHeaderId = he.iHeaderId where he.POSDocNo = '{docno}'";

                string bnkqry = $"select pl.ReferenceNo from tCore_HeaderData4100_0 he join tCore_Header_0 h on h.iHeaderId = he.iHeaderId left join tCore_Data_0 dd on dd.iHeaderId = h.iHeaderId left join tCore_Data4100_0 pl on pl.iBodyId = dd.iBodyId left join tCore_Data_Tags_0 tg on tg.iBodyId = pl.iBodyId left join mCore_paymenttype pyt on pyt.iMasterId = tg.iTag3012 left join muCore_paymenttype pyte on pyte.iMasterId = pyt.iMasterId  where  pyte.TypeSelect =  3 and  he.POSDocNo = '{docno}'";
                List<string> txnRefList = DbCtx<string>.GetObjList(compid, bnkqry);

                string reptno = DbCtx<string>.GetScalar(compid, qry);
                if(!String.IsNullOrEmpty(reptno))
                {
                    string baseUrl = WebConfigurationManager.AppSettings["Server_API_IP"];
                    string delUrl = $"{baseUrl}/Transactions/FBI POS Receipt/{reptno.Replace("/", "~~")}";

                    HashDataFocus delResp = APIManager.delDocument(sessionid, delUrl);

                    //if (delResp.result < 1)
                    //{
                    //    resp.result = -1;
                    //    resp.message = "Error in updating payment. Check User Rights: " + delResp.message;
                       
                    //}
                    if(delResp.result > 0 && txnRefList.Count > 0)
                    {
                        string filterParam = String.Join(",",txnRefList.Select(x => $"'{x}'").ToArray());

                        DbCtx<Int32>.ExecuteNonQry(compid, $"update  fpl_OnlinePayments set IsAllocatedToSale = 0 where TransactionReference  in ({filterParam})");
                    }

                    resp.result= delResp.result;                  
                    resp.message = delResp.message;
                    return resp;

                }
                resp.result = 1;


            }
            catch (Exception ex)
            {
                Logger.writeLog(ex.Message);
                Logger.writeLog(ex.StackTrace);
                resp.result = -1;
                resp.message = ex.Message;
            }
            return resp;
        }

        //[HttpGet]
        //[Route("deladvancepayment")]
        //public HashData<String> DelAdvancePayment(int compid, int vtype, string docno)
        //{
        //    HashData<String> resp = new HashData<String>();
        //    try
        //    {

        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.writeLog(ex.Message);
        //        Logger.writeLog(ex.StackTrace);
        //        resp.result = -1;
        //        resp.message = ex.Message;
        //    }
        //    return resp;
        //}
        // For Editing copy the original details
        [HttpPost]
        [Route("savetempadvancepayment")]
        public HashData<dynamic> SaveTempAdvancePayment(TxnVoucherData advanceBeforeSave)
        {
            HashData<dynamic> resp = new HashData<dynamic>();
            try
            {

                //Check if reference is used in another transaction
                if (advanceBeforeSave.DocLines != null)
                {
                    string refFilterList = String.Join(",", advanceBeforeSave.DocLines.Select(r => $"'{r.ReferenceNo}'"));
                    //Check if payment reference if used in another transaction
                    string rt = $"select count(Id) from fpl_OnlinePayments where  TransactionReference in ({refFilterList}) and IsAllocatedToSale = 1 and TxnDocNo <> '{advanceBeforeSave.DocNo}' ";
                    int count = DbCtx<Int32>.GetScalar(advanceBeforeSave.CompanyId, rt);
                    if (count > 0)
                    {
                        resp.result = -1;
                        resp.message = "One or more of the payment references have been used in another transaction. Please check and try again.";
                        return resp;
                    }
                }

                int docCount = DbCtx<Int32>.GetScalar(advanceBeforeSave.CompanyId, AdvanceReceiptQueries.AdvanceReceiptExistsQry(advanceBeforeSave.DocNo, advanceBeforeSave.Vtype));

                if(docCount == 0)
                {
                    resp.result = 2;
                    resp.message = "Advance Receipt was not created.";
                    return resp;

                }

                int usedcount = DbCtx<Int32>.GetScalar(advanceBeforeSave.CompanyId, AdvanceReceiptQueries.CheckIfAdvancedReceiptUsed(advanceBeforeSave.DocNo));   
                if(usedcount > 0)
                {
                    resp.result = -1;
                    resp.message = "Advance receipt has been used in a sale !!";
                    return resp;
                }

               



                string docIdentifier = PaymentManager.PreProcessAdvancePayment(advanceBeforeSave);

                List<string> refList = DbCtx<string>.GetObjList(advanceBeforeSave.CompanyId, AdvanceReceiptQueries.GetAdvanceReceiptReferenceNo(advanceBeforeSave.Vtype, advanceBeforeSave.DocNo));

                resp.data =  new
                {
                    docIdentifier,
                    refList 
                };          

                resp.result = 1;
            }
            catch (Exception ex)
            {
                Logger.writeLog(ex.Message);
                Logger.writeLog(ex.StackTrace);
                resp.result = -1;
                resp.message = ex.Message;
            }
            return resp;
        }
        //Update Advance Payment OnlinePayments After

        //For Advance receipts
        [HttpPost]
        [Route("updatepaymentstatus")]
        public HashData<string> GetDatePayment(List<AdvanceReceiptDelLine>  receiptDelLine)
        {
            //For advance payments
            HashData<string> resp = new HashData<string>();
            try
            {

                var headerObj = receiptDelLine.FirstOrDefault();
                foreach (var dataObj in receiptDelLine)
                {
                    
                    var paymentPaymentDto = DbCtx<PaymentTypeDto>.GetObj(headerObj.CompanyId, PaymentDetailsQueries.GetPaymentTypeQry(dataObj.PaymentType)); 

                    if(paymentPaymentDto.TypeSelect == (Int32)AppDefaults.PaymentTypes.Integration)
                    {
                       DbCtx<Int32>.ExecuteNonQry(headerObj.CompanyId, AdvanceReceiptQueries.UpdatePaymentsStatus(0, dataObj.ReferenceNo));
                    }


                }

                

                resp.result = 1;



            }
            catch (Exception ex)
            {
                Logger.writeLog(ex.Message);
                Logger.writeLog(ex.StackTrace);
                resp.result = -1;
                resp.message = ex.Message;
            }
            return resp;
        }
        [HttpPost]
        [Route("advancepaymentafterdelete")]
        public HashData<string> AdvancedPaymentAfterDel(List<AdvanceReceiptDelLine> receiptDelLine)
        {
            //For advance payments
            HashData<string> resp = new HashData<string>();
            try
            {

                var headerObj = receiptDelLine.FirstOrDefault();
                foreach (var dataObj in receiptDelLine)
                {

                    var paymentPaymentDto = DbCtx<PaymentTypeDto>.GetObj(headerObj.CompanyId, PaymentDetailsQueries.GetPaymentTypeQry(dataObj.PaymentType));

                    if (paymentPaymentDto.TypeSelect == (Int32)AppDefaults.PaymentTypes.Integration)
                    {
                        DbCtx<Int32>.ExecuteNonQry(headerObj.CompanyId, AdvanceReceiptQueries.AdvancedUpdatePaymentsStatus(dataObj.ReferenceNo));
                    }


                }



                resp.result = 1;



            }
            catch (Exception ex)
            {
                Logger.writeLog(ex.Message);
                Logger.writeLog(ex.StackTrace);
                resp.result = -1;
                resp.message = ex.Message;
            }
            return resp;
        }
        [HttpGet]
        [Route("updatereceiptstatus")]
        public HashData<string> UpdateReceiptStatus(int compid, string sessionid, int vtype, string docno )
        {
            HashData<string> resp = new HashData<string>();
            try
            {
                string rcptdocno = DbCtx<string>.GetScalar(compid, $"select h.svoucherno from tCore_Header_0 h join tCore_HeaderData4100_0 he on he.iHeaderId = h.iHeaderId where he.POSDocNo = '{docno.Trim()}'");

                string baseUrl = WebConfigurationManager.AppSettings["Server_API_IP"];

                string wUrl = $"{baseUrl}/screen/transactions/{PosReceiptScreenMain.screenName}/{rcptdocno.Replace("/", "~~")}";

                HashDataFocus response = APIManager.getData(sessionid, wUrl);
                if (response.result <= 0)
                {
                    resp.result = response.result;
                    resp.message = response.message;
                    return resp;

                }
                Hashtable docheader = JsonConvert.DeserializeObject<Hashtable>(response.data[0]["Header"].ToString());


                docheader.Remove("Flags");
                docheader.Remove("Time");
                docheader.Remove("Net");
                docheader.Remove("TransactionNet");
                docheader["Flags"] = new Hashtable() { { "Suspended", false } };

                //Hashtable flagDetails = JsonConvert.DeserializeObject<Hashtable>(docheader["Flags"].ToString());

                List<Hashtable> doclines = JsonConvert.DeserializeObject<List<Hashtable>>(response.data[0]["Body"].ToString());


                QuickFuncs.RemoveExtraMasterFields(docheader);

                foreach(var line in doclines)
                {
                    QuickFuncs.RemoveExtraMasterFields(line);
                    line.Remove("BodyFlags");
                }

               // bool isApproved = Convert.ToBoolean(flagDetails["Approved"]);



                HashDataFocus objHashRequest = new HashDataFocus();
                Hashtable objHash = new Hashtable();
                objHash.Add("Header", docheader);
                objHash.Add("Body", doclines);
                List<Hashtable> lstHash = new List<Hashtable>();
                lstHash.Add(objHash);
                objHashRequest.data = lstHash;
                string url = $"{baseUrl}/Transactions/{PosReceiptScreenMain.screenName}";
                HashDataFocus hashDataFocus = APIManager.postData(objHashRequest, sessionid, url);
                //DbCtx<Int32>.ExecuteNonQry(compid, $"update tCore_Header_0 set bSuspended = 0 where iHeaderId = {headerid}");
                resp.result = 1;



            }
            catch (Exception ex)
            {
                Logger.writeLog(ex.Message);
                Logger.writeLog(ex.StackTrace);
                resp.result = -1;
                resp.message = ex.Message;
            }
            return resp;
        }
        [HttpGet]
        [Route("updatepaymentdetails")]
        public HashData<dynamic> GetUpdatePaymentDetails(int compid,string sessionid,string docno,int vtype,string docIdentifier)
        {
            HashData<dynamic> resp = new HashData<dynamic>();
            try
            {
                string baseUrl = WebConfigurationManager.AppSettings["Server_API_IP"];

                List<TemporaryPaymentDataDto> paymentList = DbCtx<TemporaryPaymentDataDto>.GetObjList(compid, $"select * from fsm_TemporaryPayments where DocumentTagId = '{docIdentifier}'");
                //if (PosReceiptScreenMain.IsDefaultAccountsNotSet(paymentList))
                //{
                //    resp.result = -1;
                //    resp.message = "Outlet accounts have not been set";
                //    return resp;
                //}
                int headerid = 0;
                string deldocno = "";
                if (PosReceiptScreenMain.IsReceiptPosted(compid, docno))
                {
                    headerid = PosReceiptScreenMain.GetReceiptHeaderid(compid, docno);
                    deldocno = PosReceiptScreenMain.GetReceiptDocNo(compid, docno);
                    //resp.result = -1;
                    //resp.message = "Payment already posted for this sale";
                    //return resp;
                }




                //Post to payment screen

                string wUrl = $"{baseUrl}/screen/transactions/{AppUtilities.GetScreenName(compid, vtype)}/{docno.Replace("/", "~~")}";
                HashDataFocus response = APIManager.getData(sessionid, wUrl);
                if(response.result <= 0)
                {
                    resp.result = response.result;
                    resp.message = response.message;
                    return resp;

                }
                Hashtable docheader = JsonConvert.DeserializeObject<Hashtable>(response.data[0]["Header"].ToString());
                Hashtable flagDetails = JsonConvert.DeserializeObject<Hashtable>(docheader["Flags"].ToString());
                bool isApproved = Convert.ToBoolean(flagDetails["Approved"]);
               

                List<Hashtable> docbody = JsonConvert.DeserializeObject<List<Hashtable>>(response.data[0]["Body"].ToString());


                Hashtable header = PosReceiptScreenMain.BuildReceiptHeader(docheader, isApproved);
                List<Hashtable> doclines = PosReceiptScreenMain.PaymentBuildReceiptLines(docbody, paymentList);


                if (headerid != 0)                    
                {

                    string listOfRefs  = $"select de.ReferenceNo from tcore_header_0 h join tCore_Data_0 d on d.iHeaderId = h.iHeaderId left join  tCore_Data4100_0 de on de.iBodyId = d.iBodyId  where h.iVoucherType = 4100 and h.svoucherno = '{deldocno}'";
                    List<string> refList = DbCtx<string>.GetObjList(compid, listOfRefs);
                    string refs = String.Join(",", refList.Select(r => $"'{r}'"));

                    string qry = $"update  fpl_OnlinePayments set IsAllocatedToSale = 0, TxnDocNo='', Vtype=0 where TransactionReference in  ({refs})";

                    string wemaqry = $"update  fpl_WemaBankTxns set IsAllocatedToSale = 0, TxnDocNo='', Vtype=0 where transactionId in  ({refs})";

                    //Delete existing receipt to update with new payment details
                    string delUrl = $"{baseUrl}/Transactions/{PosReceiptScreenMain.screenName}/{deldocno.Replace("/", "~~")}";

                    HashDataFocus delResp = APIManager.delDocument(sessionid, delUrl);
                    if(delResp.result != 1)
                    {
                        resp.result = -1;
                        resp.message = "Error in updating payment. Check User Rights: " + delResp.message;
                        return resp;
                    }
                    else
                    {
                        DbCtx<Int32>.ExecuteNonQry(compid, qry);
                        DbCtx<Int32>.ExecuteNonQry(compid, wemaqry);
                    }

                    
                }
                header["DocNo"] = deldocno;

                HashDataFocus objHashRequest = new HashDataFocus();
                Hashtable objHash = new Hashtable();
                objHash.Add("Header", header);
                objHash.Add("Body", doclines);
                List<Hashtable> lstHash = new List<Hashtable>();
                lstHash.Add(objHash);
                objHashRequest.data = lstHash;
                string url = $"{baseUrl}/Transactions/{PosReceiptScreenMain.screenName}";
                HashDataFocus hashDataFocus = APIManager.postData(objHashRequest, sessionid, url);

                resp.result = hashDataFocus.result;


                string mssg = headerid == 0 ? "Payment Posted successfully" : "Payment Updated  successfully";

                resp.message = hashDataFocus.result != 1 ? hashDataFocus.message : mssg;

                if (hashDataFocus.result == 1)
                {
                    //Update pos header
                    string qry = AppUtilities.GetQueryUpdate(compid, vtype, paymentList);

                    string clearFieldsQry = GetClearDataFieldsQry(compid, vtype);

                    string clrUpdateFieldsQry = clearFieldsQry.Trim().Length > 0 ? $"update tCore_HeaderData{vtype}_0 set {clearFieldsQry} where iheaderid = (select top 1 iheaderid from tcore_header_0 where sVoucherNo = '{docno}' and ivouchertype ={vtype})  " : "";


                    string updateQry = qry.Trim().Length > 0 ? $"update tCore_HeaderData{vtype}_0 set PaymentPosted = '1', {qry} where iheaderid = (select top 1 iheaderid from tcore_header_0 where sVoucherNo = '{docno}' and ivouchertype ={vtype})  " : "";

                    string setpaymentisdone = $"update fsm_TemporaryPayments set IsValidated = 1  where DocumentTagId = '{docIdentifier}' ";

                    string updateMoniePointQry = $"update fpl_OnlinePayments set IsAllocatedToSale = 1, TxnDocNo='{docno}', Vtype={vtype} where TransactionReference in (select  Reference from fsm_TemporaryPayments where PaymentType = 3 and DocumentTagId = '{docIdentifier}')";

                    string updateWemaBankQry = $"update fpl_WemaBankTxns set IsAllocatedToSale = 1, TxnDocNo='{docno}', Vtype={vtype} where transactionId in (select  Reference from fsm_TemporaryPayments where PaymentType = 3 and DocumentTagId = '{docIdentifier}')";


                    DbCtx<Int32>.ExecuteNonQry(compid, setpaymentisdone);
                    DbCtx<Int32>.ExecuteNonQry(compid, updateMoniePointQry);
                    DbCtx<Int32>.ExecuteNonQry(compid, updateWemaBankQry);

                    if (updateQry.Trim().Length > 0)
                    {
                        DbCtx<Int32>.ExecuteNonQry(compid, clrUpdateFieldsQry);
                        DbCtx<Int32>.ExecuteNonQry(compid, updateQry);
                       
                    }
                }


            }
            catch (Exception ex)
            {
                Logger.writeLog(ex.Message);
                Logger.writeLog(ex.StackTrace);
                resp.result = -1;
                resp.message = ex.Message;
            }
            return resp;
        }
    }
}
