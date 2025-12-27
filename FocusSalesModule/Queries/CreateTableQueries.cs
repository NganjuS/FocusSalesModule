using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FocusSalesModule.Queries
{
    public class CreateTableQueries
    {
        public static string GetFieldMapping()
        {
            return "IF OBJECT_ID(N'fsm_VoucherFieldsMap', N'U') IS NULL create table fsm_VoucherFieldsMap ( Id int IDENTITY PRIMARY KEY, vtype int, fieldid int, fieldtype int)";
        }
    }
}