
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TaskAPI.models
{
    //The importance of the enum is that  we have to work with  him using the numbers because it makes incomptabilities
    //Using  a string part
    public enum Status
    {
        NonStarted = 0,
        InProgress = 1,
        Paused = 2,
        Late = 3,
        Finished = 4
    }
    public class Task
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

        [JsonProperty("description")]
        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonProperty("endDate")]
        [JsonPropertyName("endDate")]
        public string? EndDate { get; set; }

        [JsonProperty("status")]
        [JsonPropertyName("status")]
        public int Status { get; set; }

        //Decided not to make a class SubTasks because I do not see neccesary  I had thinked that  only with Tasks class is good
        [JsonProperty("subTasks")]
        [JsonPropertyName("subTasks")]
        public Task[]? SubTasks { get; set; }

        [JsonProperty("tags")]
        [JsonPropertyName("tags")]
        public Tag[]? Tags { get; set; }

        [JsonProperty("categories")]
        [JsonPropertyName("categories")]
        public Category[]? Categories { get; set; }

        public Task()
        {
        }

        public Task(string id, string title, string idUser, string? description, string? endDate, int status, Task[]? subTasks, Tag[]? tags, Category[]? categories)
        {
            this.Id = id;
            this.Title = title;
            this.IdUser = idUser;
            this.Description = description;
            this.EndDate = endDate;
            this.Status = status;
            this.SubTasks = subTasks;
            this.Tags = tags;
            this.Categories = categories;
        }
    } 
    public class TaskId
    {
        [JsonProperty("idUser")]
        [JsonPropertyName("idUser")]
        public string IdUser { get; set; }
    }


}
