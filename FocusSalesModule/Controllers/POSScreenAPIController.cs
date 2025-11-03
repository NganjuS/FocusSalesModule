using Focus.Common.DataStructs;
using FocusSalesModule.Data;
using FocusSalesModule.Helpers;
using FocusSalesModule.Logs;
using FocusSalesModule.Models;
using FocusSalesModule.Queries;
using FocusSalesModule.Screens;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Configuration;
using System.Web.Http;

namespace FocusSalesModule.Controllers
{
    [RoutePrefix("api/sales")]
    public class POSScreenAPIController : ApiController
    {
        const string ScreenName = "FBI POS Sales";
        [HttpPost]
        [Route("addsale")]
        public HashDataFocus AddSale(int compid, string sessionid, POSDto posDTO)
        {
            HashDataFocus hashDataFocus = new HashDataFocus();
            try
            {
                string baseUrl = WebConfigurationManager.AppSettings["Server_API_IP"];
                HashDataFocus objHashRequest = new HashDataFocus();
                Hashtable objHash = new Hashtable();
               
                var docHeader = posDTO.Items.FirstOrDefault();
                docHeader.SalesAccId = 27;
                docHeader.CustAccId = 4;
                docHeader.MemberId = 1;
                docHeader.OutletId = 42;
                docHeader.CostCenterId = 6;
                docHeader.Narration = "Test Posting";
                Hashtable header = POSSales.GetHeader(posDTO);
                List<Hashtable> doclines = POSSales.GetLines(posDTO.Items);
                objHash.Add("Header", header);
                objHash.Add("Body", doclines);
                List<Hashtable> lstHash = new List<Hashtable>();
                lstHash.Add(objHash);
                objHashRequest.data = lstHash;
                string url = $"{baseUrl}/Transactions/{ScreenName}";
                hashDataFocus = APIManager.postData(objHashRequest, sessionid, url);
                hashDataFocus.message = hashDataFocus.result == 1 ? "Sale Posted succesfully" : hashDataFocus.message;


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
