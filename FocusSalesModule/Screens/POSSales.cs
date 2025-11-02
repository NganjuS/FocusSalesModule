using FocusSalesModule.Helpers;
using FocusSalesModule.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FocusSalesModule.Screens
{
    public class POSSales
    {
        public static Hashtable GetHeader(POSDto posDTO) 
        {
            var product = posDTO.Items.FirstOrDefault();
            var headerDoc = new Hashtable
            {
                { "SalesAC__Id", product.SalesAccId },
                { "CustomerAC__Id", product.CustAccId },
                { "Member__Id",  product.MemberId },
                { "Outlet__Id", product.OutletId },
                { "Cost Center__Id", product.CostCenterId },
                { "sNarration",  product.Narration },
                { "CashReceived", posDTO.Payments.Cash },
                { "SaleTimestamp", UtilFuncs.GetDateToInt(posDTO.Timestamp)},

            };
            //Set Bank Payments
            string[] extList = { "" ,"O","T" ,"H" };
            if(posDTO.Payments.BankPayments != null)
            {
                for (int i = 0; i < posDTO.Payments.BankPayments.Count; i++)
                {
                    string ext = extList[i];
                    headerDoc[$"BankAccount{ext}__Id"] = 0;
                    headerDoc[$"BankPaymentMethod{ext}"] = posDTO.Payments.BankPayments[i].Method;
                    headerDoc[$"ReferenceNo{ext}"] = posDTO.Payments.BankPayments[i].Reference;
                    headerDoc[$"Amount{ext}"] = posDTO.Payments.BankPayments[i].Amount;
                }
            }
            
            //Set Monie
            if(posDTO.Payments.Monie != null)
            {
                for (int i = 0; i < posDTO.Payments.Monie.Count; i++)
                {
                    string ext = extList[i];
                    headerDoc[$"MonieBranchNo{ext}"] = posDTO.Payments.Monie[i].Code;
                    headerDoc[$"MonieRefNo{ext}"] = posDTO.Payments.Monie[i].Reference;
                    headerDoc[$"MonieAmount{ext}"] = posDTO.Payments.Monie[i].Amount;
                }
            }
            
            //Set Discount Voucher
            if(posDTO.Payments.DiscountVouchers != null)
            {

                for (int i = 0; i < posDTO.Payments.DiscountVouchers.Count; i++)
                {

                    string ext = extList[i];
                    headerDoc[$"VoucherCode{ext}"] = posDTO.Payments.DiscountVouchers[i].ItemCode;
                    headerDoc[$"VoucherAmount{ext}"] = posDTO.Payments.DiscountVouchers[i].VoucherValue;
                }

            }
            

            return headerDoc;

        }
        public static List<Hashtable> GetLines(List<Product> lines )
        {
            List<Hashtable> lineItems = new List<Hashtable>();
            foreach (var line in lines)
            {
                string rmaNos = String.Join(",", line.RmaNoList);
                var lineTbl = new Hashtable
                {
                   
                    { "Item__Id", line.Id },
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
    }
}