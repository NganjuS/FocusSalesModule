using CrystalDecisions.CrystalReports.Engine;
using Focus.Common.DataStructs;
using FocusSalesModule.Data;
using FocusSalesModule.Helpers;
using FocusSalesModule.Manager;
using FocusSalesModule.Models;
using FocusSalesModule.Queries;
using Microsoft.Ajax.Utilities;
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
        public ActionResult OpenPOS(int compid, int vtype,int outletid, string sessionid, string docno  )
        {
            ViewBag.compid = compid;
            ViewBag.sessionid = sessionid;
            ViewBag.outletid = outletid;
           
            ViewBag.vtype = vtype;
            ViewBag.docno = docno;
            string txnqry = $"select abs(fNet) NetAmt from tCore_Header_0 where iVoucherType = {vtype} and sVoucherNo='{docno}'";
            decimal netAmt = DbCtx<Decimal>.GetScalar(compid, txnqry);

            ViewBag.netamt = netAmt;

            return View();
        }
        public ActionResult OpenPOSBeforeSave(int compid, int vtype, int outletid, int memberid, string sessionid, string docno, decimal amount)
        {
            ExecuteCreateTable.RunQueries(compid);

            ViewBag.compid = compid;
            ViewBag.sessionid = sessionid;
            ViewBag.outletid = outletid;
            ViewBag.memberid = memberid;
            ViewBag.vtype = vtype;
            ViewBag.docno = docno;
            ViewBag.netamt = amount;

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
            newdoc.Close();
            newdoc.Dispose();
            return new FileStreamResult(stream, "application/pdf");
        }
    }
}