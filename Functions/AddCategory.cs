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
    internal class AddCategory
    {
        private readonly CosmosClient _cosmosClient;
        private readonly IUserService _userService;

        public AddCategory(CosmosClient cosmosClient, IUserService userService)
        {
            _cosmosClient = cosmosClient;
            _userService = userService;
        }

        //Deserialize first the  json post  that you recieve from FrontEnd and  later Serialize and send to  DataBase
        [Function("AddCategory")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req,
            FunctionContext executionContext)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var CategoryData = JsonSerializer.Deserialize<models.Category>(requestBody);
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/json; c-harset=utf-8");
            var Category = await _userService.AddCategory(CategoryData!.Id, CategoryData!.Title, CategoryData.IdUser);
            var categoryJson = JsonSerializer.Serialize(Category);
            await response.WriteStringAsync(categoryJson);
            return response;
        }
    }
}
