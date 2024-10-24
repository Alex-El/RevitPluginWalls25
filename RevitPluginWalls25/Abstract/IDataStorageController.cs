using RevitPluginWalls.CommandData;
using RevitPluginWalls.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitPluginWalls.Abstract
{
    internal interface IDataStorageController
    {
        bool TryReadProjectData(out DataStorageModel data);
        void WriteProjectData(DataStorageModel data);
    }
}
