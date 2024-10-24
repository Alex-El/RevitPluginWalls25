using RevitPluginWalls.Abstract;
using RevitPluginWalls.CommandData;
using RevitPluginWalls.Models;
using System.IO;
using System.Text.Json;


namespace RevitPluginWalls.Controllers
{
    internal class LocalController : ILocalController
    {
        private readonly string _fullpath = Host.Settings.LocalStoragePath;
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public void ReadUserData(CommandDataStorage data)
        {
            try
            {
                if (File.Exists(_fullpath))
                {
                    string filetxt = File.ReadAllText(_fullpath);

                    LocalDataModel localData = null;
                    try
                    {
                        localData = JsonSerializer.Deserialize<LocalDataModel>(filetxt);
                        if (localData == null) throw new Exception(": localData == null");
                        data.Login = localData.Login;
                        data.Password = localData.Password;
                        data.Url = localData.Url;
                    }
                    catch (Exception ex)
                    {
                        File.Delete(_fullpath);
                        Logger.Error(ex, "-> локальный файл удален");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        public void WriteUserData(CommandDataStorage data)
        {
            try
            {
                if (File.Exists(_fullpath)) File.Delete(_fullpath);

                FileInfo fileInfo = new FileInfo(_fullpath);
                DirectoryInfo dir = new DirectoryInfo(fileInfo.DirectoryName);
                if (!dir.Exists) dir.Create();

                LocalDataModel ld = new LocalDataModel
                {
                    Login = data.Login,
                    Password = data.Password,
                    Url = data.Url
                };
                string filetxt = JsonSerializer.Serialize(ld);

                File.WriteAllText(_fullpath, filetxt);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }
    }
}
