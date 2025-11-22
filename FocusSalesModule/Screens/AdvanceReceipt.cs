using FocusSalesModule.Helpers;
using FocusSalesModule.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FocusSalesModule.Screens
{
    public class AdvanceReceipt
    {
        public static Hashtable GetReceiptHeader(AdvancedPaymentDTO advancedPayment,Outlet outlet)
        {
            return new Hashtable()
            {
                { "Date", UtilFuncs.GetDateToInt(advancedPayment.DocDate) },
                { "CashBankAC__Id" , outlet.DefaultCashAccount},
                { "Cost Center__Id" , outlet.DefaultCostCenter},
                { "Outlet__Id" , outlet.Id},
                { "Member__Id" , advancedPayment.MemberId},
                { "sChequeNo" , advancedPayment.ReferenceNo}
            };
        }
        public static List<Hashtable> GetReceiptLines(AdvancedPaymentDTO advancedPayment, Outlet outlet)
        {
            List<Hashtable> lines = new List<Hashtable>();
            lines.Add(new Hashtable() {
                { "Account__Id", outlet.DefaultCustomer },
                 {   "Amount",  advancedPayment.Amount},
                 {   "Reference",advancedPayment.ReferenceNo },
                  {  "sRemarks", "" },

            });
            return lines;

        }
    }
}