using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FocusSalesModule.Queries
{
    public class MasterQueries
    {
        public static string GetDefault(int loginid)
        {

            string filterqry = loginid > 1 ? $" and f.iMasterId in (SELECT iMasterId FROM mSec_UserMasterRestriction where iUserId = {loginid} and iMasterTypeId = 3012)" : "";
            string groupeditems = loginid > 1 ? $" union select  f.iMasterId  Id,  f.sCode Code, f.sName  Name from mCore_farm f left join mCore_farmTreeDetails tr1 on tr1.iMasterId  =f.iMasterId where tr1.iParentId in (SELECT iMasterId FROM mSec_UserMasterRestriction where iUserId = {loginid} and iMasterTypeId = 3012) " : "";
            return $"select  f.iMasterId  Id,  f.sCode Code, f.sName  Name from mCore_farm f where f.bgroup = 0 and f.istatus =0  and f.imasterid <> 0 {filterqry} {groupeditems}";
        }
    }
}