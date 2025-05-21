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
    internal class UpdateTask
    {
        private readonly CosmosClient _cosmosClient;
        private readonly IUserService _userService;

        public UpdateTask(CosmosClient cosmosClient, IUserService userService)
        {
            _cosmosClient = cosmosClient;
            _userService = userService;
        }

        [Function("UpdateTask")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "patch")] HttpRequestData req,
            FunctionContext executionContext)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var TaskData = JsonSerializer.Deserialize<models.Task>(requestBody);
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/json; c-harset=utf-8");
            var Task = _userService.UpdateTask(TaskData!.Id, TaskData.Title, TaskData.IdUser, TaskData!.Description! ,TaskData!.EndDate!, TaskData.Status, TaskData.SubTasks!, TaskData.Tags!, TaskData.Categories!);
            return response;
        }
    }
}
