using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RevitPluginWalls.Models
{
    internal class WindowDTOModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("type_name")]
        public string TypeName { get; set; }

        [JsonPropertyName("host_id")]
        public int HostId { get; set; }

        [JsonPropertyName("location")]
        public List<float> Location { get; set; }
    }
}
