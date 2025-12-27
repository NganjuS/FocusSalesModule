using FocusSalesModule.Data;
using FocusSalesModule.DTO;
using FocusSalesModule.Helpers;
using FocusSalesModule.Queries;
using FocusSalesModule.Screens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace FocusSalesModule.ViewModel
{
    public class OnlinePaymentVM
    {
        public static PagingStatus<OnlinePaymentDTO> GetPagedOnlinePayments(int compid, int outletid,int maxmin, int pageno, int pagesize, string searchval)
        {
            int recpageno = pageno == 1 ? 0 : ((pageno - 1) * pagesize);
            PagingStatus<OnlinePaymentDTO> pagingData = new PagingStatus<OnlinePaymentDTO>();
            pagingData.currentPage = pageno;

            pagingData.data = new List<OnlinePaymentDTO>();
            searchval = AppUtilities.SanitizeStr(searchval);
            DateTime filterdate = DateTime.Now.AddMinutes(-1*maxmin);
            string filterparam = $"  and mp.TransactionTime >= '{filterdate.ToString("yyyy-MM-dd HH:mm:ss.fff")}' ";

            string orderbyextra = " mp.TransactionTime desc ";
            string extraparams = GeneralUtils.BuildQueryParams(orderbyextra, pageno, pagesize);
            if (!String.IsNullOrEmpty(searchval))
            {
               filterparam += $" and mp.TransactionReference ='{searchval}' ";
                pagingData.recordsTotal = DbCtx<Int32>.GetScalar(compid , PaymentDetailsQueries.GetOutstandingPaymentListPagedCount(outletid, filterparam));

                pagingData.data = DbCtx<OnlinePaymentDTO>.GetObjList(compid, PaymentDetailsQueries.GetOutstandingPaymentListPaged(outletid, $"{filterparam} {extraparams}"));
            }
            else
            {
                pagingData.recordsTotal = DbCtx<Int32>.GetScalar(compid, PaymentDetailsQueries.GetOutstandingPaymentListPagedCount(outletid, filterparam));
                pagingData.data = DbCtx<OnlinePaymentDTO>.GetObjList(compid, PaymentDetailsQueries.GetOutstandingPaymentListPaged(outletid, $"{filterparam} {extraparams}"));
            }
            pagingData.totalPages = pagingData.recordsTotal % pagesize != 0 ? (pagingData.recordsTotal / pagesize) + 1 : pagingData.recordsTotal / pagesize;

            return pagingData;
        }
    }
}