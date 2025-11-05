using CrystalDecisions.CrystalReports.Engine;
using Focus.Common.DataStructs;
using FocusSalesModule.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Web;

namespace FocusSalesModule.Manager
{
    public class ReportManager
    {
        public static ReportDocument POSSale(int compid, int headerid)
        {
            ReportDocument myReportDocument = new ReportDocument();
            string reportname = "PosSale.rpt";
            string reportpath = $@"C:\Reports\{reportname}";
            myReportDocument.Load(reportpath);
            string execqry = $"select * from vwPOSSales where headerid = {headerid}";
            DataTable reportData = DbCtx<Int32>.GetData(compid, execqry);
            myReportDocument.SetDataSource(reportData);


            return myReportDocument;
        }
    }
}