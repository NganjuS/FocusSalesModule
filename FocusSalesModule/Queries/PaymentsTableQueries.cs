using FocusSalesModule.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FocusSalesModule.Queries
{
    public class PaymentsTableQueries
    {
        public static string InsertPaymentQry(string docTagId, decimal amount)
        {
            return "";
        }
        public static string InsertPaymentLineQry()
        {
            return $"";
        }
        public static string CreateTemporaryPaymentsQry()
        {
            return "IF OBJECT_ID(N'fsm_TemporaryPayments', N'U') IS NULL create table fsm_TemporaryPayments ( Id int IDENTITY PRIMARY KEY, DocumentTagId varchar(200), ProbableDocNo varchar(50), Vtype int,CustomerId int, MemberId int,OutletId int, CostCenterId int,DocDate int, LoginId int ,IsValidated bit,Amount decimal(20,4), Reference  varchar(150), PaymentMethodId int , PaymentType int , ShowReference  bit,ShowBank bit,SelectedAccount int, TxnDate Datetime, CreatedOn Datetime)";
        }
        public static string CreateTemporaryPaymentListQry()
        {
            return "IF OBJECT_ID(N'fsm_TemporaryPaymentsList', N'U') IS NULL create table fsm_TemporaryPaymentsList (  Id int IDENTITY PRIMARY KEY, TempPaymentId int,  Code varchar(50), Reference varchar(50), Amount decimal(20,4), TxnDate Datetime";
        }
        public static Guid CreateGuidFromStrings(string str1, string str2)
        {
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                var inputBytes = System.Text.Encoding.UTF8.GetBytes($"{str1}_{str2}");
                var hashBytes = md5.ComputeHash(inputBytes);
                return new Guid(hashBytes);
            }
        }
    }
}