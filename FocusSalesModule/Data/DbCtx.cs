using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace FocusSalesModule.Data
{
    public class DbCtx<T>
    {
        public static string connstr = "";
        public static void BuildConnectionStr(int compid)
        {

            Focus.DatabaseFactory.DatabaseWrapper.GetDatabase(string.Empty, compid);
            string databasename = Focus.DatabaseFactory.DatabaseWrapper.GetDataBaseName(compid);
            connstr = String.Format(Focus.DatabaseFactory.DatabaseWrapper.strConnectionString, databasename);
        }

        public static List<T> GetObjList(int compid, string query)
        {
            BuildConnectionStr(compid);

            using (var ctx = new SqlConnection(connstr))
            {
                return ctx.Query<T>(query).ToList();
            }
        }
        public static T GetObj(int compid, string query)
        {
            BuildConnectionStr(compid);
            using (var ctx = new SqlConnection(connstr))
            {
                return ctx.Query<T>(query).FirstOrDefault();
            }
        }
        public static int ExecuteNonQry(int compid, string query)
        {
            BuildConnectionStr(compid);
            using (var ctx = new SqlConnection(connstr))
            {
                return ctx.Execute(query);
            }
        }
        public static T GetScalar(int compid, string query)
        {
            BuildConnectionStr(compid);
            using (var ctx = new SqlConnection(connstr))
            {
                return ctx.ExecuteScalar<T>(query);
            }
        }
    }
}
