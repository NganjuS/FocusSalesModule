using FocusSalesModule.Logs;
using FocusSalesModule.Models;
using FocusSalesModule.Updates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace FocusSalesModule.Controllers
{
    [RoutePrefix("api/updates")]
    public class UpdatesController : ApiController
    {
        [HttpGet]
        [Route("updateapp")]
        public async Task<HashData<String>> UpdateApp(int compid)
        {
            HashData<string> resp = new HashData<string>();
            try
            {
                HashData<string> data = new HashData<string>();
                Migrations.RunMigrations(compid, data);
                resp.result = 1;
                resp.message = data.message;

            }
            catch (Exception ex)
            {
                Logger.writeLog(ex.Message);
                Logger.writeLog(ex.StackTrace);
                resp.result = -1;
                resp.message = ex.Message;

            }
            return resp;
        }
    }
}
