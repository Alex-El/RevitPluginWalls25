using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using NLog;
using System;
using System.IO;

namespace RevitPluginWalls
{
    [Transaction(TransactionMode.Manual)]
    internal class Application : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication application)
        {
            string logpath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "RevitPluginLogs", "WallPlugin", "2025", "log.txt");

            NLog.LogManager.Setup().LoadConfiguration(builder => {
                builder.ForLogger().FilterMinLevel(LogLevel.Debug).WriteToFile(fileName: logpath);
            });

            NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

            try
            {
                Host.Initialize(application);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Result.Failed;
            }

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
    }
}
