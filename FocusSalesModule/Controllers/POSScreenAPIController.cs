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
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Remoting.Messaging;
using System.Web.Configuration;
using System.Web.Http;

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
        [HttpPost]
        [Route("addsale")]
        public HashDataFocus AddSale(int compid, string sessionid, POSDTO posDTO)
        {
            HashDataFocus hashDataFocus = new HashDataFocus();
            try
            {
                string baseUrl = WebConfigurationManager.AppSettings["Server_API_IP"];
                HashDataFocus objHashRequest = new HashDataFocus();
                Hashtable objHash = new Hashtable();

                //var docHeader = posDTO.Items.FirstOrDefault();
                posDTO.SalesAccId = 27;
                posDTO.CustAccId = 4;
                posDTO.MemberId = 1;
                //posDTO.OutletId = 42;
                //posDTO.CostCenterId = 6;
                posDTO.Narration = "Test Posting";
                Hashtable header = POSSales.GetHeader(posDTO);
                List<Hashtable> doclines = POSSales.GetLines(posDTO.Items);
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
     }

}
