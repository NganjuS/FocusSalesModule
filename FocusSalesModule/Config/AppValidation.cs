using Focus.Common.DataStructs;
using FocusSalesModule.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static FocusSalesModule.Config.AppDefaults;

namespace FocusSalesModule.Config
{
    public class AppValidation
    {
        public static bool IsDefaultAccountsNotSet(List<BillSettlement> billSettlement)
        {
            foreach (var bill in billSettlement)
            {
                if (bill.DefaultBankAccount == 0 || bill.DefaultCashAccount == 0 || bill.DefaultOnlineAccount == 0 || bill.DefaultCreditNoteAccount == 0 || bill.DefaultDiscountAccount == 0  ||bill.DefaultAdvanceReceiptAccount == 0 )
                    return true;

                if (bill.TypeSelect == (Int32)AppDefaults.PaymentTypes.Integration)
                {
                    foreach (var payment in bill.PayList)
                    {
                        if (payment.IsSelected && payment.AccountId == 0)
                            throw new Exception($"Payment account for reference {payment.Reference} is not set !!!");
                    }
                }
            }

            return false;

        }
        public static int GetAccountId(BillSettlement billSettlement)
        {

            switch (billSettlement.TypeSelect)
            {
                case (int)PaymentTypes.Cash:
                    return billSettlement.DefaultCashAccount;
                case (int)PaymentTypes.Bank:
                    return billSettlement.DefaultOnlineAccount;
                case (int)PaymentTypes.Integration:
                    return 0;
                case (int)PaymentTypes.DiscountVoucher:
                    return billSettlement.DefaultDiscountAccount;
                case (int)PaymentTypes.CreditNote:
                    return billSettlement.DefaultCreditNoteAccount;
                case (int)PaymentTypes.AdvanceReceipt:
                    return billSettlement.DefaultAdvanceReceiptAccount;
                default:
                    return billSettlement.DefaultAccount;
            }
        }
        static int GetOnlineAccountsId(BillSettlement billSettlement)
        {

            switch (billSettlement.IntegrationType)
            {
                case (Int32)IntegrationTypes.Moniepoint:
                    return billSettlement.DefaultMoniepointAccount;
                case (Int32)IntegrationTypes.Easybuy:
                    return billSettlement.DefaultEasyBuyAccount;
                case (Int32)IntegrationTypes.WemaBank:
                    return billSettlement.DefaultSentinalAccount;
                default:
                    return billSettlement.DefaultOnlineAccount;

            }
        }
    }
}