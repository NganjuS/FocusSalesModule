using FocusSalesModule.Data;
using FocusSalesModule.Logs;
using FocusSalesModule.Models;
using FocusSalesModule.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http;

namespace FocusSalesModule.Controllers
{
    [RoutePrefix("api/focuspayments")]
    public class ExternalPaymentsAPIController : ApiController
    {
        [HttpGet]
        [Route("validatepayment")]
        public HashData<dynamic> GetValidatePayment (int compid, decimal amount)
        {
            HashData<dynamic> resp = new HashData<dynamic>();
            try
            {
                Thread.Sleep(2000);
                resp.data = new
                {
                    TerminalSerial = "P260678997653",
                    InvoiceID = "82674382",
                    Amount = amount,
                    TransactionStatus = "Approved"
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
    }
}
