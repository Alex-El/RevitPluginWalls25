using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml.Linq;

namespace RevitPluginWalls.BIMCreator
{
    internal static class Creator
    {
        public static Level CreateLevel(Document document, double elevation, string name, bool inTransaction, out string error)
        {
            Level level = null;
            error = "";

            if (inTransaction)
            {
                using (Transaction tr = new Transaction(document, "Create level"))
                {
                    try
                    {
                        level = Level.Create(document, elevation);
                        level.Name = name;
                    }
                    catch (Exception ex)
                    {
                        tr.RollBack();
                        error = ex.Message;
                    }
                }
            }
            else
            {
                level = Level.Create(document, elevation);
                level.Name = name;
            }
            
            return level;
        }

        public static Wall CreateWall(Document document, Level level_down, Level level_up, XYZ start, XYZ end, WallType type, bool inTransaction, out string error)
        {
            Wall wall = null;
            error = "";

            if (inTransaction)
            {
                using (Transaction tr = new Transaction(document, "Create wall"))
                {
                    try
                    {
                        Line geomLine = Line.CreateBound(start, end);
                        // Create a wall using the location line
                        wall = Wall.Create(document, geomLine, type.Id, level_down.Id, 1.0, 0.0, false, true);
                        wall.get_Parameter(BuiltInParameter.WALL_HEIGHT_TYPE).Set(level_up.Id);
                    }
                    catch (Exception ex)
                    {
                        tr.RollBack();
                        error = ex.Message;
                    }
                }
            }
            else
            {
                Line geomLine = Line.CreateBound(start, end);
                // Create a wall using the location line
                wall = Wall.Create(document, geomLine, type.Id, level_down.Id, 1.0, 0.0, false, true);
                wall.get_Parameter(BuiltInParameter.WALL_HEIGHT_TYPE).Set(level_up.Id);
            }

            return wall;
        }

        public static Floor GreateFloor(Document document, Level level, double elevation, List<XYZ> contour, FloorType type, bool inTransaction, out string error)
        {
            Floor floor = null;
            error = "";

            if (inTransaction)
            {
                using (Transaction tr = new Transaction(document, "Create floor"))
                {
                    try
                    {
                        // Build a floor profile for the floor creation
                        CurveLoop profile = new CurveLoop();
                        for (int i = 0; i < contour.Count - 1; i++)
                        {
                            profile.Append(Line.CreateBound(contour[i], contour[i + 1]));
                        }
                        profile.Append(Line.CreateBound(contour[contour.Count - 1], contour[0]));

                        floor = Floor.Create(document, new List<CurveLoop> { profile }, type.Id, level.Id);
                        Parameter param = floor.get_Parameter(BuiltInParameter.FLOOR_HEIGHTABOVELEVEL_PARAM);
                        param.Set(elevation);
                    }
                    catch (Exception ex)
                    {
                        tr.RollBack();
                        error = ex.Message;
                    }
                }
            }
            else
            {
                // Build a floor profile for the floor creation
                CurveLoop profile = new CurveLoop();
                for (int i = 0; i < contour.Count - 1; i++)
                {
                    profile.Append(Line.CreateBound(contour[i], contour[i + 1]));
                }
                profile.Append(Line.CreateBound(contour[contour.Count - 1], contour[0]));

                floor = Floor.Create(document, new List<CurveLoop> { profile }, type.Id, level.Id);
                Parameter param = floor.get_Parameter(BuiltInParameter.FLOOR_HEIGHTABOVELEVEL_PARAM);
                param.Set(elevation);
            }

            return floor;
        }

        public static FamilyInstance CreateDoor(Document document, Wall wall, Level level, XYZ location, FamilySymbol doorType, bool inTransaction, out string error)
        {
            FamilyInstance fi = null;
            error = "";

            if (inTransaction)
            {
                using (Transaction tr = new Transaction(document, "Create door"))
                {
                    try
                    {
                        if (!doorType.IsActive)
                        {
                            doorType.Activate();
                            document.Regenerate();
                        }
                        fi = document.Create.NewFamilyInstance(location, doorType, wall, level, StructuralType.NonStructural);
                    }
                    catch (Exception ex)
                    {
                        tr.RollBack();
                        error = ex.Message;
                    }
                }
            }
            else
            {
                if (!doorType.IsActive)
                {
                    doorType.Activate();
                    document.Regenerate();
                }
                fi = document.Create.NewFamilyInstance(location, doorType, wall, level, StructuralType.NonStructural);
            }

            return fi;
        }

        public static FamilyInstance CreateWindow(Document document, Wall wall, Level level, XYZ location, FamilySymbol winType, bool inTransaction, out string error)
        {
            FamilyInstance fi = null;
            error = "";

            if (inTransaction)
            {
                using (Transaction tr = new Transaction(document, "Create window"))
                {
                    try
                    {
                        if (!winType.IsActive)
                        {
                            winType.Activate();
                            document.Regenerate();
                        }
                        fi = document.Create.NewFamilyInstance(location, winType, wall, level, StructuralType.NonStructural);
                    }
                    catch (Exception ex)
                    {
                        tr.RollBack();
                        error = ex.Message;
                    }
                }
            }
            else
            {
                if (!winType.IsActive)
                {
                    winType.Activate();
                    document.Regenerate();
                }
                fi = document.Create.NewFamilyInstance(location, winType, wall, level, StructuralType.NonStructural);
            }

            return fi;
        }

    }
}
