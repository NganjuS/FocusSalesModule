using FocusSalesModule.Data;
using FocusSalesModule.Helpers;
using FocusSalesModule.Logs;
using FocusSalesModule.Models;
using FocusSalesModule.Queries;
using FocusSalesModule.Screens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Web.Http;
using System.Web.UI;

namespace FocusSalesModule.Controllers
{
    [RoutePrefix("api/preordersales")]
    public class PreOrderSaleAPIController : ApiController
    {
        [HttpGet]
        [Route("itemsets")]
        public HashData<GenericMaster> GetSalesData(string term = "", int compid = 0, int page = 1, int pagesize = 10)
        {
            HashData<GenericMaster> resp = new HashData<GenericMaster>();
            try
            {

                string filterparams = String.Empty;
                if (!String.IsNullOrEmpty(term))
                {

                    filterparams += $" and (  sVoucherNo  like '%{term}%') ";
                }

                string orderby = " sVoucherNo asc ";
                string orderbyparams = GeneralUtils.BuildQueryParams(orderby, page, pagesize);
                int reccount = DbCtx<Int32>.GetScalar(compid, MasterQueries.GetItemSetCountQry(filterparams));

                int totalPages = reccount % pagesize != 0 ? (reccount / pagesize) + 1 : reccount / pagesize;

                resp.datalist = DbCtx<GenericMaster>.GetObjList(compid, MasterQueries.GetItemSetQry(filterparams, orderbyparams));

                resp.pagingStatus = new PagingStatusMain()
                {
                    currentPage = page,
                    recordsTotal = reccount,
                    totalPages = totalPages,
                    hasMore = (page * pagesize) < reccount
                };
                resp.result = 1;


            }
            catch (Exception ex)
            {
                resp.result = -1;
                resp.message = ex.Message;
                Logger.writeLog(ex.Message);
                Logger.writeLog(ex.StackTrace);

            }
            return resp;
        }
        [HttpGet]
        [Route("loadsets")]
        public HashData<ItemSet> LoadSets(int compid, int pageno = 1, int pagesize = 10,  string searchval = "")
        {
            HashData<ItemSet> resp = new HashData<ItemSet>();
            try
            {
                string orderby = "  sVoucherNo asc ";
                string orderbyparams = GeneralUtils.BuildQueryParams(orderby, pageno, pagesize);
                string filterparams = $"  and (  sVoucherNo  like '%{searchval}%') ";

               

                resp.datalist = DbCtx<ItemSet>.GetObjList(compid, MasterQueries.GetItemSetQry(filterparams, orderbyparams));

                int reccount = DbCtx<Int32>.GetScalar(compid, MasterQueries.GetItemSetCountQry(filterparams));

                int totalPages = reccount % pagesize != 0 ? (reccount / pagesize) + 1 : reccount / pagesize;

                resp.pagingStatus = new PagingStatusMain()
                {
                    currentPage = pageno,
                    recordsTotal = reccount,
                    totalPages = totalPages
                };
                resp.result = 1;


            }
            catch (Exception ex)
            {
                resp.result = -1;
                resp.message = ex.Message;
                Logger.writeLog(ex.Message);
                Logger.writeLog(ex.StackTrace);

            }
            return resp;
        }
        [HttpGet]
        [Route("loaditems")]
        public HashData<ItemSetLine> LoadItems(int compid, int itemId)
        {
            HashData<ItemSetLine> resp = new HashData<ItemSetLine>();
            try
            {            
                resp.datalist = DbCtx<ItemSetLine>.GetObjList(compid, MasterQueries.GetItemSetLines(itemId));
                resp.result = 1;
            }
            catch (Exception ex)
            {
                resp.result = -1;
                resp.message = ex.Message;
                Logger.writeLog(ex.Message);
                Logger.writeLog(ex.StackTrace);
            }
            return resp;
        }
    }
}
