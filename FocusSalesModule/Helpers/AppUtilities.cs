using FocusSalesModule.Data;
using FocusSalesModule.DTO;
using FocusSalesModule.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Web;

namespace FocusSalesModule.Helpers
{
    public class AppUtilities
    {

        public static string GetScreenName(int compid, int vtype)
        {
            string voucherQry = $"select sname from cCore_Vouchers_0 where iVoucherType = {vtype} ";
            return DbCtx<String>.GetObj(compid, voucherQry);

        }
        public enum PaymentTypes { Cash = 1, Bank = 2, Integration = 3, DiscountVoucher = 4, CreditNote = 5 }
        public static string StripExtraChar(string paymentTypeName)
        {
            return paymentTypeName.Trim().Replace(" ", "").Replace("/", "_");
        }
        public static int GetAccountId(int typeSelect, BillSettlement settlement)
        {


            if (typeSelect == (int)PaymentTypes.Bank)
            {
                return settlement.DefaultBankAccount;
            }
            else if (typeSelect == (int)PaymentTypes.Integration)
            {
                return settlement.DefaultOnlineAccount;
            }
            else if (typeSelect == (int)PaymentTypes.DiscountVoucher)
            {
                return settlement.DefaultDiscountAccount;
            }
            else if (typeSelect == (int)PaymentTypes.CreditNote)
            {
                return settlement.DefaultCreditNoteAccount;
            }
            else
            {
                return settlement.DefaultCashAccount;
            }
        }
        
        public static string GetQueryUpdate(int compid, int vtype, List<BillSettlement> billSettlement)
        {
            string fieldlistqry = $"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'tCore_HeaderData{vtype}_0'";
            List<string> fieldList = DbCtx<String>.GetObjList(compid, fieldlistqry);
            string qry = "";
            foreach (var billSettle in billSettlement)
            {

                string strippedName = AppUtilities.StripExtraChar(billSettle.PaymentType);

                if (billSettle.TypeSelect == (Int32)PaymentTypes.Cash)
                {

                    string cashreferencefield = $"{strippedName}Reference";
                    string cashamountfield = $"{strippedName}Amount";
                    string cashaccountfield = $"{strippedName}Account";

                    if (fieldList.Contains(cashreferencefield))
                    {
                        string comma = qry.Length > 0 ? "," : "";
                        qry += $"{comma}{cashreferencefield} = '{billSettle.Reference}'";
                    }
                    if (fieldList.Contains(cashamountfield))
                    {
                        string comma = qry.Length > 0 ? "," : "";
                        qry += $"{comma}{cashamountfield} = '{billSettle.Amount}'";
                    }
                    if (fieldList.Contains(cashaccountfield))
                    {
                        string comma = qry.Length > 0 ? "," : "";
                        qry += $"{comma}{cashaccountfield} = '{AppUtilities.GetAccountId(billSettle.TypeSelect, billSettle)}'";
                    }

                }
                else
                {
                    string[] extList = { "", "O", "T", "H", "K" };
                    string payreferencefield = $"{strippedName}Reference";
                    string paymentrefamountfield = $"{strippedName}Amount";
                    string payaccountfield = $"{strippedName}Account";
                    if (billSettle.PayList.Count == 1)
                    {
                        if (fieldList.Contains(payreferencefield))
                        {
                            string comma = qry.Length > 0 ? "," : "";
                            qry += $"{comma}{payreferencefield} = '{billSettle.Reference}'";
                        }
                        if (fieldList.Contains(paymentrefamountfield))
                        {
                            string comma = qry.Length > 0 ? "," : "";
                            qry += $"{comma}{paymentrefamountfield} = '{billSettle.Amount}'";
                        }
                        if (fieldList.Contains(payaccountfield))
                        {
                            string comma = qry.Length > 0 ? "," : "";
                            qry += $"{comma}{payaccountfield} = '{AppUtilities.GetAccountId(billSettle.TypeSelect, billSettle)}'";
                        }
                    }
                    else
                    {
                        for (int i = 0; i < billSettle.PayList.Count; i++)
                        {
                            string ext = extList[i];
                            string payreffieldExt = $"{strippedName}Reference{ext}";
                            string payamountfieldExt = $"{strippedName}Amount{ext}";
                            string payaccountfieldExt = $"{strippedName}Account{ext}";
                            if (fieldList.Contains(payreffieldExt))
                            {
                                string comma = qry.Length > 0 ? "," : "";
                                qry += $"{comma}{payreffieldExt} = '{billSettle.PayList[i].Reference}'";
                            }
                            if (fieldList.Contains(payamountfieldExt))
                            {
                                string comma = qry.Length > 0 ? "," : "";
                                qry += $"{comma}{payamountfieldExt} = '{billSettle.PayList[i].Amount}'";
                            }
                            //if (fieldList.Contains(payaccountfieldExt))
                            //{
                            //    string comma = qry.Length > 0 ? "," : "";
                            //    qry += $"{comma}{payaccountfieldExt} = '{billSettle.PayList[i].Acc}'";
                            //}
                        }
                    }



                }

            }
            return qry;
        }
        public static string GetQueryUpdate(int compid, int vtype, List<TemporaryPaymentDataDto> billSettlement)
        {
            string fieldlistqry = $"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'tCore_HeaderData{vtype}_0'";
            List<string> fieldList = DbCtx<String>.GetObjList(compid, fieldlistqry);
            string qry = "";
            foreach (var billSettle in billSettlement)
            {
                string pname = DbCtx<string>.GetScalar(compid, $"select sName from mCore_paymenttype where imasterid = {billSettle.PaymentMethodId}");
                string strippedName = AppUtilities.StripExtraChar(pname);

                //if (billSettle.PaymentType == (Int32)PaymentTypes.Cash)
                //{

                    string cashreferencefield = $"{strippedName}Reference";
                    string cashamountfield = $"{strippedName}Amount";
                    string cashaccountfield = $"{strippedName}Account";

                    if (fieldList.Contains(cashreferencefield))
                    {
                        string comma = qry.Length > 0 ? "," : "";
                        qry += $"{comma}{cashreferencefield} = '{billSettle.Reference}'";
                    }
                    if (fieldList.Contains(cashamountfield))
                    {
                        string comma = qry.Length > 0 ? "," : "";
                        qry += $"{comma}{cashamountfield} = '{billSettle.Amount}'";
                    }
                    if (fieldList.Contains(cashaccountfield))
                    {
                        string comma = qry.Length > 0 ? "," : "";
                        qry += $"{comma}{cashaccountfield} = '{billSettle.SelectedAccount}'";
                    }

                //}
                //else
                //{
                //    string[] extList = { "", "O", "T", "H", "K" };
                //    string payreferencefield = $"{strippedName}Reference";
                //    string paymentrefamountfield = $"{strippedName}Amount";
                //    string payaccountfield = $"{strippedName}Account";
                //    if (billSettle.PayList.Count == 1)
                //    {
                //        if (fieldList.Contains(payreferencefield))
                //        {
                //            string comma = qry.Length > 0 ? "," : "";
                //            qry += $"{comma}{payreferencefield} = '{billSettle.Reference}'";
                //        }
                //        if (fieldList.Contains(paymentrefamountfield))
                //        {
                //            string comma = qry.Length > 0 ? "," : "";
                //            qry += $"{comma}{paymentrefamountfield} = '{billSettle.Amount}'";
                //        }
                //        if (fieldList.Contains(payaccountfield))
                //        {
                //            string comma = qry.Length > 0 ? "," : "";
                //            qry += $"{comma}{payaccountfield} = '{AppUtilities.GetAccountId(billSettle.TypeSelect, billSettle)}'";
                //        }
                //    }
                //    else
                //    {
                //        for (int i = 0; i < billSettle.PayList.Count; i++)
                //        {
                //            string ext = extList[i];
                //            string payreffieldExt = $"{strippedName}Reference{ext}";
                //            string payamountfieldExt = $"{strippedName}Amount{ext}";
                //            string payaccountfieldExt = $"{strippedName}Account{ext}";
                //            if (fieldList.Contains(payreffieldExt))
                //            {
                //                string comma = qry.Length > 0 ? "," : "";
                //                qry += $"{comma}{payreffieldExt} = '{billSettle.PayList[i].Reference}'";
                //            }
                //            if (fieldList.Contains(payamountfieldExt))
                //            {
                //                string comma = qry.Length > 0 ? "," : "";
                //                qry += $"{comma}{payamountfieldExt} = '{billSettle.PayList[i].Amount}'";
                //            }
                //            //if (fieldList.Contains(payaccountfieldExt))
                //            //{
                //            //    string comma = qry.Length > 0 ? "," : "";
                //            //    qry += $"{comma}{payaccountfieldExt} = '{billSettle.PayList[i].Acc}'";
                //            //}
                //        }
                //    }



                //}

            }
            return qry;
        }
    }
}