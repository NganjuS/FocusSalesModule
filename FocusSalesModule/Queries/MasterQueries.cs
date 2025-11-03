using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FocusSalesModule.Queries
{
    public class MasterQueries
    {
        public static string GetAllowedOutlets(int loginid)
        {

            string filterqry = loginid > 1 ? $" and iMasterId in (SELECT iMasterId FROM mSec_UserMasterRestriction where iUserId = {loginid} and iMasterTypeId = 1100)" : "";  

            return $"select iMasterId Id, sCode Code,sName Name from mPos_Outlet where istatus = 0 and bgroup = 0 and iMasterId <> 0 {filterqry}";
        }
    }
}