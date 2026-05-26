using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Focus.Common.DataStructs;

namespace FocusSalesModule.Logs
{
    public class Logger
    {

        public static void writeLog(string sContent)
        {
            FConvert.LogFile(DateTime.Now.ToString("MMyy") + "focussalesmodule.log", DateTime.Now.ToString() + " : " + sContent);

        }



    }
}