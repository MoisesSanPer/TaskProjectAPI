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
    internal class UpdateTag
    {
        private readonly CosmosClient _cosmosClient;
        private readonly IUserService _userService;
        public UpdateTag(CosmosClient cosmosClient, IUserService userService)
        {
            _cosmosClient = cosmosClient;
            _userService = userService;
        }

        [Function("UpdateTag")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "patch")] HttpRequestData req,
            FunctionContext executionContext)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var TagData = JsonSerializer.Deserialize<models.Tag>(requestBody);
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/json; c-harset=utf-8");
            var Tag = _userService.UpdateTag(TagData!.Id, TagData.Title, TagData.IdUser);
            return response;
        }
    }
}
