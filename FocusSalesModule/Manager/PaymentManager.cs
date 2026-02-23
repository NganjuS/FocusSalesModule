using FocusSalesModule.Config;
using FocusSalesModule.Data;
using FocusSalesModule.DTO;
using FocusSalesModule.Logs;
using FocusSalesModule.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static FocusSalesModule.Config.AppDefaults;
using static FocusSalesModule.Helpers.AppUtilities;

namespace FocusSalesModule.Manager
{
    public class PaymentManager
    {
        public static string PreProcessAdvancePayment(AdvanceReceiptBeforeSave beforeSaveDto)
        {
            Guid id = Guid.NewGuid();

            string qry = $"select dd.ReferenceNo from tCore_Header_0 h join tCore_Data_0 d on d.iHeaderId = h.iHeaderId left join tCore_Data4611_0 dd on dd.iBodyId = d.iBodyId left join  tCore_Data_Tags_0 tg on tg.iBodyId = d.iBodyId left join mCore_paymenttype py on py.iMasterId = tg.iTag3012 left join muCore_paymenttype pyu on pyu.imasterid = py.imasterid where h.iVoucherType = 4611 and h.sVoucherNo = '{beforeSaveDto.DocNo}'  and pyu.TypeSelect = 3";

            List<string> refList = DbCtx<string>.GetObjList(beforeSaveDto.CompanyId , qry);

            string insertQry = "insert into fsm_TemporaryAdvancePayments (DocumentTagId,DocNo , ReferenceNo ,Vtype,CustomerId) values (@DocumentTagId,@DocNo , @ReferenceNo ,@Vtype,@CustomerId)";

            foreach (var reference in refList.Distinct())
            {
                DbCtx<int>.ExecuteNonQry(beforeSaveDto.CompanyId, insertQry, new
                {
                    DocumentTagId = id,
                    beforeSaveDto.DocNo,
                    beforeSaveDto.Vtype,
                    CustomerId = 0,
                    ReferenceNo = reference
                });
            }
           



            return id.ToString();


        }
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
                            int account = item.TypeSelect == (Int32)PaymentTypes.DiscountVoucher ? pay.AccountId : AppValidation.GetAccountId(item);
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
        
    }
}