﻿using Autodesk.Revit.UI;
using RevitPluginWalls.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitPluginWalls.CommandData
{
    public class CommandDataStorage
    {
        private ExternalCommandData _commandData;

        public CommandDataStorage(ExternalCommandData commandData)
        {
            _commandData = commandData;
        }

        public ExternalCommandData ExternalCommandData { get { return _commandData; } }

        public bool SaveNewData { get; internal set; } = false;
        public bool IsBuild { get; internal set; } = false;
        public string Login { get; internal set; } = "";
        public string Password { get; internal set; } = "";
        public string Url { get; internal set; } = "";
        public string ProcjectId { get; internal set; } = "";
        public bool IsBuildFast { get; internal set; } = false;

        internal List<LevelDTOModel> Levels { get; set; } = new List<LevelDTOModel>();
        internal List<SlabDTOModel> Slabs { get; set; } = new List<SlabDTOModel>();
        internal List<BasementDTOModel> Basements { get; set; } = new List<BasementDTOModel>();
        internal List<WallDTOModel> Walls { get; set; } = new List<WallDTOModel>();
        internal List<DoorDTOModel> Doors { get; set; } = new List<DoorDTOModel>();
        internal List<WindowDTOModel> Windows { get; set; } = new List<WindowDTOModel>();

        public string DataToString()
        {
            string result = string.Empty;

            result += $"SaveNewData = {SaveNewData}; ";
            result += $"IsBuild = {IsBuild}; ";
            result += $"Url = {Url}; ";
            result += $"Login = {Login}; ";
            result += $"Password = {Password}; ";
            result += $"IsBuildFast = {IsBuildFast}; ";

            return result;
        }
    }
}
