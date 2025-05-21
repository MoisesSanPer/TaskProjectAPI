using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker;
using TaskAPI.Services;

namespace TaskAPI.Functions
{
    public  class AddTask
    {
        private readonly CosmosClient _cosmosClient;
        private readonly IUserService _userService;

        public AddTask(CosmosClient cosmosClient, IUserService userService)
        {
            _cosmosClient = cosmosClient;
            _userService = userService;
        }

        //This functions is similar to all Add Functions  that we found it do the same
        [Function("AddTask")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req,
            FunctionContext executionContext)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var TaskData = JsonSerializer.Deserialize<models.Task>(requestBody);
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json; c-harset=utf-8");
            var Task = _userService.AddTask(TaskData!.Id, TaskData!.Title , TaskData.IdUser, TaskData.Status, TaskData.Description, TaskData.EndDate, TaskData.SubTasks, TaskData.Tags, TaskData.Categories);
            var task = await Task;
            var taskJson = JsonSerializer.Serialize(task);
            await response.WriteStringAsync(taskJson);
            Console.WriteLine("The response2 is " + taskJson);
            return response;
        }
    }
}
