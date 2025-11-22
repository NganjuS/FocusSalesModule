using FocusSalesModule.Helpers;
using FocusSalesModule.Models;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static System.Net.Mime.MediaTypeNames;

namespace FocusSalesModule.Screens
{
    public class POSSales
    {
        public static Hashtable GetHeader(POSDTO posDTO, Outlet outlet) 
        {
            var product = posDTO.Items.FirstOrDefault();
            var headerDoc = new Hashtable
            {
                { "Date", UtilFuncs.GetDateToInt(posDTO.DocDate) },
                { "SalesAC__Id", outlet.DefaultSalesAccount },
                { "CustomerAC__Id", outlet.DefaultCustomer },
                { "Member__Id",  posDTO.MemberId },
                { "Outlet__Id", posDTO.OutletId },
                { "Cost Center__Id", outlet.DefaultCostCenter },
                { "sNarration",  posDTO.Narration },
               
                { "SaleTimestamp", UtilFuncs.GetDateToInt(posDTO.Timestamp)},

            };
            if (posDTO.Payments != null)
            {
                foreach(var payment in posDTO.Payments)
                {
                    int selectType = Convert.ToInt32(payment.TypeSelect.ToString());
                    string paymentType = StripExtraChar(payment.PaymentType.ToString());
                    if(selectType == 1)
                    {
                        headerDoc[$"{paymentType}Amount"] = payment.Amount.ToString();
                        headerDoc[$"{paymentType}Reference"] = payment.Reference.ToString();

                    }
                    var payList = payment.PayList;
                    if(payment.PayList != null && payment.PayList.Count > 0)
                    {
                        string[] extList = { "", "O", "T", "H", "K" };
                        int i = 0;
                        var pList = JsonConvert.DeserializeObject<List<PaymentFields>>(JsonConvert.SerializeObject(payment.PayList)); 

                        foreach (var pay in pList)
                        {
                           headerDoc[$"{paymentType}Account{extList[i]}"] = pay.AccountId;
                            headerDoc[$"{paymentType}Amount{extList[i]}"] = pay.Amount;
                            headerDoc[$"{paymentType}Reference{extList[i]}"] = pay.Reference;
                            i++;
                        }
                    }
                }
            }

            return headerDoc;

        }
        private static string StripExtraChar(string paymentTypeName)
        {
            return paymentTypeName.Trim().Replace(" ", "").Replace("/", "_"); ;
        }
        public static List<Hashtable> GetLines(List<Product> lines )
        {
            List<Hashtable> lineItems = new List<Hashtable>();
            foreach (var line in lines)
            {
                string rmaNos = String.Join(",", line.RmaNoList);
                var lineTbl = new Hashtable
                {
                   
                    { "Item__Id", line.ItemId },
                    { "Unit__Id", line.UnitId },
                    { "RMA", rmaNos },
                    { "Quantity", line.Qty },
                    { "Rate", line.Price },
                    { "Gross", line.Gross},
                   

                };
                lineItems.Add(lineTbl);
              
            }
            return lineItems;
        }
        public class PaymentFields
        {
            public int? AccountId { get; set; } = 0;
            public decimal Amount { get; set; } = 0;
            public string Reference { get; set; } = String.Empty;

        }
    }
}