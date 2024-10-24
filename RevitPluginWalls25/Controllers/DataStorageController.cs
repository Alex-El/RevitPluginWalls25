using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.UI;
using RevitPluginWalls.Abstract;
using RevitPluginWalls.Models;
using RevitPluginWalls25.Properties;
using System.Text.Json;

namespace RevitPluginWalls.Controllers
{
    internal class DataStorageController : IDataStorageController
    {
        Guid _schemaGuid;
        string _datastorageName;
        string _entityName;
        Document _doc;
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public DataStorageController(ExternalCommandData commandData)
        {
            _doc = commandData.Application.ActiveUIDocument.Document;
            _schemaGuid = new Guid(Resources.DataStorageGuid);
            _datastorageName = Resources.DataStorageName;
            _entityName = "Settings";
        }

        public bool TryReadProjectData(out DataStorageModel data)
        {
            data = new DataStorageModel { CreationMode = 0 };

            ExtensibleStorageFilter f = new ExtensibleStorageFilter(_schemaGuid);

            DataStorage dataStorage = new FilteredElementCollector(_doc)
                   .OfClass(typeof(DataStorage))
                   .WherePasses(f)
                   .Where(e => _datastorageName.Equals(e.Name))
                   .FirstOrDefault() as DataStorage;

            if (dataStorage == null) return false;

            Schema schema = Schema.Lookup(_schemaGuid);
            Entity entity = dataStorage.GetEntity(schema);

            if (entity == null) return false;
            string json = entity.Get<string>(_entityName);
            if (string.IsNullOrEmpty(json)) return false;

            try
            {
                data = JsonSerializer.Deserialize<DataStorageModel>(json);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public void WriteProjectData(DataStorageModel data)
        {
            ExtensibleStorageFilter f = new ExtensibleStorageFilter(_schemaGuid);

            DataStorage dataStorage = new FilteredElementCollector(_doc)
                   .OfClass(typeof(DataStorage))
                   .WherePasses(f)
                   .Where(e => _datastorageName.Equals(e.Name))
                   .FirstOrDefault() as DataStorage;

            if (dataStorage != null)
            {
                using (Transaction ts = new Transaction(_doc, "Create or write storage"))
                {
                    try
                    {
                        ts.Start();

                        // Create entity to store the Guid data
                        Schema schema = CreateSchema(_datastorageName, _schemaGuid);
                        Entity entity = new Entity(schema);
                        entity.Set(_entityName, JsonSerializer.Serialize(data));
                        // Set entity to the data storage element
                        dataStorage.SetEntity(entity);

                        ts.Commit();
                    }
                    catch (Exception ex)
                    {
                        ts.RollBack();
                        Logger.Error(ex);
                    }
                }
            }
            else
            {
                using (Transaction ts = new Transaction(_doc, "Create or write storage"))
                {
                    try
                    {
                        ts.Start();

                        // Create named data storage element
                        var newDataStorage = DataStorage.Create(_doc);
                        newDataStorage.Name = _datastorageName;
                        // Create entity to store the Guid data
                        Schema schema = CreateSchema(_datastorageName, _schemaGuid);
                        Entity entity = new Entity(schema);
                        entity.Set(_entityName, JsonSerializer.Serialize(data));
                        // Set entity to the data storage element
                        newDataStorage.SetEntity(entity);

                        ts.Commit();
                    }
                    catch (Exception ex)
                    {
                        ts.RollBack();
                        Logger.Error(ex);
                    }
                }
            }
        }

        Schema CreateSchema(string schemaName, Guid guid)
        {
            SchemaBuilder schemaBuilder = new SchemaBuilder(guid);
            schemaBuilder.SetSchemaName(schemaName);
            schemaBuilder.AddSimpleField(_entityName, typeof(string));
            return schemaBuilder.Finish();
        }
    }
}
