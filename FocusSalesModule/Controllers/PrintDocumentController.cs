using FocusSalesModule.Data;
using FocusSalesModule.DTO;
using FocusSalesModule.Helpers;
using FocusSalesModule.Logs;
using FocusSalesModule.Models;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace FocusSalesModule.Controllers
{
    public class PrintDocumentController : Controller
    {
        // GET: PrintDocument
        public ActionResult Index(int compid,int vtype, string sessionid, string docno)
        {
            int authStatus = DbCtx<Int32>.GetScalar(compid, $"select iAuth from tCore_Header_0 where sVoucherNo = '{docno}' and  iVoucherType = {vtype}");

            if(authStatus != 1)
                throw new Exception("Not allowed to print.");

            int printCount = DbCtx<Int32>.GetScalar(compid, $"select iPrintCount from tCore_Header_0 where sVoucherNo = '{docno}' and  iVoucherType = {vtype}");
            if (printCount > 0)
                throw new Exception("Max print count already utilised !!! ");
            int saleLayoutId = Convert.ToInt32(WebConfigurationManager.AppSettings["SaleLayoutId"].ToString());
            Hashtable reportParams = new Hashtable();
            HashDataFocus req = new HashDataFocus();
            req.data = new List<Hashtable>();
            string filename = $"CheckOutPrintOut_{docno}";
            req.data.Add(new Hashtable
                    {
                        { "VoucherTypeId", vtype },
                        { "LayoutId", saleLayoutId },
                        { "VoucherNo", docno },
                        { "FileName",  filename}
                    });

            
            string baseUrl = WebConfigurationManager.AppSettings["Server_API_IP"];
            string printurl = $"{baseUrl}/Transactions/PrintLayout";

            
            HashDataFocus resp = APIManager.postData(req, sessionid, printurl);
            byte[] pdfBytes = Array.Empty<byte>();
            if (resp.result == 1)
            {

                

                DbCtx<Int32>.ExecuteNonQry(compid, $"update tCore_Header_0 set iPrintCount={printCount + 1}  where sVoucherNo = '{docno}' and  iVoucherType = {vtype}");

                var filePath = $@"C:\\Windows\\Temp\\{filename}.pdf";
                var contentType = "application/pdf";
                var fileName = $"{filename}.pdf";

                return File(filePath, contentType, fileName);


            }
            var stream = new MemoryStream(pdfBytes);
            stream.Position = 0;
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            Response.AddHeader("Content-disposition", "inline; filename=POS Sale.pdf");

            stream.Seek(0, SeekOrigin.Begin);

            return new FileStreamResult(stream, "application/pdf");



        }
        public ActionResult ResetPrintCount(int compid, int vtype, string docno)
        {
            HashDataFocus resp = new HashDataFocus();
            try
            {
                int printCount = DbCtx<Int32>.GetScalar(compid, $"select iPrintCount from tCore_Header_0 where sVoucherNo = '{docno}' and  iVoucherType = {vtype}");
                printCount = printCount <= 0 ? 0 : printCount - 1;

                DbCtx<Int32>.ExecuteNonQry(compid, $"update tCore_Header_0 set iPrintCount={printCount}  where sVoucherNo = '{docno}' and  iVoucherType = {vtype}");

                resp.result = 1;

            }
            catch (Exception ex)
            {
                Logger.writeLog(ex.Message);
                Logger.writeLog(ex.StackTrace);
                resp.result = -1;
                resp.message = ex.Message;
            }
            return Json(resp, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SilentPrint(int compid, int vtype, string sessionid, string docno)
        {
            HashDataFocus resp = new HashDataFocus();
            try
            {
                
                resp.result = -1;

                int authStatus = DbCtx<Int32>.GetScalar(compid, $"select iAuth from tCore_Header_0 where sVoucherNo = '{docno}' and  iVoucherType = {vtype}");
                int printCount = DbCtx<Int32>.GetScalar(compid, $"select iPrintCount from tCore_Header_0 where sVoucherNo = '{docno}' and  iVoucherType = {vtype}");

                if (authStatus != 1)
                    throw new Exception("Not allowed to print.");
                if (printCount > 0)
                    throw new Exception("Max print count already utilised !!! ");

                int saleLayoutId = Convert.ToInt32(WebConfigurationManager.AppSettings["SaleLayoutId"].ToString());

                Hashtable reportParams = new Hashtable();
                HashDataFocus req = new HashDataFocus();
                req.data = new List<Hashtable>();
                string filename = $"CheckOutPrintOut_{docno}";
                req.data.Add(new Hashtable
                    {
                        { "VoucherTypeId", vtype },
                        { "LayoutId", saleLayoutId },
                        { "VoucherNo", docno },
                        { "FileName",  filename}
                    });


                string baseUrl = WebConfigurationManager.AppSettings["Server_API_IP"];
                string printurl = $"{baseUrl}/Transactions/PrintLayout";
               resp = APIManager.postData(req, sessionid, printurl);
                //byte[] pdfBytes = Array.Empty<byte>();
                if (resp.result == 1)
                {

                   

                    DbCtx<Int32>.ExecuteNonQry(compid, $"update tCore_Header_0 set iPrintCount=1  where sVoucherNo = '{docno}' and  iVoucherType = {vtype}");



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

            return Json(resp, JsonRequestBehavior.AllowGet);

        }
        
        public ActionResult Sign(string request)
        {
            var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly);

            var cert = store.Certificates
                .Find(X509FindType.FindBySubjectName, "fbstores.ng", false)
                .OfType<X509Certificate2>()
                .Where(c => c.NotAfter > DateTime.Now && c.HasPrivateKey)  // valid + has private key
                .OrderByDescending(c => c.NotAfter)
                .FirstOrDefault();

            store.Close();

            var data = Encoding.UTF8.GetBytes(request);

            // Debug output
            Logger.writeLog("Input string: " + request);
            Logger.writeLog("Data hex: " + BitConverter.ToString(data).Replace("-", ""));
            Logger.writeLog("Cert subject: " + cert.Subject);
            Logger.writeLog("Cert thumbprint: " + cert.Thumbprint);

          

            using (var rsa = cert.GetRSAPrivateKey())
            {
                var signature = rsa.SignData(data, HashAlgorithmName.SHA512, RSASignaturePadding.Pkcs1);
                return Content(Convert.ToBase64String(signature), "text/plain");
            }
        }
        public ActionResult GetPublicCert()
        {
            var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly);
            var cert = store.Certificates
    .Find(X509FindType.FindBySubjectName, "fbstores.ng", false)
    .OfType<X509Certificate2>()
    .Where(c => c.NotAfter > DateTime.Now)  // only valid certs
    .OrderByDescending(c => c.NotAfter)      // pick the latest one
    .FirstOrDefault();
            store.Close();

            // Export as PEM
            var certPem = "-----BEGIN CERTIFICATE-----\r\n" +
                          Convert.ToBase64String(cert.RawData, Base64FormattingOptions.InsertLineBreaks) +
                          "\r\n-----END CERTIFICATE-----";

            return Content(certPem, "text/plain");
        }
        public ActionResult DownloadOverrideCert()
        {

            var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly);
            var cert = store.Certificates
                .Find(X509FindType.FindBySubjectName, "fbstores.ng", false)
                .OfType<X509Certificate2>()
                .FirstOrDefault();
            store.Close();

            var pem = "-----BEGIN CERTIFICATE-----\r\n" +
                      Convert.ToBase64String(cert.RawData, Base64FormattingOptions.InsertLineBreaks) +
                      "\r\n-----END CERTIFICATE-----";

            var bytes = System.Text.Encoding.ASCII.GetBytes(pem);
            return File(bytes, "application/x-pem-file", "override.crt");
        }

        // GET /api/qz/certificate
        public ActionResult Certificate()
        {
            var certText = System.IO.File.ReadAllText(Server.MapPath("~/digital-certificate.txt"));
            return Content(certText, "text/plain");
        }
       }
}