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
            EntryPoint = Properties.Settings.Default.EntryPoint;
            RibbonSectionName = Properties.Settings.Default.RibbonSectionName;
            RibbonBtnName = Properties.Settings.Default.RibbonBtnName;
            RibbonBtnNameText = Properties.Settings.Default.RibbonBtnNameTips;
            LocalStoragePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "RevitPluginData", "WallPlugin", "2022", "local.json");
#if DEBUG
            IconPath = Path.Combine( Properties.Settings.Default.DebugAppPath,
                Properties.Settings.Default.ImageFolderName);
            AppDllFullName = Path.Combine(Properties.Settings.Default.DebugAppPath,
                Properties.Settings.Default.AssembleName);
#else       
            IconPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                Properties.Settings.Default.ReleaseAppPath,
                Properties.Settings.Default.ImageFolderName);
            AppDllFullName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                Properties.Settings.Default.ReleaseAppPath,
                Properties.Settings.Default.AssembleName);
#endif
        }
    }
}
