using Focus.Common.DataStructs;
using FocusSalesModule.Data;
using FocusSalesModule.DTO;
using FocusSalesModule.Helpers;
using FocusSalesModule.Models;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using static FocusSalesModule.Helpers.AppUtilities;
using static FocusSalesModule.Screens.POSSales;

namespace FocusSalesModule.Screens
{
    public class PosReceiptScreenMain
    {
        public const string screenName = "FBI POS Receipt";
        //1,Cash,2,Bank,3,Integration,4,Discount Voucher,5,Credit Note
        
        public static bool IsReceiptPosted(int compid, string docno)
        {
            int vtype = GetReceiptVtype(compid);
            return DbCtx<int>.GetObj(compid, $"select count(*) from tCore_HeaderData{vtype}_0 where posdocno = '{docno}'") > 0;

        }
        public static bool IsDefaultAccountsNotSet(List<BillSettlement> billSettlement)
        {
            foreach (var bill in billSettlement)
            {
                if(bill.DefaultBankAccount == 0 || bill.DefaultCashAccount == 0 || bill.DefaultOnlineAccount == 0 || bill.DefaultCreditNoteAccount == 0 || bill.DefaultDiscountAccount == 0 || bill.DefaultMoniepointAccount == 0 || bill.DefaultEasyBuyAccount == 0 || bill.DefaultSentinalAccount == 0)
                return true;
            }
            return false;
            
        }
       
        public static int GetReceiptVtype(int compid)
        {
            return DbCtx<int>.GetObj(compid, $"select iVoucherType from cCore_Vouchers_0 where sName = '{screenName}'");
        }
        public static Hashtable BuildReceiptHeader(Hashtable docheader)
        {
            return new Hashtable()
            {
                { "Account__Id" , docheader["CustomerAC__Id"].ToString()},
                { "Cost Center__Id" , docheader["Cost Center__Id"].ToString()},
                { "Outlet__Id" , docheader["Outlet__Id"].ToString()},
                { "Member__Id" , docheader["Member__Id"].ToString()},
                { "POSDocNo" , docheader["DocNo"].ToString()}
            };
        }
        public static List<Hashtable> BuildReceiptLines(List<Hashtable> datalist, List<BillSettlement> billSettlements)
        {
            List<Hashtable> payLines = new List<Hashtable>();

            foreach (var payment in billSettlements)
            {


                int accountId = AppUtilities.GetAccountId(payment.TypeSelect, payment);
                if (payment.TypeSelect == (Int32)PaymentTypes.Cash)
                {
                    var cashPayment = new Hashtable
                        {
                            {"Account__Id",  accountId },
                            { "Amount" , payment.Amount.ToString()},
                            { "Payment Type__Id", payment.iMasterId },
                            { "ReferenceNo",  payment.Reference}

                        };

                    payLines.Add(cashPayment);
                }
                else
                {

                    if (payment.PayList.Count > 0)
                    {

                        foreach (var pay in payment.PayList)
                        {
                            var bankPayment = new Hashtable
                                {
                                    {"Account__Id",  accountId } ,
                                    { "Amount" , pay.Amount.ToString() },
                                    { "Payment Type__Id", payment.iMasterId },
                                    { "ReferenceNo",  pay.Reference}

                                };
                            payLines.Add(bankPayment);

                        }
                    }
                }

            }

            return payLines;

        }
        public static List<Hashtable> PaymentBuildReceiptLines(List<Hashtable> datalist, List<TemporaryPaymentDataDto> billSettlements)
        {
            List<Hashtable> payLines = new List<Hashtable>();
            
                foreach (var payment in billSettlements)
                {
                                  
                        var cashPayment = new Hashtable
                        {
                            {"Account__Id",  payment.SelectedAccount },
                            { "Amount" , payment.Amount.ToString()},
                            { "Payment Type__Id", payment.PaymentMethodId },
                            { "ReferenceNo",  payment.Reference}

                        };
                       
                        payLines.Add(cashPayment);                        
                  
                }
            
            return payLines;
        
        }
        static int GetAccountId(int typeSelect, Outlet outlet)
        {
            

            if (typeSelect == (int)PaymentTypes.Bank)
            {
                return outlet.DefaultBankAccount;
            }
            else if (typeSelect == (int)PaymentTypes.Integration)
            {
                return outlet.DefaultOnlineAccount;
            }
            else if (typeSelect == (int)PaymentTypes.DiscountVoucher)
            {
                return outlet.DefaultDiscountAccount;
            }
            else if(typeSelect == (int)PaymentTypes.CreditNote)
            {
                return outlet.DefaultCreditNoteAccount;
            }
            else
            {
                return outlet.DefaultCashAccount;
            }
        }
    }
}