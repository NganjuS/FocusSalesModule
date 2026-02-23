using FocusSalesModule.Models;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static FocusSalesModule.Config.AppDefaults;
using static FocusSalesModule.Helpers.AppUtilities;
using static FocusSalesModule.Screens.POSSales;


namespace FocusSalesModule.Screens
{
    public class PosReceiptScreen
    {
        public const string screenName = "POS Sales Receipt";
        //1,Cash,2,Bank,3,Integration,4,Discount Voucher,5,Credit Note
        
        public static Hashtable GetReceiptHeader(POSDTO posDTO,Outlet outlet,string docNo)
        {
            return new Hashtable()
            {
                { "Account__Id" , outlet.DefaultCustomer},
                { "Cost Center__Id" , outlet.DefaultCostCenter},
                { "Outlet__Id" , outlet.Id},
                { "Member__Id" , posDTO.MemberId},
                { "POSDocNo" , docNo}
            };
        }
        public static List<Hashtable> GetReceiptLines(POSDTO posDTO , Outlet outlet)
        {
            List<Hashtable> payLines = new List<Hashtable>();
            if (posDTO.Payments != null)
            {
                foreach (var payment in posDTO.Payments)
                {
                    int selectType = Convert.ToInt32(payment.TypeSelect.ToString());
                    int integrationType = Convert.ToInt32(payment.IntegrationType.ToString());

                    int accountId = GetAccountId(selectType, integrationType, outlet);
                    if (selectType == (Int32)PaymentTypes.Cash)
                    {
                        var cashPayment = new Hashtable
                        {
                            { "Account__Id",  accountId },
                            { "Amount" , payment.Amount.ToString()},
                            { "POSPaymentType__Id", selectType },
                            { "ReferenceNo",  payment.Reference}

                        };
                       
                        payLines.Add(cashPayment);                        
                    }
                    else
                    {
                        var payList = payment.PayList;
                        if (payment.PayList != null && payment.PayList.Count > 0)
                        {
                   
                            var pList = JsonConvert.DeserializeObject<List<PaymentFields>>(JsonConvert.SerializeObject(payment.PayList));

                            foreach (var pay in pList)
                            {
                                var bankPayment = new Hashtable
                                {
                                    {"Account__Id",  accountId},
                                    { "Amount" , pay.Amount.ToString()},
                                    {   "POSPaymentType__Id", selectType },
                                    { "ReferenceNo",  pay.Reference}

                                };
                                payLines.Add(bankPayment);
                                
                            }
                        }
                    }
                   
                }
            }
            return payLines;
        
        }
        static int GetAccountId(int typeSelect, int integrationType,Outlet outlet)
        {
            

            if (typeSelect == (int)PaymentTypes.Bank)
            {
                return outlet.DefaultBankAccount;
            }
            else if (typeSelect == (int)PaymentTypes.Integration)
            {
                if (integrationType == (int)IntegrationTypes.Moniepoint)
                {
                    return outlet.DefaultMoniepointAccount;
                }
                else if (integrationType == (int)IntegrationTypes.Easybuy)
                {
                    return outlet.DefaultEasyBuyAccount;
                }
                else if (integrationType == (int)IntegrationTypes.Sentinal)
                {
                    return outlet.DefaultSentinalAccount;
                }
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
            else if (typeSelect == (int)PaymentTypes.AdvanceReceipt)
            {
                return outlet.DefaultAdvanceReceiptAccount;
            }
            else
            {
                return outlet.DefaultCashAccount;
            }
        }
    }
}