using RevitPluginWalls.CommandData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitPluginWalls.Abstract
{
    internal interface IModelBuilder
    {
        bool BuildModel(CommandDataStorage data, out string result);
    }
}
