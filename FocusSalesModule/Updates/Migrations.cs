using DbUp;
using FocusSalesModule.Data;
using FocusSalesModule.Logs;
using FocusSalesModule.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FocusSalesModule.Updates
{
    public class Migrations
    {
        public static void RunMigrations<T>(int compid, HashData<T> hashData)
        {

            string connectionString = DbCtx<Int32>.GetConnectionStr(compid) + ";TrustServerCertificate = true";
            var dbUpgradeEngine = DeployChanges.To
                .SqlDatabase(connectionString).JournalToSqlTable("dbo", "fsm_VersioningTable").WithScriptsEmbeddedInAssembly(typeof(Migrations).Assembly).LogTo(new SerilogUpgradeLog())
                .WithTransaction().Build();

            var scripts = dbUpgradeEngine.GetScriptsToExecute();
            Logger.writeLog("Scripts to run:");
            foreach (var script in scripts)
                Logger.writeLog(script.Name);

            if (dbUpgradeEngine.IsUpgradeRequired())
            {
                Logger.writeLog("Upgrades have been detected. Upgrading database now...");
                var operation = dbUpgradeEngine.PerformUpgrade();
                if (operation.Successful)
                {

                    hashData.result = 1;
                    hashData.message = "Upgrade completed successfully";
                    Logger.writeLog(hashData.message);


                }
                else
                {
                    hashData.result = -1;
                    hashData.message = "Error happened in the upgrade. Please check the logs";
                    Logger.writeLog(hashData.message);
                }

            }
            else
            {
                hashData.result = 1;
                hashData.message = "No pending updates";
            }

        }
        public bool IsPendingMigrations(int compid)
        {
            string connectionString = DbCtx<Int32>.GetConnectionStr(compid);
            var dbUpgradeEngineBuilder = DeployChanges.To
                .SqlDatabase(connectionString)
                .WithScriptsEmbeddedInAssembly(typeof(Migrations).Assembly)
                .WithTransaction()
                .LogToConsole();

            var dbUpgradeEngine = dbUpgradeEngineBuilder.Build();
            return dbUpgradeEngine.IsUpgradeRequired();

        }
    }
}