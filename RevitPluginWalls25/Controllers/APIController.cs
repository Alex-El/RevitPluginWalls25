using RevitPluginWalls.Abstract;
using RevitPluginWalls.CommandData;
using RevitPluginWalls.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                openFileDialog.RestoreDirectory = true;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string fileText = File.ReadAllText(openFileDialog.FileName);
                    T model = JsonSerializer.Deserialize<T>(fileText);

                    return model;
                }
            }
            return new T();
        }
    }
}
