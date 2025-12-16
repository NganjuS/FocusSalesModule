using FocusSalesModule.Data;
using FocusSalesModule.DTO;
using FocusSalesModule.Logs;
using FocusSalesModule.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static FocusSalesModule.Helpers.AppUtilities;

namespace FocusSalesModule.Manager
{
    public class PaymentManager
    {
        public static string PreProcessPayment(PosBeforeSaveDto beforeSaveDto)
        {
            Guid id = Guid.NewGuid();
            foreach (var item in beforeSaveDto.BillSettlement)
            {

               if(item.TypeSelect == (Int32)PaymentTypes.Cash && item.Amount > 0)
                {
                    string insertQry = $"insert into fsm_TemporaryPayments (DocumentTagId , ProbableDocNo , Vtype ,CustomerId , MemberId ,OutletId , CostCenterId ,DocDate, LoginId,IsValidated,Amount, Reference, PaymentMethodId, PaymentType, ShowReference,ShowBank ,SelectedAccount, TxnDate, CreatedOn ) values ('{id}', '{beforeSaveDto.DocNo}', {beforeSaveDto.Vtype},{beforeSaveDto.CustomerId},{beforeSaveDto.MemberId},{beforeSaveDto.OutletId},{beforeSaveDto.CostCenterId},{beforeSaveDto.DocDate},{beforeSaveDto.LoginId}, 0, {item.Amount}, '', {item.iMasterId},{item.TypeSelect},{BoolToIntConv(item.ShowReference)},{BoolToIntConv(item.ShowBank)},{item.DefaultCashAccount},'{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}','{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}') ";
                    DbCtx<int>.ExecuteNonQry(beforeSaveDto.CompId, insertQry);
               }
                if (item.PayList.Count > 0 )
                {
                    foreach(var pay in item.PayList)
                    {
                        if(pay.IsSelected)
                        {
                            int account = item.TypeSelect == (Int32)PaymentTypes.DiscountVoucher ? pay.AccountId : getAccountId(item);
                            string insertQry = $"insert into fsm_TemporaryPayments (DocumentTagId , ProbableDocNo , Vtype ,CustomerId , MemberId ,OutletId , CostCenterId ,DocDate, LoginId,IsValidated,Amount, Reference, PaymentMethodId, PaymentType, ShowReference,ShowBank ,SelectedAccount, TxnDate, CreatedOn ) values ('{id}', '{beforeSaveDto.DocNo}', {beforeSaveDto.Vtype},{beforeSaveDto.CustomerId},{beforeSaveDto.MemberId},{beforeSaveDto.OutletId},{beforeSaveDto.CostCenterId},{beforeSaveDto.DocDate},{beforeSaveDto.LoginId}, 0, {pay.Amount}, '{pay.Reference}', {item.iMasterId}, {item.TypeSelect},{BoolToIntConv(item.ShowReference)}, {BoolToIntConv(item.ShowBank)},{account},'{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}','{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}') ";
                            Logger.writeLog(insertQry);
                            DbCtx<int>.ExecuteNonQry(beforeSaveDto.CompId, insertQry);
                        }
                            
                    }
                    
                }


            }
            return id.ToString();

        }
        static int BoolToIntConv(bool bolVal)
        {
            return bolVal ? 1 : 0;
        }
        static int getAccountId(BillSettlement billSettlement)
        {

            switch (billSettlement.TypeSelect)
            {
                case (int)PaymentTypes.Cash:
                    return billSettlement.DefaultCashAccount;
                case (int)PaymentTypes.Bank:
                    return billSettlement.DefaultBankAccount;
                case (int)PaymentTypes.Integration:
                    return billSettlement.DefaultOnlineAccount;
                case (int)PaymentTypes.DiscountVoucher:
                    return billSettlement.DefaultDiscountAccount; 
                case (int)PaymentTypes.CreditNote:
                    return billSettlement.DefaultCreditNoteAccount;
                default:
                    return billSettlement.DefaultAccount;
            }
        }
    }
}