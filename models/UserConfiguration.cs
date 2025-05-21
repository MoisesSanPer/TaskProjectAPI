using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Azure;
using Azure.Data.Tables;
using Newtonsoft.Json;

namespace TaskAPI.models
{
    public  class UserConfiguration:ITableEntity
    {
        [JsonProperty("theme")]
        [JsonPropertyName("theme")]
        public string Theme { get; set; }
        [JsonProperty("idUser")]
        [JsonPropertyName("idUser")]
        public string IdUser { get; set; }

        public string PartitionKey { get; set; }

        public string RowKey { get; set; }

        public DateTimeOffset? Timestamp { get; set; }

        public ETag ETag { get; set; }

        public UserConfiguration() { }


    }
    public class UserConfigurationId
    {
        [JsonProperty("idUser")]
        [JsonPropertyName("idUser")]
        public string IdUser { get; set; }
    }
}
