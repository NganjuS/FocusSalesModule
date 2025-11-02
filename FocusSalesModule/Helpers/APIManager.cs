using FocusSalesModule.Logs;
using FocusSalesModule.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace FocusSalesModule.Helpers
{
    public class APIManager
    {
        public static HashDataFocus postData(HashDataFocus hdt, string sess, string url)
        {
            string sContent = JsonConvert.SerializeObject(hdt);
            using (var client = new WebClient())
            {
                client.Headers.Add("fSessionId", sess);
                client.Headers.Add("Content-Type", "application/json");
                string sUrl = url;
                string strResponse = "";
                try
                {
                    strResponse = client.UploadString(sUrl, sContent);
                }
                catch (Exception ex)
                {
                    HashDataFocus mn = new HashDataFocus();
                    mn.result = -2;
                    mn.message = ex.Message;
                    return mn;
                }

                int h = 0;
                //MpesaController.Logger.Info(JsonConvert.DeserializeObject(strResponse));

                return JsonConvert.DeserializeObject<HashDataFocus>(strResponse);

            }
        }
        public static HashDataFocus getData(string sess, string url)
        {
            //string sContent = JsonConvert.SerializeObject(hdt);
            using (var client = new WebClient())
            {
                client.Headers.Add("fSessionId", sess);
                client.Headers.Add("Content-Type", "application/json");
                Logger.writeLog(url);
                Logger.writeLog(sess);
                string sUrl = url;
                string strResponse = client.DownloadString(sUrl);

                return JsonConvert.DeserializeObject<HashDataFocus>(strResponse);

            }
        }
    }
}