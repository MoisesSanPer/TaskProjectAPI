using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace TaskAPI.models
{
    //Using both properties because we do not know exactly  what Serializacion uses
    public class User
    {
        [JsonProperty("id")]
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonProperty("username")]
        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonProperty("email")]
        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonProperty("password")]
        [JsonPropertyName("password")]
        public string Password { get; set; }

        [JsonProperty("token")]
        [JsonPropertyName("token")]
        public string Token { get; set; }

        public User() { }
        public override string ToString()
        {
            return base.ToString()!;
        }
    }
}
