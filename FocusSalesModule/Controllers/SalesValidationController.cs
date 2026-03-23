using Focus.Common.DataStructs;
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
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
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
        public HashData<string> GetUpdateAdvancePayment(AdvanceReceiptBeforeSave advance)
        {
            HashData<string> resp = new HashData<string>();
            try
            {

                if(!String.IsNullOrEmpty(advance.DocumentTagId))
                {
                    string qry = $"update  fpl_OnlinePayments set IsAllocatedToSale = 0  where TransactionReference in (select ReferenceNo from fsm_TemporaryAdvancePayments where DocumentTagId= '{advance.DocumentTagId}')";
                    DbCtx<Int32>.ExecuteNonQry(advance.CompanyId, qry);


                }
                DbCtx<Int32>.ExecuteNonQry(advance.CompanyId, AdvanceReceiptQueries.UpdateOnlinePaymentsStatus(advance.Vtype , advance.DocNo, 1));
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
        [Route("checkreturnused")]
        public HashData<String> CheckReturnedUsed(int compid, int vtype, string docno)
        {
            HashData<String> resp = new HashData<String>();
            try
            {
                int usedcount = DbCtx<Int32>.GetScalar(compid, SalesReturnQueries.CheckIfSalesReurnUsed(docno));
                if (usedcount > 0)
                {
                    resp.result = -1;
                    resp.message = "Sales return has already been used in a sale !!";
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
                resp.message = reccount > 0 ? "Sales return has been posted for this checkout" : "Sales return has not been posted for this document";


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

        // For Editing copy the original details
        [HttpPost]
        [Route("savetempadvancepayment")]
        public HashData<String> SaveTempAdvancePayment(AdvanceReceiptBeforeSave advanceBeforeSave)
        {
            HashData<String> resp = new HashData<String>();
            try
            {

                int docCount = DbCtx<Int32>.GetScalar(advanceBeforeSave.CompanyId, AdvanceReceiptQueries.AdvanceReceiptExistsQry(advanceBeforeSave.DocNo));
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

                resp.data = PaymentManager.PreProcessAdvancePayment(advanceBeforeSave);

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
        [HttpGet]
        [Route("advancepaymentafterdelete")]
        public HashData<string> AdvancePaymentAfterDelete(int compid, string sessionid, string docno, int vtype)
        {
            HashData<string> resp = new HashData<string>();
            try
            {
                DbCtx<Int32>.ExecuteNonQry(compid, AdvanceReceiptQueries.UpdateOnlinePaymentsStatus(vtype, docno,0));
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
                Hashtable docheader = JsonConvert.DeserializeObject<Hashtable>(response.data[0]["Header"].ToString());
                List<Hashtable> docbody = JsonConvert.DeserializeObject<List<Hashtable>>(response.data[0]["Body"].ToString());
                Hashtable header = PosReceiptScreenMain.BuildReceiptHeader(docheader);
                List<Hashtable> doclines = PosReceiptScreenMain.PaymentBuildReceiptLines(docbody, paymentList);


                if (headerid != 0)                    
                {

                    string listOfRefs  = $"select de.ReferenceNo from tcore_header_0 h join tCore_Data_0 d on d.iHeaderId = h.iHeaderId left join  tCore_Data4100_0 de on de.iBodyId = d.iBodyId  where h.iVoucherType = 4100 and h.svoucherno = '{deldocno}'";
                    List<string> refList = DbCtx<string>.GetObjList(compid, listOfRefs);
                    string refs = String.Join(",", refList.Select(r => $"'{r}'"));

                    string qry = $"update  fpl_OnlinePayments set IsAllocatedToSale = 0 where TransactionReference in  ({refs})";



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

                    string updateMoniePointQry = $"update fpl_OnlinePayments set IsAllocatedToSale = 1 where TransactionReference in (select  Reference from fsm_TemporaryPayments where PaymentType = 3 and DocumentTagId = '{docIdentifier}')";

                    
                    DbCtx<Int32>.ExecuteNonQry(compid, setpaymentisdone);
                    DbCtx<Int32>.ExecuteNonQry(compid, updateMoniePointQry);

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
