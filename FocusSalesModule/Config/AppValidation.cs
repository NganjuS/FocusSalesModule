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
                if (bill.DefaultBankAccount == 0 || bill.DefaultCashAccount == 0 || bill.DefaultOnlineAccount == 0 || bill.DefaultCreditNoteAccount == 0 || bill.DefaultDiscountAccount == 0 || bill.DefaultMoniepointAccount == 0 || bill.DefaultEasyBuyAccount == 0 || bill.DefaultSentinalAccount == 0 || bill.DefaultAdvanceReceiptAccount == 0 )
                    return true;
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
                    return billSettlement.DefaultBankAccount;
                case (int)PaymentTypes.Integration:
                    return GetOnlineAccountsId(billSettlement);
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
                case (Int32)IntegrationTypes.Sentinal:
                    return billSettlement.DefaultSentinalAccount;
                default:
                    return billSettlement.DefaultOnlineAccount;

            }
        }
    }
}