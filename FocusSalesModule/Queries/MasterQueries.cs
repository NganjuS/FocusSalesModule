using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FocusSalesModule.Queries
{
    public class MasterQueries
    {
        public static string GetCashCustomerQry()
        {
            return "select top 1 iMasterId from mCore_Account where sName = 'Cash Customer' and bGroup = 0 and iStatus = 0 and iAccountType = 5 ";
        }
        public static string GetMembersQry()
        {
            return "select iMasterId Id , sCode Code, sName Name from mPos_Member where bGroup =0 and iStatus = 0 and imasterid <> 0";
        }
        public static string GetAllowedOutlets(int loginid)
        {

            string filterqry = loginid > 1 ? $" and outl.iMasterId in (SELECT iMasterId FROM mSec_UserMasterRestriction where iUserId = {loginid} and iMasterTypeId = 1100)" : "";  

            return $"select outl.iMasterId Id, outl.sCode Code,outl.sName Name, outldet.sDescription Description, outldet.sAddress1 Address, outldet.sPhone Phone, outldet.sEmailID Email, outldet.DefaultCashAccount,outldet.DefaultBankAccount, outldet.DefaultOnlineAccount , outldet.DefaultCreditNoteAccount, outldet.DefaultDiscountAccount,outldet.DefaultCustomer , outldet.DefaultCostCenter,outldet.DefaultSalesAccount from mPos_Outlet outl join muPos_Outlet outldet on outldet.iMasterId = outl.iMasterId where outl.istatus = 0 and outl.bgroup = 0 and outl.iMasterId <> 0 {filterqry}";
        }
        public static string GetOutlet(int outletid)
        {

            return $"select outl.iMasterId Id, outl.sCode Code,outl.sName Name, outldet.sDescription Description, outldet.sAddress1 Address, outldet.sPhone Phone, outldet.sEmailID Email , outldet.DefaultCashAccount,outldet.DefaultBankAccount, outldet.DefaultOnlineAccount , outldet.DefaultCreditNoteAccount, outldet.DefaultDiscountAccount,outldet.DefaultCustomer , outldet.DefaultCostCenter,outldet.DefaultSalesAccount from mPos_Outlet outl join muPos_Outlet outldet on outldet.iMasterId = outl.iMasterId where outl.istatus = 0 and outl.bgroup = 0 and outl.iMasterId <> 0 and outl.iMasterId={outletid}";
        }
        public static string GetAllowedCostCenters(int loginid)
        {

            string filterqry = loginid > 1 ? $" and iMasterId in (SELECT iMasterId FROM mSec_UserMasterRestriction where iUserId = {loginid} and iMasterTypeId = 5)" : "";

            return $"select iMasterId Id, sCode Code,sName Name from mPos_Outlet where istatus = 0 and bgroup = 0 and iMasterId <> 0 {filterqry}";
        }
        public static string GetAllowedBankAccounts(int loginid)
        {

            string filterqry = loginid > 1 ? $" and iMasterId in (SELECT iMasterId FROM mSec_UserMasterRestriction where iUserId = {loginid} and iMasterTypeId = 5)" : "";

            return $"select iMasterId Id, sCode Code,sName Name from mCore_Account where iAccountType in (1,2) and bGroup = 0 and iStatus =0 ";
        }
    }
}