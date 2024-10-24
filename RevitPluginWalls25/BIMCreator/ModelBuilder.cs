using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitPluginWalls.Abstract;
using RevitPluginWalls.CommandData;
using RevitPluginWalls.Models;


namespace RevitPluginWalls.BIMCreator
{
    internal class ModelBuilder : IModelBuilder
    {
        public bool BuildModel(CommandDataStorage data, out string result)
        {
            result = string.Empty;

            if (data.Walls.Count == 0)
            {
                result = "Количество полученных стен = 0";
                return false;
            }

            if (data.Levels.Count == 0)
            {
                result = "Количество полученных уровней = 0";
                return false;
            }

            Document doc = data.ExternalCommandData.Application.ActiveUIDocument.Document;

            // set type of wall
            var wallcollector = new FilteredElementCollector(doc)
                .OfClass(typeof(WallType))
                .Cast<WallType>()
                .ToList();

            WallType wallType = wallcollector.FirstOrDefault(s => s.Name.Contains(data.Walls[0].WallTypeName));
            if (wallType == null)
            {
                result = $"В проекте не найден тип стен для имени {data.Walls[0].WallTypeName}";
                return false;
            }

            data.Levels.Sort(delegate (LevelDTOModel l1, LevelDTOModel l2) 
            { 
                if (l1.Elevation <  l2.Elevation) return -1;
                if (l1.Elevation ==  l2.Elevation) return 0;
                return 1;
            });

            int wall_counter = 0;
            int level_counter = 0;
            Level level_base = null;
            Level level_up = null;

            if (data.IsBuildFast)
            {
                using (Transaction tr = new Transaction(doc, "Create model"))
                {
                    try
                    {
                        tr.Start();

                        level_base = Creator.CreateLevel(doc, mm2ft(data.Levels[0].Elevation), data.Levels[0].UniqName);
                        if (level_base != null) level_counter++;

                        for (int i = 0; i < data.Levels.Count - 1; i++)
                        {
                            level_up = Creator.CreateLevel(doc, mm2ft(data.Levels[i+1].Elevation), data.Levels[i+1].UniqName);

                            if (level_up != null) level_counter++;

                            var walls = data.Walls.Where(s => (s.LevelBaseName.Contains(data.Levels[i].UniqName) && s.LevelTopName.Contains(data.Levels[i + 1].UniqName)));
                            foreach (var wall in walls)
                            {
                                var w = Creator.CreateWall(doc,
                                    level_base,
                                    level_up,
                                    XYZ2ft(new XYZ(wall.StartPoint[0], wall.StartPoint[1], wall.StartPoint[2])),
                                    XYZ2ft(new XYZ(wall.EndPoint[0], wall.EndPoint[1], wall.EndPoint[2])),
                                    wallType
                                    );

                                if (w != null) wall_counter++;
                            }

                            level_base = level_up;
                        }

                        tr.Commit();
                    }
                    catch (Exception ex)
                    {
                        tr.RollBack();
                        result = ex.Message;
                        return false;
                    }
                }
                result = $"Построено {level_counter} уровней, {wall_counter} стен.";
                return true;
            }

            using (Transaction tr = new Transaction(doc, "Create level"))
            {
                try
                {
                    tr.Start();

                    level_base = Creator.CreateLevel(doc, mm2ft(data.Levels[0].Elevation), data.Levels[0].UniqName);

                    tr.Commit();
                }
                catch (Exception ex)
                {
                    tr.RollBack();
                    TaskDialog.Show("Ошибка", $"Ошибка построения уровня {data.Levels[0].UniqName}\n{ex.Message}");
                }
            }
            if (level_base != null) level_counter++;

            for (int i = 0; i < data.Levels.Count - 1; i++)
            {
                using (Transaction tr = new Transaction(doc, "Create level"))
                {
                    try
                    {
                        tr.Start();

                        level_up = Creator.CreateLevel(doc, mm2ft(data.Levels[i + 1].Elevation), data.Levels[i + 1].UniqName);

                        tr.Commit();
                    }
                    catch (Exception ex)
                    {
                        tr.RollBack();
                        TaskDialog.Show("Ошибка", $"Ошибка построения уровней {data.Levels[i].UniqName} {data.Levels[i + 1].UniqName}\n{ex.Message}");
                    }
                }

                if (level_up != null) level_counter++;

                var walls = data.Walls.Where(s => (s.LevelBaseName.Contains(data.Levels[i].UniqName) && s.LevelTopName.Contains(data.Levels[i + 1].UniqName)));
                foreach (var wall in walls)
                {
                    Wall new_wall = null;

                    using (Transaction tr = new Transaction(doc, "Create wall"))
                    {
                        try
                        {
                            tr.Start();

                            new_wall = Creator.CreateWall(doc,
                                level_base,
                                level_up,
                                XYZ2ft(new XYZ(wall.StartPoint[0], wall.StartPoint[1], wall.StartPoint[2])),
                                XYZ2ft(new XYZ(wall.EndPoint[0], wall.EndPoint[1], wall.EndPoint[2])),
                                wallType
                                );

                            tr.Commit();
                        }
                        catch (Exception ex)
                        {
                            tr.RollBack();
                            TaskDialog.Show("Ошибка", $"Ошибка построения стены id = {wall.Id}\n{ex.Message}");
                        }
                    }
                        
                    if (new_wall != null) wall_counter++;
                }

                level_base = level_up;
            }

            result = $"Построено {level_counter} уровней, {wall_counter} стен.";
            return true;
        }

        double mm2ft(double l)
        {
            return l / 304.8;
        }

        XYZ XYZ2ft(XYZ p)
        {
            return new XYZ(p.X / 304.8, p.Y / 304.8, p.Z / 304.8);
        }


    }
}
