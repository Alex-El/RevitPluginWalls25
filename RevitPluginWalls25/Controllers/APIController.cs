using Microsoft.Win32;
using RevitPluginWalls.Abstract;
using RevitPluginWalls.CommandData;
using RevitPluginWalls.Models;
using System.IO;
using System.Text.Json;


namespace RevitPluginWalls.Controllers
{
    internal class APIController : IAPIController
    {
        public bool RequestData(CommandDataStorage data, out string result)
        {
            try
            {
                data.Levels = ReadAndParseFile<List<LevelDTOModel>>();
                data.Walls = ReadAndParseFile<List<WallDTOModel>>();

                result = "";
                return true;
            }
            catch (Exception ex)
            {
                result = "Load data error: " + ex.Message;
                return false;
            }
        }

        T ReadAndParseFile<T>() where T : new()
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == true)
            {
                string fileText = File.ReadAllText(openFileDialog.FileName);
                T model = JsonSerializer.Deserialize<T>(fileText);

                return model;
            }
            return new T();
        }
    }
}
