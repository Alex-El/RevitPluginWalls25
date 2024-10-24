using RevitPluginWalls.CommandData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitPluginWalls.Abstract
{
    internal interface ILocalController
    {
        void ReadUserData(CommandDataStorage data);
        void WriteUserData(CommandDataStorage data);
    }
}
