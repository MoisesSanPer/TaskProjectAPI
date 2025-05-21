
using System.Net;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker;
using TaskAPI.Services;
using System.Text.Json;
using Grpc.Core;
using System.Web;

namespace TaskAPI.Functions
{
    public class DeleteCategory
    {
        private readonly CosmosClient _cosmosClient;
        private readonly IUserService _userService;

        public DeleteCategory(CosmosClient cosmosClient, IUserService userService)
        {
            _cosmosClient = cosmosClient;
            _userService = userService;
        }

        //This functions read the paramater of the url that send from frontend and  mapping to a variable we use as paramter of a method
        [Function("DeleteCategory")]
        public async Task<HttpStatusCode> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete")] HttpRequestData req,
            FunctionContext executionContext)
        {
            var queryParams = HttpUtility.ParseQueryString(req.Url.Query);
            var categoryId = queryParams["id"];
            var categoryBool= queryParams["isAll"];
            bool booleancategory;
            if (categoryBool == "true")
            {
                booleancategory=true;
            }
            else{
                booleancategory = false;
            }
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/json; c-harset=utf-8");
            var Category =  await _userService.DeleteCategory(categoryId!, booleancategory!);
            return Category;
        }
    }
}
