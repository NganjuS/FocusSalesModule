using Focus.Common.DataStructs;
using FocusSalesModule.Data;
using FocusSalesModule.DTO;
using FocusSalesModule.Helpers;
using FocusSalesModule.Logs;
using FocusSalesModule.Models;
using FocusSalesModule.Screens;
using FocusSalesModule.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static FocusSalesModule.Helpers.AppUtilities;

namespace FocusSalesModule.Controllers
{
    public class TransactionManagerController : Controller
    {
        // GET: TransactionManager
        public ActionResult Index(int compid = 540)
        {
            ViewBag.compid = compid;
            return View();
        }
        public ActionResult GetAllPaged(int compid, int pageno = 1, int pagesize = 10, string searchval = "")
        {
            HashData<PagingStatus<MoniepointTxnDto>> resp = new HashData<PagingStatus<MoniepointTxnDto>>();
            try
            {
                
                 resp.data = OnlinePaymentVM.GetPagedTxnOnlinePayments(compid, pageno, pagesize, searchval);
                

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
        public ActionResult UpdateTxnStatus(int compid, int txnid, int status)
        {
            HashData<string> resp = new HashData<string>();
            try
            {

                OnlinePaymentVM.UpdateTxnStatus(compid, txnid);
                resp.message = "Transaction status updated successfully";
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
    }
}