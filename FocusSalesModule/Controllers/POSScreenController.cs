using System;
using System.Collections.Generic;
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
        public ActionResult PrintCashSale(string docNo, int compid = 72)
        {
            ViewBag.compid = compid;
            return View();
        }
    }
}