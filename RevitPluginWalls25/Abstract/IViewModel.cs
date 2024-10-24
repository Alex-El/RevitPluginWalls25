using RevitPluginWalls.CommandData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitPluginWalls.Abstract
{
    public interface IViewModel
    {
        void ShowDialog(CommandDataStorage data);
    }
}
