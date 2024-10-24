using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitPluginWalls.BIMCreator
{
    internal static class Creator
    {
        public static Level CreateLevel(Document doc, double elevation, string name)
        {
            Level level = Level.Create(doc, elevation);
            if (null == level)
            {
                throw new Exception("Create a new level failed.");
            }

            level.Name = name;
            return level;
        }

        public static Wall CreateWall(Document document, Level level_down, Level level_up, XYZ start, XYZ end, WallType type)
        {
            Line geomLine = Line.CreateBound(start, end);
            // Create a wall using the location line
            Wall wall = Wall.Create(document, geomLine, type.Id, level_down.Id, 1.0, 0.0, false, true);
            wall.get_Parameter(BuiltInParameter.WALL_HEIGHT_TYPE).Set(level_up.Id);

            return wall;
        }


    }
}
