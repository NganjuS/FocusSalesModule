using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FocusSalesModule.Helpers
{
    public class GeneralUtils
    {
        public static string BuildQueryParams(string orderby, int pageno, int pagesize)
        {

            int startingrecord = (pageno * pagesize) - pagesize;
            return $" order by {orderby}  OFFSET {startingrecord} ROWS FETCH NEXT {pagesize} ROWS ONLY ";
        }
    }
}