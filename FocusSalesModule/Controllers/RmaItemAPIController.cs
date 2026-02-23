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
        public HashData<dynamic> GetSaleReturnRma(int compid, int vtype,int outletid,int memberid, string rmano, string posdocno)
        {
            HashData<dynamic> hashData = new HashData<dynamic>();
            try
            {
                

                hashData.data = DbCtx<dynamic>.GetObj(compid, ProductQueries.GetSalesReturnRmaData(rmano));

                if (hashData.data == null)
                {
                    hashData.result = -1;
                    hashData.message = "Rma is invalid or has already been returned ";
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
        [Route("loadsalesdata")]
        public HashData<dynamic> LoadSalesData(int compid, int vtype, string docnosearch)
        {
            HashData<dynamic> hashData = new HashData<dynamic>();
            try
            {


                hashData.datalist = DbCtx<dynamic>.GetObjList(compid, ProductQueries.GetSalesReturnSaleByDocNo(docnosearch));

                if (hashData.datalist.Count == 0)
                {
                    hashData.result = -1;
                    hashData.message = "Document has already been returned or  Number is invalid !!";
                    return hashData;
                }

                foreach(var line in hashData.datalist)
                {
                    int bodyid = line.BodyId;
                    string docNo = line.DocNo;
                    // string rmaQry = $"select sRmaNo from tCore_Rma_0 where iBodyId = {bodyid}";
                    string rmaQry = $"select rmamain.sRmaNo from tCore_Rma_0 rmamain where iBodyId = {bodyid} and not exists(select rh.iHeaderId from tCore_Header_0 rh join tCore_Data_0 rd on rd.iHeaderId = rh.iHeaderId left join tCore_HeaderData1793_0  rhe on rhe.iheaderid = rh.iheaderid left join tCore_Rma_0 rrma on rrma.iBodyId = rd.iBodyId where rh.iVoucherType = 1793 and rhe.POSDocNo = '{docNo}' and rrma.sRmaNo = rmamain.sRmaNo)";
                    List<string> rmaList = DbCtx<string>.GetObjList(compid, rmaQry);
                    line.RmaNo = string.Join(",", rmaList);
                    line.Qty = rmaList.Count;
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
        public HashData<Product> GetBrStockIn(int compid, int outletid, string rmano, string stkoutdocno)
        {
            HashData<Product> hashData = new HashData<Product>();
            try
            {
                List<Product> products = DbCtx<Product>.GetObjList(compid, ProductQueries.GetBrStockIn(rmano, stkoutdocno));
       
                if (products.Count == 0)
                {
                    hashData.result = -1;
                    hashData.message = "Rma was not found";
                    return hashData;
                }

                //Check if already posted
                if (products[0].PostedRma != 0)
                {
                    hashData.result = -1;
                    hashData.message = "Rma has already been utilised !!";
                    return hashData;
                }

                //Check outlets

                string outletQry = $"select count(he.ToOutlet) from tCore_HeaderData5380_0 he join tCore_Header_0 h on h.iHeaderId = he.iHeaderId where he.ToOutlet = {outletid}  and h.sVoucherNo = '{products[0].DocNo}' ";
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
                hashData.data = products[0];
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
        [Route("stocktransferinrma")]
        public HashData<Product> StockTansferIn(int compid, int outletid, string rmano, string stkoutdocno)
        {
            HashData<Product> hashData = new HashData<Product>();
            try
            {
                stkoutdocno = stkoutdocno.Split(':')[1].ToString();
                List<Product> products = DbCtx<Product>.GetObjList(compid, ProductQueries.GetStockTransferIn(rmano, stkoutdocno));

                if (products.Count == 0)
                {
                    hashData.result = -1;
                    hashData.message = "Rma was not found";
                    return hashData;
                }

                //Check if already posted
                if (products[0].PostedRma != 0)
                {
                    hashData.result = -1;
                    hashData.message = "Rma has already been utilised !!";
                    return hashData;
                }

                //Check outlets

                string outletQry = $"select count(tg.iTag3015) from tCore_Header_0 h join tCore_HeaderData3073_0 he on h.iHeaderId = he.iHeaderId join tCore_Data_0 d on d.iHeaderId = h.iHeaderId join  tCore_Data_Tags_0 tg on tg.iBodyId =  d.iBodyId where tg.iTag3015 = {outletid} and   h.sVoucherNo = '{stkoutdocno}'  ";
                int reccount = DbCtx<Int32>.GetScalar(compid, outletQry);
                if (reccount == 0)
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
                hashData.data = products[0];
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
