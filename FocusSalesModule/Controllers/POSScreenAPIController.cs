using Focus.Common.DataStructs;
using FocusSalesModule.Data;
using FocusSalesModule.Helpers;
using FocusSalesModule.Logs;
using FocusSalesModule.Models;
using FocusSalesModule.Queries;
using FocusSalesModule.Screens;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Web.Configuration;
using System.Web.Http;
using System.Xml.Linq;

namespace FocusSalesModule.Controllers
{
    [RoutePrefix("api/sales")]
    public class POSScreenAPIController : ApiController
    {
        const string ScreenName = "FBI POS Sales";
        [HttpGet]
        [Route("salesdata")]
        public HashData<POSDTO> GetSalesData(int compid, int outletid, DateTime datefrom , DateTime dateto)
        {
            HashData<POSDTO> resp = new HashData<POSDTO>();
            try
            {
                int startDate = UtilFuncs.GetDateToInt(datefrom);
                int endDate = UtilFuncs.GetDateToInt(dateto);
                resp.datalist = DbCtx<POSDTO>.GetObjList(compid, TxnQueries.GetPOSData(outletid, startDate, endDate));
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
        [Route("salesbytxnid")]
        public HashData<POSDTO> GetSalesData(int compid, int headerid)
        {
            HashData<POSDTO> resp = new HashData<POSDTO>();
            try
            {

                POSDTO headerObj  = DbCtx<POSDTO>.GetObj(compid, TxnQueries.GetPOSDataByHeaderId(headerid));
                headerObj.Items = DbCtx<Product>.GetObjList(compid,  TxnQueries.GetPOSLinesByHeaderId(headerid));
                foreach(var item in headerObj.Items)
                {
                    item.RmaNoList = DbCtx<String>.GetObjList(compid, TxnQueries.GetRmaNoListQry(item.LineId));
                }

                resp.data = headerObj;

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
        [Route("outletpaymenttypes")]
        public HashData<dynamic> GetOutletPaymenttypes(int compid, int outletid)
        {
            HashData<dynamic> resp = new HashData<dynamic>();
            try
            {
                resp.datalist = DbCtx<dynamic>.GetObjList(compid, TxnQueries.GetOutletPaymentTypesQry(outletid));
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
        [Route("discountvoucher")]
        public HashData<dynamic> GetDiscountVoucher(int compid,DateTime txnDate,string discVoucherNo)
        {
            HashData<dynamic> resp = new HashData<dynamic>();
            try
            {
                int txnDateNw = UtilFuncs.GetDateToInt(txnDate);
                resp.data = DbCtx<dynamic>.GetObj(compid, TxnQueries.GetDiscVoucher(txnDateNw, discVoucherNo));
                resp.result = 1;
                if (resp.data == null)
                {
                    resp.result = -1;
                    resp.message = "Voucher does not exist or has expired !!!";
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
        [HttpPost]
        [Route("addsale")]
        public HashDataFocus AddSale(int compid, string sessionid, POSDTO posDTO)
        {
            HashDataFocus hashDataFocus = new HashDataFocus();
            try
            {
                string baseUrl = WebConfigurationManager.AppSettings["Server_API_IP"];
              
                //Get Cash Customer 
                Outlet outlet = DbCtx<Outlet>.GetObj(compid, MasterQueries.GetOutlet(posDTO.OutletId));
                if (outlet.DefaultCustomer == 0 || outlet.DefaultSalesAccount == 0 || outlet.DefaultCostCenter == 0 || outlet.DefaultBankAccount == 0 || outlet.DefaultCashAccount == 0 || outlet.DefaultOnlineAccount == 0 || outlet.DefaultCreditNoteAccount == 0 || outlet.DefaultDiscountAccount == 0)
                {
                    throw new Exception("Set default accounts in outlet to continue !!!");
                }
                //var docHeader = posDTO.Items.FirstOrDefault();
                //posDTO.SalesAccId = outlet.DefaultSalesAccount;
                //posDTO.CustAccId = outlet.DefaultCustomer;
                posDTO.MemberId = 1;
                //posDTO.OutletId = 42;
                //posDTO.CostCenterId = 6;
                posDTO.Narration = "Test Posting";
                Hashtable header = POSSales.GetHeader(posDTO, outlet);
                List<Hashtable> doclines = POSSales.GetLines(posDTO.Items);

                HashDataFocus objHashRequest = new HashDataFocus();
                Hashtable objHash = new Hashtable();
                objHash.Add("Header", header);
                objHash.Add("Body", doclines);
                List<Hashtable> lstHash = new List<Hashtable>();
                lstHash.Add(objHash);
                objHashRequest.data = lstHash;
                string url = $"{baseUrl}/Transactions/{ScreenName}";
                hashDataFocus = APIManager.postData(objHashRequest, sessionid, url);
                Logger.writeLog(JsonConvert.SerializeObject(hashDataFocus.data));

                if (hashDataFocus.result == 1)
                {
                    string headerId = hashDataFocus.data[0]["HeaderId"].ToString();
                    string docNo= hashDataFocus.data[0]["VoucherNo"].ToString();

                    Hashtable recptHeader = PosReceiptScreen.GetReceiptHeader(posDTO, outlet, docNo);
                    List<Hashtable> recptLines = PosReceiptScreen.GetReceiptLines(posDTO, outlet);

                    HashDataFocus payResult = PostPayment(baseUrl, sessionid, recptHeader, recptLines);
                    int isSuccess = payResult.result == 1 ? 1 : 0;
                    string updatePosQry = $"update tCore_HeaderData3334_0 set paymentposted = {isSuccess} where iHeaderId = {headerId}";

                    DbCtx<Int32>.ExecuteNonQry(compid, updatePosQry);
                   
                    hashDataFocus.message = "Sale Posted succesfully";
                    hashDataFocus.printlink = $"/focussalesmodule/posscreen/printcashsale?compid={compid}&headerid={headerId}";
                }
               


            }
            catch (Exception ex)
            {
                Logger.writeLog(ex.Message);
                Logger.writeLog(ex.StackTrace);
                hashDataFocus.result = -1;
                hashDataFocus.message = ex.Message;
            }
            return hashDataFocus;
        }
        [HttpPost]
        [Route("addadvancepayment")]
        public HashDataFocus AddAdvancePayment(int compid, string sessionid, AdvancedPaymentDTO advancedPayment)
        {
            HashDataFocus hashDataFocus = new HashDataFocus();
            try
            {
                string baseUrl = WebConfigurationManager.AppSettings["Server_API_IP"];
                Outlet outlet = DbCtx<Outlet>.GetObj(compid, MasterQueries.GetOutlet(advancedPayment.OutletId));
                Hashtable header = AdvanceReceipt.GetReceiptHeader(advancedPayment, outlet);
                List<Hashtable> doclines = AdvanceReceipt.GetReceiptLines(advancedPayment, outlet);

                HashDataFocus objHashRequest = new HashDataFocus();
                Hashtable objHash = new Hashtable();
                objHash.Add("Header", header);
                objHash.Add("Body", doclines);
                List<Hashtable> lstHash = new List<Hashtable>();
                lstHash.Add(objHash);
                objHashRequest.data = lstHash;
                string url = $"{baseUrl}/Transactions/ADVANCE RECEIPT";
                hashDataFocus = APIManager.postData(objHashRequest, sessionid, url);
                if (hashDataFocus.result == 1)
                {
                    string docNo = hashDataFocus.data[0]["VoucherNo"].ToString();
                    hashDataFocus.message = $"Created document {docNo} successfully";
                   
                }

             }
            catch (Exception ex)
            {
                Logger.writeLog(ex.Message);
                Logger.writeLog(ex.StackTrace);
                hashDataFocus.result = -1;
                hashDataFocus.message = ex.Message;
            }
            return hashDataFocus;
        }
        [HttpGet]
        [Route("outlets")]
        public HashData<RequestDTO> GetOutlets(int compId, int loginId)
        {
            HashData<RequestDTO> resp = new HashData<RequestDTO>();
            try
            {
                
                RequestDTO requestDTO = new RequestDTO();
                requestDTO.DocNo = DocUtilities.GetNextDocNo(compId);
                requestDTO.Outlets = DbCtx<Outlet>.GetObjList(compId, MasterQueries.GetAllowedOutlets(loginId));
                requestDTO.CostCenters = DbCtx<MasterDTO>.GetObjList(compId, MasterQueries.GetAllowedCostCenters(loginId));
                requestDTO.BankAccounts = DbCtx<MasterDTO>.GetObjList(compId, MasterQueries.GetAllowedBankAccounts(loginId));
                requestDTO.Members = DbCtx<MasterDTO>.GetObjList(compId, MasterQueries.GetMembersQry());
                resp.data = requestDTO;

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
        [Route("addmember")]
        public HashData<MasterDTO> AddMember(int compid, string sessionid, MemberDTO member)
        {
            HashData<MasterDTO> data = new HashData<MasterDTO>();
            HashDataFocus hashDataFocus = new HashDataFocus();
            try
            {
                string baseUrl = WebConfigurationManager.AppSettings["Server_API_IP"];
                HashDataFocus objHashRequest = new HashDataFocus();
                Hashtable objHash = new Hashtable()
                {
                    {  "sCode" ,  member.Name },
                    {  "sName" ,  member.Name },
                    { "iMemberType__Id", 1},
                    {  "Mobile" ,  "" },

                };
               
                List<Hashtable> lstHash = new List<Hashtable>();
                lstHash.Add(objHash);
                objHashRequest.data = lstHash;
                string url = $"{baseUrl}/Masters/Pos__Member";
                hashDataFocus = APIManager.postData(objHashRequest, sessionid, url);
                data.result = hashDataFocus.result;
                data.message = hashDataFocus.message;
                if(hashDataFocus.result == 1)
                {
                    data.datalist = DbCtx<MasterDTO>.GetObjList(compid, MasterQueries.GetMembersQry());
                }
            }
            catch(Exception ex) {

                data.result = -1;
                data.message = ex.Message;
            }
            return data;
        }
        HashDataFocus PostPayment(string baseUrl,string sessionid, Hashtable header, List<Hashtable> lines)
        {
            HashDataFocus hashDataFocus  = new HashDataFocus();
            HashDataFocus objHashRequest = new HashDataFocus();
            Hashtable objHash = new Hashtable();
            objHash.Add("Header", header);
            objHash.Add("Body", lines);
            List<Hashtable> lstHash = new List<Hashtable>();
            lstHash.Add(objHash);
            objHashRequest.data = lstHash;
            string url = $"{baseUrl}/Transactions/{PosReceiptScreen.screenName}";
            hashDataFocus = APIManager.postData(objHashRequest, sessionid, url);
            Logger.writeLog(JsonConvert.SerializeObject(hashDataFocus.data));
            return hashDataFocus;
        }

     }

}
