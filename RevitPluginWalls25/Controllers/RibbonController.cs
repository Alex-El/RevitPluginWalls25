using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace RevitPluginWalls.Controllers
{
    internal class RibbonController
    {
        public RibbonController(UIControlledApplication application) 
        {
            string iconFullName32 = Path.Combine(Host.Settings.IconPath, "WallsBtn32.png");
            string iconFullName16 = Path.Combine(Host.Settings.IconPath, "WallsBtn16.png");
            string dllFullName = Host.Settings.AppDllFullName;
            string entryPoint = Host.Settings.EntryPoint;
            string paneName = Host.Settings.RibbonSectionName;
            string btnName = Host.Settings.RibbonBtnName;
            string btnText = Host.Settings.RibbonBtnNameText;

            RibbonPanel addinPanel = application.CreateRibbonPanel(paneName);
            PushButton startButton = addinPanel.AddItem(new PushButtonData("startButton", btnName, dllFullName, entryPoint)) as PushButton;
            startButton.ToolTip = btnText;
            startButton.LargeImage = new BitmapImage(new Uri(iconFullName32, UriKind.RelativeOrAbsolute));
            startButton.Image = new BitmapImage(new Uri(iconFullName16, UriKind.RelativeOrAbsolute));
        }
    }
}
