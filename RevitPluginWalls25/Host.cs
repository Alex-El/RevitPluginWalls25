using Autodesk.Revit.UI;
using RevitPluginWalls.Controllers;

namespace RevitPluginWalls
{
    internal static class Host
    {
        public static Settings Settings { get; private set; }
        public static RibbonController RibbonController { get; private set; }

        internal static void Initialize(UIControlledApplication application)
        {
            Settings = new Settings();
            RibbonController = new RibbonController(application);
        }
    }
}
