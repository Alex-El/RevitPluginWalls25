using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RevitPluginWalls.Models
{
    public class WallDTOModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("type_name")]
        public string WallTypeName { get; set; }

        [JsonPropertyName("level_base_name")]
        public string LevelBaseName { get; set; }

        [JsonPropertyName("level_top_name")]
        public string LevelTopName { get; set; }

        [JsonPropertyName("start_point")]
        public List<float> StartPoint { get; set; }

        [JsonPropertyName("end_point")]
        public List<float> EndPoint { get; set; }
    }
}
