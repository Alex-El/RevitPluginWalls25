using RevitPluginWalls.CommandData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitPluginWalls.Abstract
{
    internal interface IAPIController
    {
        bool RequestData(CommandDataStorage data, out string result);
    }
}
