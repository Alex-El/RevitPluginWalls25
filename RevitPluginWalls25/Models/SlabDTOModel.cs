using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RevitPluginWalls.Models
{
    internal class SlabDTOModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("type_name")]
        public string TypeName { get; set; }

        [JsonPropertyName("level_base_name")]
        public string LevelBaseName { get; set; }

        [JsonPropertyName("polygon")]
        public List<float[]> Polygon { get; set; }
    }
}
