using FocusSalesModule.Data;
using FocusSalesModule.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FocusSalesModule.Helpers
{
    public class ExecuteCreateTable
    {
        public static void RunQueries(int compid)
        {
            DbCtx<Int32>.ExecuteNonQry(compid, MoneyPointQuery.CreateTransactionsTable());
            DbCtx<Int32>.ExecuteNonQry(compid, PaymentsTableQueries.CreateTemporaryPaymentsQry());
            //DbCtx<Int32>.ExecuteNonQry(compid, PaymentsTableQueries.CreateTemporaryPaymentListQry());
        }
    }
}