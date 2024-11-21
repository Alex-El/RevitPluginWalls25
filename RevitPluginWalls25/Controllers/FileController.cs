using Microsoft.Win32;
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


namespace RevitPluginWalls.Controllers
{
    internal class FileController : IAPIController
    {
        public bool RequestData(CommandDataStorage data, out string result)
        {
            try
            {
                var folderDialog = new OpenFolderDialog
                {
                    DefaultDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
                };

                if (folderDialog.ShowDialog() == true)
                {
                    var folderName = folderDialog.FolderName;
                    string[] files = Directory.GetFiles(folderName);
                    foreach (string file in files)
                    {
                        if (file.Contains("level")) data.Levels = ReadAndParseFile<List<LevelDTOModel>>(folderName, file);
                        if (file.Contains("slab")) data.Slabs = ReadAndParseFile<List<SlabDTOModel>>(folderName, file);
                        if (file.Contains("base")) data.Basements = ReadAndParseFile<List<BasementDTOModel>>(folderName, file);
                        if (file.Contains("wall")) data.Walls = ReadAndParseFile<List<WallDTOModel>>(folderName, file);
                        if (file.Contains("door")) data.Doors = ReadAndParseFile<List<DoorDTOModel>>(folderName, file);
                        if (file.Contains("win")) data.Windows = ReadAndParseFile<List<WindowDTOModel>>(folderName, file);
                    }
                }

                result = "";
                return true;
            }
            catch (Exception ex)
            {
                result = "Load data error: " + ex.Message;
                return false;
            }
        }

        T ReadAndParseFile<T>(string path, string fname) where T : new()
        {
            string fullPath = Path.Combine(path, fname);
            string fileText = File.ReadAllText(fullPath);
            T model = JsonSerializer.Deserialize<T>(fileText);

            return model;
        }
    }
}
