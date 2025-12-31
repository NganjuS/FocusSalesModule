using FocusSalesModule.Data;
using FocusSalesModule.Helpers;
using FocusSalesModule.Logs;
using FocusSalesModule.Models;
using FocusSalesModule.Models.MoniePoint;
using FocusSalesModule.Queries;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Http;

namespace FocusSalesModule.Controllers
{
    [RoutePrefix("api/paymentvalidation")]
    public class PaymentValidationController : ApiController
    {
        [HttpPost]
        [Route("save")]
        public async Task<IHttpActionResult> GetSalesData(MoniePointRequestDTO requestDTO)
        {
           
            try
            {
                Logger.writeLog(JsonConvert.SerializeObject(requestDTO));
                var headers = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());
                IEnumerable<string> webhookIdHeaders;
                IEnumerable<string> timestampHeaders;
                IEnumerable<string> signatureHeaders;
                Request.Headers.TryGetValues("moniepoint-webhook-id", out webhookIdHeaders);
                Request.Headers.TryGetValues("moniepoint-webhook-timestamp", out timestampHeaders);
                Request.Headers.TryGetValues("moniepoint-webhook-signature", out signatureHeaders);
                var webhookId = webhookIdHeaders.First();
                var timestamp = timestampHeaders.First();
                var signature = signatureHeaders.First();


                //string moniepointWebhookId = headers.ContainsKey("moniepoint-webhook-id") ? headers["moniepoint-webhook-id"] : string.Empty;
                //string moniepointWebhookSignature = headers.ContainsKey("moniepoint-webhook-signature") ? headers["moniepoint-webhook-signature"] : string.Empty;
                //string moniepointWebhookTimestamp = headers.ContainsKey("moniepoint-webhook-timestamp") ? headers["moniepoint-webhook-timestamp"] : string.Empty;

                string body = await Request.Content.ReadAsStringAsync();
                var payload = $"{webhookId}__{timestamp}__{body}";

                // Step 4: Compute signature

                string  validationId = WebConfigurationManager.AppSettings["ValidationId"].ToString();
               
                var computedSignature = ComputeHmacSha256(payload, validationId);
                bool isTxnValid = true;
                if (!SlowEquals(computedSignature, signature))
                {
                    Logger.writeLog($"Payment was invalid, received signature is {webhookId}, Computed signature is {computedSignature}");
                    isTxnValid = false;
                    //return Unauthorized();
                }

                  int compId = Convert.ToInt32( WebConfigurationManager.AppSettings["DbId"].ToString());

                try
                {
                    string qry = MoneyPointQuery.InsertTransactionsTable(requestDTO, webhookId, timestamp, isTxnValid);
                    Logger.writeLog(qry);
                    //Insert into database
                    DbCtx<Int32>.ExecuteNonQry(compId , qry);
                }
                catch (Exception ex)
                {
                    Logger.writeLog(ex.Message);
                }

               
            }
            catch (Exception ex)
            {
                Logger.writeLog(ex.Message);
                Logger.writeLog(ex.StackTrace);
                string body = await Request.Content.ReadAsStringAsync();
                Logger.writeLog(body);
           
            }
            return Ok();
        }
        [HttpGet]
        [Route("healthcheck")]
        public async Task<IHttpActionResult> HealthCheck()
        {
            try
            {
                int compId = Convert.ToInt32(WebConfigurationManager.AppSettings["DbId"].ToString());
                int i = DbCtx<Int32>.GetScalar(compId, "select top 500 * from  tcore_header_0 ");
                return Ok($"Server is Alive record count:{i} ");
            }
            catch (Exception ex)
            {
                Logger.writeLog(ex.Message);
                Logger.writeLog(ex.StackTrace);
                return InternalServerError(ex);
            }
            
        }
        private string ComputeHmacSha256(string data, string secret)
        {
            var secretBytes = Encoding.UTF8.GetBytes(secret);
            var dataBytes = Encoding.UTF8.GetBytes(data);

            using (var hmac = new HMACSHA256(secretBytes))
            {
                var hashBytes = hmac.ComputeHash(dataBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }
        private bool SlowEquals(string a, string b)
        {
            if (a == null || b == null) return false;

            uint diff = (uint)a.Length ^ (uint)b.Length;
            for (int i = 0; i < a.Length && i < b.Length; i++)
                diff |= (uint)(a[i] ^ b[i]);

            return diff == 0;
        }

    }
}
