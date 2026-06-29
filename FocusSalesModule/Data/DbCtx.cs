using Dapper;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FocusSalesModule.Data
{
    public class DbCtx<T>
    {
        private static readonly ConcurrentDictionary<int, string> _connCache = new ConcurrentDictionary<int, string>();
        public static string GetConnectionStr(int compid)
        {
            return _connCache.GetOrAdd(compid, id =>
            {
                Focus.DatabaseFactory.DatabaseWrapper.GetDatabase(string.Empty, compid);
                string databasename = Focus.DatabaseFactory.DatabaseWrapper.GetDataBaseName(compid);
                return String.Format(Focus.DatabaseFactory.DatabaseWrapper.strConnectionString, databasename);
            });
        }

        public static List<T> GetObjList(int compid, string query)
        {
            ;

            using (var ctx = new SqlConnection(GetConnectionStr(compid)))
            {
                return ctx.Query<T>(query).ToList();
            }
        }
        public static T GetObj(int compid, string query)
        {

            using (var ctx = new SqlConnection(GetConnectionStr(compid)))
            {
                return ctx.Query<T>(query).FirstOrDefault();
            }
        }
        public static int ExecuteNonQry(int compid, string query)
        {

            using (var ctx = new SqlConnection(GetConnectionStr(compid)))
            {
                return ctx.Execute(query);
            }
        }
        public static int ExecuteNonQry(int compid, string query, Object obj)
        {
            using (var ctx = new SqlConnection(GetConnectionStr(compid)))
            {
                return ctx.Execute(query, obj);
            }
        }
        public static int ExecuteNonWithIdQry(int compid, string query, Object obj)
        {
            using (var ctx = new SqlConnection(GetConnectionStr(compid)))
            {
                string fullQry = $"{query}; SELECT CAST(SCOPE_IDENTITY() AS int);";
                return ctx.QuerySingle<int>(fullQry, obj);
            }
        }
        public static T GetScalar(int compid, string query)
        {

            using (var ctx = new SqlConnection(GetConnectionStr(compid)))
            {
                return ctx.ExecuteScalar<T>(query);
            }
        }
        public static T GetScalar(int compid, string query, Object obj)
        {

            using (var ctx = new SqlConnection(GetConnectionStr(compid)))
            {
                return ctx.ExecuteScalar<T>(query, obj);
            }
        }
    }
}
