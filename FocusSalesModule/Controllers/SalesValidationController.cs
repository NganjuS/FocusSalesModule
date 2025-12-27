using Focus.Common.DataStructs;
using FocusSalesModule.Data;
using FocusSalesModule.DTO;
using FocusSalesModule.Helpers;
using FocusSalesModule.Logs;
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
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Web.Configuration;
using System.Web.Http;
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

                resp.datalist = DbCtx<OnlinePaymentDTO>.GetObjList(compid, PaymentDetailsQueries.GetManualSearchPayment(outletid, reference));
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
                if(integrationtype == (Int32)IntegrationTypes.Moniepoint)
                {
                    resp.data = OnlinePaymentVM.GetPagedOnlinePayments(compid, outletid, maxmin,pageno, pagesize, searchval);
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
        [HttpGet]
        [Route("updatepaymentdetails")]
        public HashData<string> GetUpdatePaymentDetails(int compid,string sessionid,string docno,int vtype,string docIdentifier)
        {
            HashData<string> resp = new HashData<string>();
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

                if (PosReceiptScreenMain.IsReceiptPosted(compid, docno))
                {
                    resp.result = -1;
                    resp.message = "Payment already posted for this sale";
                    return resp;
                }




                //Post to payment screen

                string wUrl = $"{baseUrl}/screen/transactions/{AppUtilities.GetScreenName(compid, vtype)}/{docno.Replace("/", "~~")}";
                HashDataFocus response = APIManager.getData(sessionid, wUrl);
                Hashtable docheader = JsonConvert.DeserializeObject<Hashtable>(response.data[0]["Header"].ToString());
                List<Hashtable> docbody = JsonConvert.DeserializeObject<List<Hashtable>>(response.data[0]["Body"].ToString());
                Hashtable header = PosReceiptScreenMain.BuildReceiptHeader(docheader);
                List<Hashtable> doclines = PosReceiptScreenMain.PaymentBuildReceiptLines(docbody, paymentList);

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
                resp.message = hashDataFocus.result != 1 ? hashDataFocus.message : "Payment Posted successfully";
                if (hashDataFocus.result == 1)
                {
                    //Update pos header
                    string qry = AppUtilities.GetQueryUpdate(compid, vtype, paymentList);

                    string updateQry = qry.Trim().Length > 0 ? $"update tCore_HeaderData{vtype}_0 set PaymentPosted = '1', {qry} where iheaderid = (select top 1 iheaderid from tcore_header_0 where sVoucherNo = '{docno}' and ivouchertype ={vtype})  " : "";

                    string setpaymentisdone = $"update fsm_TemporaryPayments set IsValidated = 1  where DocumentTagId = '{docIdentifier}' ";

                    string updateMoniePointQry = $"update MoniePointData set IsAllocatedToSale = 1 where TransactionReference in (select  Reference from fsm_TemporaryPayments where PaymentType = 3 and DocumentTagId = '{docIdentifier}')";

                    DbCtx<Int32>.ExecuteNonQry(compid, setpaymentisdone);
                    DbCtx<Int32>.ExecuteNonQry(compid, updateMoniePointQry);

                    if (updateQry.Trim().Length > 0)
                    {
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
