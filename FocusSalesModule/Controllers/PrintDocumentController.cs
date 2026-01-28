using FocusSalesModule.DTO;
using FocusSalesModule.Helpers;
using FocusSalesModule.Models;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
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
    }
}