using RevitPluginWalls25.Properties;
using System;
using System.IO;

namespace RevitPluginWalls
{
    internal class Settings
    {
        public string IconPath { get; private set; }
        public string AppDllFullName { get; private set; }

        public string EntryPoint { get; private set; }
        public string RibbonSectionName { get; private set; }
        public string RibbonBtnName { get; private set; }
        public string RibbonBtnNameText { get; private set; }
        public string LocalStoragePath { get; private set; }

        public Settings() 
        {
            EntryPoint = Resources.EntryPoint;
            RibbonSectionName = Resources.RibbonSectionName;
            RibbonBtnName = Resources.RibbonBtnName;
            RibbonBtnNameText = Resources.RibbonBtnNameTips;
            LocalStoragePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "RevitPluginData", "WallPlugin", "2022", "local.json");
#if DEBUG
            IconPath = Path.Combine( Resources.DebugAppPath, Resources.ImageFolderName);
            AppDllFullName = Path.Combine(Resources.DebugAppPath, Resources.AssembleName);
#else       
            IconPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                Resources.ReleaseAppPath, Resources.ImageFolderName);
            AppDllFullName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                Resources.ReleaseAppPath, Resources.AssembleName);
#endif
        }
    }
}
