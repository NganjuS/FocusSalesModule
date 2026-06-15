using DbUp.Engine.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FocusSalesModule.Logs
{
    public class SerilogUpgradeLog : IUpgradeLog
    {

        public void LogDebug(string format, params object[] args)
        {
            Serilog.Log.Debug(format, args);
        }

        public void LogError(string format, params object[] args)
        {
            Serilog.Log.Error(format, args);
        }

        public void LogError(Exception ex, string format, params object[] args)
        {
            Serilog.Log.Error(ex, format, args);
        }

        public void LogInformation(string format, params object[] args)
        {
            Serilog.Log.Information(format, args);
        }

        public void LogTrace(string format, params object[] args)
        {
            Serilog.Log.Information(format, args);
        }

        public void LogWarning(string format, params object[] args)
        {
            Serilog.Log.Warning(format, args);
        }
    }
}