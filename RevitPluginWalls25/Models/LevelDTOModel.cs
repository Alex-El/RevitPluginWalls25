using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RevitPluginWalls.Models
{
    public class LevelDTOModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("uniq_name")]
        public string UniqName { get; set; }

        [JsonPropertyName("elevation")]
        public float Elevation { get; set; }
    }
}
