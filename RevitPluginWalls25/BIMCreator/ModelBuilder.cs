using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitPluginWalls.Abstract;
using RevitPluginWalls.CommandData;
using RevitPluginWalls.Models;
using System.Net.WebSockets;
using System.Windows.Controls;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace RevitPluginWalls.BIMCreator
{
    internal class ModelBuilder : IModelBuilder
    {
        public Dictionary<DataObjectType, DataRecord> reports = new Dictionary<DataObjectType, DataRecord>();

        FloorType floorType = null;
        WallType wallType = null;
        WallType basementType = null;
        FamilySymbol doorType = null;
        FamilySymbol winType = null;

        List<Level> created_levels = new List<Level>();
        List<Floor> created_floors = new List<Floor>();
        List<Wall> created_basements = new List<Wall>();
        List<(int, Wall, Level)> created_walls = new List<(int, Wall, Level)>();
        List<FamilyInstance> created_doors = new List<FamilyInstance>();
        List<FamilyInstance> created_windows = new List<FamilyInstance>();

        public bool BuildModel(CommandDataStorage data, out string result)
        {
            // data 
            result = string.Empty;

            reports.Add(DataObjectType.None, new DataRecord());
            reports.Add(DataObjectType.Level, new DataRecord { Name = "Уровни" });
            reports.Add(DataObjectType.Floor, new DataRecord { Name = "Полы" });
            reports.Add(DataObjectType.Basement, new DataRecord { Name = "Фундамент" });
            reports.Add(DataObjectType.Wall, new DataRecord { Name = "Стены" });
            reports.Add(DataObjectType.Door, new DataRecord { Name = "Двери" });
            reports.Add(DataObjectType.Window, new DataRecord { Name = "Окна" });

            reports[DataObjectType.Level].CountIn = data.Levels.Count;
            reports[DataObjectType.Floor].CountIn = data.Slabs.Count;
            reports[DataObjectType.Basement].CountIn = data.Basements.Count;
            reports[DataObjectType.Wall].CountIn = data.Walls.Count;
            reports[DataObjectType.Door].CountIn = data.Doors.Count;
            reports[DataObjectType.Window].CountIn = data.Windows.Count;

            Document doc = data.ExternalCommandData.Application.ActiveUIDocument.Document;

            if (data.Slabs.Count > 0)
            {
                var floorcollector = new FilteredElementCollector(doc)
                    .OfClass(typeof(FloorType))
                    .Cast<FloorType>()
                    .ToList();
                floorType = floorcollector.FirstOrDefault(s => s.Name.Contains(data.Slabs[0].TypeName));
                if (floorType == null)
                {
                    reports[DataObjectType.Floor].Errors.Add($"В проекте на найден тип {data.Slabs[0].TypeName}");
                    data.Slabs.Clear();
                }
            }

            if (data.Basements.Count > 0)
            {
                var wallcollector = new FilteredElementCollector(doc)
                    .OfClass(typeof(WallType))
                    .Cast<WallType>()
                    .ToList();

                basementType = wallcollector.FirstOrDefault(s => s.Name.Contains(data.Basements[0].TypeName));
                if (basementType == null)
                {
                    reports[DataObjectType.Basement].Errors.Add($"В проекте на найден тип {data.Basements[0].TypeName}");
                    data.Basements.Clear();
                }
            }

            if (data.Walls.Count > 0)
            {
                var wallcollector = new FilteredElementCollector(doc)
                    .OfClass(typeof(WallType))
                    .Cast<WallType>()
                    .ToList();

                wallType = wallcollector.FirstOrDefault(s => s.Name.Contains(data.Walls[0].WallTypeName));
                if (wallType == null)
                {
                    reports[DataObjectType.Wall].Errors.Add($"В проекте на найден тип {data.Walls[0].WallTypeName}");
                    data.Walls.Clear();
                }
            }

            if (data.Doors.Count > 0)
            {
                FilteredElementCollector doorcollector = new FilteredElementCollector(doc);
                IList<Element> doorTypes = doorcollector.OfClass(typeof(FamilySymbol))
                    .OfCategory(BuiltInCategory.OST_Doors)
                    .WhereElementIsElementType()
                    .ToElements();

                doorType = doorcollector.FirstOrDefault(s => s.Name.Contains(data.Doors[0].TypeName)) as FamilySymbol;
                if (doorType == null)
                {
                    reports[DataObjectType.Door].Errors.Add($"В проекте не найдено семейство {data.Doors[0].TypeName}");
                    data.Doors.Clear();
                }
            }

            if (data.Windows.Count > 0)
            {
                FilteredElementCollector wincollector = new FilteredElementCollector(doc);
                IList<Element> winTypes = wincollector.OfClass(typeof(FamilySymbol))
                    .OfCategory(BuiltInCategory.OST_Windows)
                    .WhereElementIsElementType()
                    .ToElements();

                winType = wincollector.FirstOrDefault(s => s.Name.Contains(data.Windows[0].TypeName)) as FamilySymbol;
                if (winType == null)
                {
                    reports[DataObjectType.Window].Errors.Add($"В проекте не найдено семейство {data.Windows[0].TypeName}");
                    data.Windows.Clear();
                }
            }

            // creation
            data.Levels.Sort(delegate (LevelDTOModel l1, LevelDTOModel l2) 
            { 
                if (l1.Elevation <  l2.Elevation) return -1;
                if (l1.Elevation ==  l2.Elevation) return 0;
                return 1;
            });

            if (data.IsBuildFast)
            {
                using (Transaction tr = new Transaction(doc, "Create model"))
                {
                    try
                    {
                        _ = Build(doc, data, false);
                    }
                    catch (Exception ex)
                    {
                        tr.RollBack();
                        reports[DataObjectType.None].Errors.Add(ex.Message);
                        result = CreateReport();
                        return false;
                    }
                }
                
            }
            else
            {
                reports[DataObjectType.None].Errors = Build(doc, data, true);
            }

            result = CreateReport();
            return true;
        }

        List<string> Build(Document doc, CommandDataStorage data, bool inTransaction)
        {
            var result = new List<string>();

            foreach (var level in data.Levels)
            {
                var nl = Creator.CreateLevel(doc, mm2ft(data.Levels[0].Elevation), data.Levels[0].UniqName, inTransaction, out string err);
                if (nl != null)
                {
                    created_levels.Add(nl);
                }
                else
                {
                    reports[DataObjectType.Level].Errors.Add($"не создан id = {level.Id}");
                }
                result.Add(err);
            }
            reports[DataObjectType.Level].CountDone = created_levels.Count;

            foreach (var bwall in data.Basements)
            {
                var l_down = created_levels.FirstOrDefault(l => l.Name.Equals(bwall.LevelBaseName));
                if (l_down == null)
                {
                    reports[DataObjectType.Basement].Errors.Add($"не найден нижний уровень для id = {bwall.Id}");
                    continue;
                }

                var l_top = created_levels.FirstOrDefault(l => l.Name.Equals(bwall.LevelTopName));
                if (l_top == null)
                {
                    reports[DataObjectType.Basement].Errors.Add($"не найден верхний уровень для id = {bwall.Id}");
                    continue;
                }

                var nb = Creator.CreateWall(doc,
                    l_down,
                    l_top,
                    XYZ2ft(new XYZ(bwall.StartPoint[0], bwall.StartPoint[1], bwall.StartPoint[2])),
                    XYZ2ft(new XYZ(bwall.EndPoint[0], bwall.EndPoint[1], bwall.EndPoint[2])),
                    basementType,
                    inTransaction,
                    out string err
                    );

                if (nb != null)
                {
                    created_basements.Add(nb);
                }
                else
                {
                    reports[DataObjectType.Basement].Errors.Add($"не создан id = {bwall.Id}");
                }
                result.Add(err);
            }
            reports[DataObjectType.Basement].CountDone = created_basements.Count;

            foreach (var wall in data.Walls)
            {
                var l_down = created_levels.FirstOrDefault(l => l.Name.Equals(wall.LevelBaseName));
                if (l_down == null)
                {
                    reports[DataObjectType.Wall].Errors.Add($"не найден нижний уровень для id = {wall.Id}");
                    continue;
                }

                var l_top = created_levels.FirstOrDefault(l => l.Name.Equals(wall.LevelTopName));
                if (l_top == null)
                {
                    reports[DataObjectType.Wall].Errors.Add($"не найден верхний уровень для id = {wall.Id}");
                    continue;
                }

                var nw = Creator.CreateWall(doc,
                    l_down,
                    l_top,
                    XYZ2ft(new XYZ(wall.StartPoint[0], wall.StartPoint[1], wall.StartPoint[2])),
                    XYZ2ft(new XYZ(wall.EndPoint[0], wall.EndPoint[1], wall.EndPoint[2])),
                    wallType,
                    inTransaction,
                    out string err
                    );

                if (nw != null)
                {
                    created_walls.Add((wall.Id, nw, l_down));
                }
                else
                {
                    reports[DataObjectType.Wall].Errors.Add($"не создан id = {wall.Id}");
                }
                result.Add(err);
            }
            reports[DataObjectType.Wall].CountDone = created_walls.Count;

            foreach (var slab in data.Slabs)
            {
                var lev = created_levels.FirstOrDefault(s => s.Name.Contains(slab.LevelBaseName));
                if (lev == null)
                {
                    reports[DataObjectType.Floor].Errors.Add($"не найден уровень для id = {slab.Id}");
                    continue;
                }

                var contour = new List<XYZ>();
                slab.Polygon.ForEach(p => contour.Add(XYZ2ft(new XYZ(p[0], p[1], p[2]))));

                var ns = Creator.GreateFloor(doc, lev, lev.Elevation, contour, floorType, inTransaction, out string err);
                if (ns != null)
                {
                    created_floors.Add(ns);
                }
                else
                {
                    reports[DataObjectType.Floor].Errors.Add($"не создан id = {slab.Id}");
                }
                result.Add(err);
            }
            reports[DataObjectType.Floor].CountDone = created_floors.Count;

            foreach (var door in data.Doors)
            {
                var host = created_walls.FirstOrDefault(s => s.Item1 == door.HostId);
                if (host.Item2 == null)
                {
                    reports[DataObjectType.Door].Errors.Add($"не найдена стена для id = {door.Id}");
                    continue;
                }

                var nd = Creator.CreateDoor(doc, 
                    host.Item2, 
                    host.Item3, 
                    XYZ2ft(new XYZ(door.Location[0], door.Location[1], door.Location[2])), 
                    doorType, 
                    inTransaction, 
                    out string res );

                if (nd != null)
                {
                    created_doors.Add(nd);
                }
                else
                {
                    reports[DataObjectType.Door].Errors.Add($"не создан id = {door.Id}");
                }
            }
            reports[DataObjectType.Door].CountDone = created_doors.Count;

            foreach(var win in data.Windows)
            {
                var host = created_walls.FirstOrDefault(s => s.Item1 == win.HostId);
                if (host.Item2 == null)
                {
                    reports[DataObjectType.Window].Errors.Add($"не найдена стена для id = {win.Id}");
                    continue;
                }

                var nw = Creator.CreateWindow(doc,
                    host.Item2,
                    host.Item3,
                    XYZ2ft(new XYZ(win.Location[0], win.Location[1], win.Location[2])),
                    winType,
                    inTransaction,
                    out string res);

                if (nw != null)
                {
                    created_doors.Add(nw);
                }
                else
                {
                    reports[DataObjectType.Window].Errors.Add($"не создан id = {win.Id}");
                }
            }
            reports[DataObjectType.Window].CountDone = created_windows.Count;

            return result;
        }

        double mm2ft(double l)
        {
            return l / 304.8;
        }

        XYZ XYZ2ft(XYZ p)
        {
            return new XYZ(p.X / 304.8, p.Y / 304.8, p.Z / 304.8);
        }

        string CreateReport()
        {
            string res = "";
            foreach(var k in reports.Keys)
            {
                res += $"{reports[k].Name}: получено: {reports[k].CountIn}; построено: {reports[k].CountDone}; ошибки: {string.Join(", ", reports[k].Errors)};\n";
            }

            if (!reports[DataObjectType.None].Errors.All(s => string.IsNullOrEmpty(s)))
            {
                res += "\n\n";
                res += "Ошибки построения:\n";
                res += string.Join("\n", reports[DataObjectType.None].Errors);
            }

            return res;
        }
    }

    public record DataRecord
    {
        public string Name { get; set; } = "";
        public int CountIn { get; set; } = 0;
        public int CountDone { get; set; } = 0;
        public List<string> Errors { get; set; } = new List<string>();
    }

    enum DataObjectType
    {
        None = 0,
        Level = 1,
        Floor = 2,
        Basement = 3,
        Wall = 4,
        Window = 5,
        Door = 6
    }
}
