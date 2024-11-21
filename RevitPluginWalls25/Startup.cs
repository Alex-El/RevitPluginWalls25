using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using NLog;
using RevitPluginWalls.Abstract;
using RevitPluginWalls.BIMCreator;
using RevitPluginWalls.CommandData;
using RevitPluginWalls.Controllers;
using RevitPluginWalls.Models;
using RevitPluginWalls.ViewModels;
using RevitPluginWalls25.Properties;
using System;

namespace RevitPluginWalls
{
    [Transaction(TransactionMode.Manual)]
    public class Startup : IExternalCommand
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                ILocalController localController = new LocalController();
                IDataStorageController storageController = new DataStorageController(commandData);
                //IAPIController apiController = new APIController();
                IAPIController apiController = new FileController();
                IModelBuilder modelBuilder = new ModelBuilder();
                IViewModel viewModel = new PluginViewModel();

                var data = new CommandDataStorage(commandData);
                localController.ReadUserData(data);


                if (storageController.TryReadProjectData(out DataStorageModel dataStorageData))
                {
                    data.IsBuildFast = dataStorageData.CreationMode == 1 ? true : false;
                    data.ProcjectId = dataStorageData.ProjectId;
                }

                viewModel.ShowDialog(data);

                if (data.SaveNewData)
                {
                    localController.WriteUserData(data);
                    storageController.WriteProjectData(new DataStorageModel 
                    { 
                        CreationMode = (data.IsBuildFast ? 1 : 0),
                        ProjectId = data.ProcjectId
                    });
                }

                if (!data.IsBuild) return Result.Succeeded;

                if (apiController.RequestData(data, out string api_result))
                {
                    if(modelBuilder.BuildModel(data, out string build_result))
                        TaskDialog.Show(Resources.ProgramName, Resources.SuccessMessage + "\n - " + build_result);
                    else
                        TaskDialog.Show(Resources.ProgramName, Resources.FailAPIMessage + "\n - " + build_result);
                }
                else
                {
                    TaskDialog.Show(Resources.ProgramName, Resources.FailAPIMessage + "\n - " + api_result);
                }

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Result.Failed;
            }
        }
    }
}
