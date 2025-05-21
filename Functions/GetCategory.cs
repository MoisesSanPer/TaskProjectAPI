using System.Net;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker;
using TaskAPI.Services;
using System.Text.Json;

namespace TaskAPI.Functions
{
    internal class GetCategory
    {
        private readonly CosmosClient _cosmosClient;
        private readonly IUserService _userService;

        public GetCategory(CosmosClient cosmosClient, IUserService userService)
        {
            _cosmosClient = cosmosClient;
            _userService = userService;
        }

        //Get Functions returns a list of a object  that will be read later in the front end
        [Function("GetCategory")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req,
            FunctionContext executionContext)
        {
            var jsonData = executionContext.BindingContext.BindingData["Query"];
            var CategoryData = JsonSerializer.Deserialize<models.CategoryId>(jsonData!.ToString()!);
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json; charset=utf-8");
            var List = _userService.GetCategories(CategoryData!.IdUser);
            var list = await List;
            var listJson = JsonSerializer.Serialize(list);
            await response.WriteStringAsync(listJson);
            Console.WriteLine("The response2 is " + listJson);
            return response;
        }
    }
}
