using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FocusSalesModule.Controllers
{
    public class PreOrderSalesController : Controller
    {
        // GET: PreOrderSales
        public ActionResult ItemSets(int compid=540)
        {
            ViewBag.compid = compid;
            return View();
        }
    }
}