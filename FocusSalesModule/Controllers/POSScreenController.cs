using CrystalDecisions.CrystalReports.Engine;
using Focus.Common.DataStructs;
using FocusSalesModule.Data;
using FocusSalesModule.Manager;
using FocusSalesModule.Models;
using FocusSalesModule.Queries;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FocusSalesModule.Controllers
{
    public class POSScreenController : Controller
    {
        // GET: POSScreen
        public ActionResult Index(int compid=72)
        {
            ViewBag.compid = compid;
          
            return View();
        }
        public ActionResult PrintCashSaleByDocNo(string docNo, int compid = 72)
        {
            ViewBag.compid = compid;

            return View();
        }
        public ActionResult PrintCashSale(int compid = 72, int headerid=0)
        {
            ReportDocument newdoc = ReportManager.POSSale(compid, headerid);
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            Response.AddHeader("Content-disposition", "inline; filename=POS Sale.pdf");
            Stream stream = newdoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            return new FileStreamResult(stream, "application/pdf");
        }
    }
}