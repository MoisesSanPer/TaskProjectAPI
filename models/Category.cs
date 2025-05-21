using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TaskAPI.models
{
    public class Category
    {
        [JsonProperty("id")]
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonProperty("title")]
        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonProperty("idUser")]
        [JsonPropertyName("idUser")]
        public string IdUser { get; set; }

        public Category(string id , string title, string idUser) 
        {
            this.Id = id;
            this.Title = title;
             this.IdUser = idUser;
        }
        public Category() { }
    }
    public class CategoryId
    {
        [JsonProperty("idUser")]
        [JsonPropertyName("idUser")]
        public string IdUser { get; set; }
    }
}
