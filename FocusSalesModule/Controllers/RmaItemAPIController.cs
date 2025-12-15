using FocusSalesModule.Data;
using FocusSalesModule.Logs;
using FocusSalesModule.Models;
using FocusSalesModule.Queries;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace FocusSalesModule.Controllers
{
    [RoutePrefix("api/sales")]
    public class RmaItemAPIController : ApiController
    {

        [HttpGet]
        [Route("rmaitems")]
        public HashData<Product> GetRmaItem(int compid, int outletid, string rmano)
        {
            HashData<Product> hashData = new HashData<Product>();
            try
            {
                hashData.data = DbCtx<Product>.GetObj(compid, ProductQueries.GetRmaData(rmano, outletid));
                if(hashData.data == null)
                {
                    hashData.result = -1;
                    hashData.message = "Rma number is not valid";
                    return hashData;
                }

                //Check if consumed
                if(hashData.data != null )
                {
                    if (hashData.data.Stock <= 0)
                    {
                        hashData.result = -1;
                        hashData.message = "Rma has already been utilised !!";
                        return hashData;
                    }
                }
                hashData.result = 1;
            }
            catch (Exception ex)
            {
                Logger.writeLog(ex.Message);
                Logger.writeLog(ex.StackTrace);
                hashData.result = -1;
                hashData.message = ex.Message;
            }
            return hashData;
        }
        [HttpGet]
        [Route("salesreturnrma")]
        public HashData<Product> GetSaleReturnRma(int compid, int outletid,int memberid, string rmano)
        {
            HashData<Product> hashData = new HashData<Product>();
            try
            {
                

                //Check if another sales return has been done for this rma
                int rmacount = DbCtx<Int32>.GetObj(compid, ProductQueries.GetSalesReturnCount(rmano));

                if (rmacount > 0)
                {
                    hashData.result = -1;
                    hashData.message = "Another sales return has been done for this rma !! ";
                    return hashData;
                }

                hashData.data = DbCtx<Product>.GetObj(compid, ProductQueries.GetSalesReturnRmaData(rmano, outletid, memberid));

                if (hashData.data == null)
                {
                    hashData.result = -1;
                    hashData.message = "Rma number is not valid or was not found ";
                    return hashData;
                }


                hashData.result = 1;
            }
            catch (Exception ex)
            {
                Logger.writeLog(ex.Message);
                Logger.writeLog(ex.StackTrace);
                hashData.result = -1;
                hashData.message = ex.Message;
            }
            return hashData;
        }
        [HttpGet]
        [Route("rmaitemsnooutlet")]
        public HashData<Product> GetRmaItemNoOutlet(int compid, string stockoutdocno, string rmano)
        {
            HashData<Product> hashData = new HashData<Product>();
            try
            {
                hashData.data = DbCtx<Product>.GetObj(compid, ProductQueries.GetRmaDataStockIn(rmano, stockoutdocno));
                if (hashData.data == null)
                {
                    hashData.result = -1;
                    hashData.message = "Rma number is not valid or has been consumed";
                    return hashData;
                }

                hashData.result = 1;
            }
            catch (Exception ex)
            {
                Logger.writeLog(ex.Message);
                Logger.writeLog(ex.StackTrace);
                hashData.result = -1;
                hashData.message = ex.Message;
            }
            return hashData;
        }
        [HttpGet]
        [Route("rmadata")]
        public HashData<RmaItem> LoadRmaData(int compid, string rmano)
        {
            HashData<RmaItem> hashData = new HashData<RmaItem>();

            try
            {
               RmaItem rmaItem =  DbCtx<RmaItem>.GetObj(compid, ProductQueries.GetRmaItemQry(rmano));
                if(rmaItem != null)
                {
                    hashData.data = rmaItem;
                    hashData.result = 1;
                }
                else
                {
                    hashData.result = -1;
                    hashData.message = "Rma number is not valid or has been consumed";
                }


            }
            catch (Exception ex)
            {
                hashData.result = -1;
                hashData.message = ex.Message;
                Logger.writeLog(ex.Message);
                Logger.writeLog(ex.StackTrace);
            }
            return hashData;
        }
        [HttpGet]
        [Route("brstockinrma")]
        public HashData<Product> GetBrStockIn(int compid, int outletid, string rmano)
        {
            HashData<Product> hashData = new HashData<Product>();
            try
            {
                
                hashData.data = DbCtx<Product>.GetObj(compid, ProductQueries.GetBrStockIn(rmano));
                if (hashData.data == null)
                {
                    hashData.result = -1;
                    hashData.message = "Rma number is not valid";
                    return hashData;
                }
                //Check if already posted
                if (hashData.data.PostedRma != 0)
                {
                    hashData.result = -1;
                    hashData.message = "Rma has already been utilised !!";
                    return hashData;
                }

                //Check outlets

                string outletQry = $"select count(he.ToOutlet) from tCore_HeaderData5380_0 he join tCore_Header_0 h on h.iHeaderId = he.iHeaderId where he.ToOutlet = {outletid}  and h.sVoucherNo = '{hashData.data.DocNo}' ";
                int reccount = DbCtx<Int32>.GetScalar(compid , outletQry);
                if(reccount == 0)
                {
                    hashData.result = -1;
                    hashData.message = "From and To outlets do not match";
                    return hashData;
                }

                //if (hashData.data.Stock <= 0)
                //{
                //    hashData.result = -1;
                //    hashData.message = "Rma has already been utilised !!";
                //    return hashData;
                //}
                
                hashData.result = 1;
            }
            catch (Exception ex)
            {
                Logger.writeLog(ex.Message);
                Logger.writeLog(ex.StackTrace);
                hashData.result = -1;
                hashData.message = ex.Message;
            }
            return hashData;
        }
    }
}
