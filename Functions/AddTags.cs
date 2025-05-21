using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker;
using TaskAPI.Services;
using System.Text.Json;

namespace TaskAPI.Functions
{
    public class AddTags
    {
        private readonly CosmosClient _cosmosClient;
        private readonly IUserService _userService;

        public AddTags(CosmosClient cosmosClient, IUserService userService)
        {
            _cosmosClient = cosmosClient;
            _userService = userService;
        }

        [Function("AddTags")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req,
            FunctionContext executionContext)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var TagData = JsonSerializer.Deserialize<models.Tag>(requestBody);
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json; c-harset=utf-8");
            var Tag = _userService.AddTag(TagData!.Id, TagData.Title, TagData.IdUser);
            var tag = await Tag;
            var tagJson = JsonSerializer.Serialize(tag);
            await response.WriteStringAsync(tagJson);
            Console.WriteLine("The response2 is " + tagJson);
            return response;
        }
    }
}
