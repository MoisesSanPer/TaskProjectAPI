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
    internal class GetTasks
    {
        private readonly CosmosClient _cosmosClient;
        private readonly IUserService _userService;

        public GetTasks(CosmosClient cosmosClient, IUserService userService)
        {
            _cosmosClient = cosmosClient;
            _userService = userService;
        }

        [Function("GetTasks")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req,
            FunctionContext executionContext)
        {
            var jsonData = executionContext.BindingContext.BindingData["Query"];
            var TaskData = JsonSerializer.Deserialize<models.TaskId>(jsonData!.ToString()!);
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json; charset=utf-8");
            var List = _userService.GetTasks(TaskData!.IdUser);
            var list = await List;
            var listJson = JsonSerializer.Serialize(list);
            await response.WriteStringAsync(listJson);
            Console.WriteLine("The response2 is " + listJson);
            return response;
        }
    }
}
