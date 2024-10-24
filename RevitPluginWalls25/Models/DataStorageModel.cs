using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RevitPluginWalls.Models
{
    internal class DataStorageModel
    {
        [JsonPropertyName("creation_mode")]
        public int CreationMode { get; set; }
    }
}
