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
        public static Hashtable GetHeader(POSDTO posDTO) 
        {
            var product = posDTO.Items.FirstOrDefault();
            var headerDoc = new Hashtable
            {
                { "Date", UtilFuncs.GetDateToInt(posDTO.DocDate) },
                { "SalesAC__Id", posDTO.SalesAccId },
                { "CustomerAC__Id", posDTO.CustAccId },
                { "Member__Id",  posDTO.MemberId },
                { "Outlet__Id", posDTO.OutletId },
                { "Cost Center__Id", posDTO.CostCenterId },
                { "sNarration",  posDTO.Narration },
               
                { "SaleTimestamp", UtilFuncs.GetDateToInt(posDTO.Timestamp)},

            };
            if (posDTO.Payments != null)
            {
                headerDoc["CashReceived"] = posDTO.Payments.CashAmount;
                //Set Bank Payments
                string[] extList = { "", "O", "T", "H" };
                if (posDTO.Payments.BankPayments != null)
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
                if (posDTO.Payments.MoniePayments != null)
                {
                    for (int i = 0; i < posDTO.Payments.MoniePayments.Count; i++)
                    {
                        string ext = extList[i];
                        headerDoc[$"MonieBranchNo{ext}"] = posDTO.Payments.MoniePayments[i].Code;
                        headerDoc[$"MonieRefNo{ext}"] = posDTO.Payments.MoniePayments[i].Reference;
                        headerDoc[$"MonieAmount{ext}"] = posDTO.Payments.MoniePayments[i].Amount;
                    }
                }

                //Set Discount Voucher
                if (posDTO.Payments.DiscountVouchers != null)
                {

                    for (int i = 0; i < posDTO.Payments.DiscountVouchers.Count; i++)
                    {

                        string ext = extList[i];
                        headerDoc[$"VoucherCode{ext}"] = posDTO.Payments.DiscountVouchers[i].ItemCode;
                        headerDoc[$"VoucherAmount{ext}"] = posDTO.Payments.DiscountVouchers[i].VoucherValue;
                    }

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
    }
}