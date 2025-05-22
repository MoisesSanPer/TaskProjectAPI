using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using System.Web;
using TaskAPI.Services;
using Microsoft.Azure.Functions.Worker.Http;

namespace TaskAPI.Functions
{
    public class DeleteTag
    {
        private readonly CosmosClient _cosmosClient;
        private readonly IUserService _userService;

        public DeleteTag(CosmosClient cosmosClient, IUserService userService)
        {
            _cosmosClient = cosmosClient;
            _userService = userService;
        }

        [Function("DeleteTag")]
        public async Task<HttpStatusCode> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete")] HttpRequestData req,
            FunctionContext executionContext)
        {
            var queryParams = HttpUtility.ParseQueryString(req.Url.Query);
            var tagId = queryParams["id"];
            var tagBool = queryParams["isAll"];
            bool booleantag;
            if (tagBool == "true")
            {
                booleantag = true;
            }
            else
            {
                booleantag = false;
            }
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/json; c-harset=utf-8");
            var Category = await _userService.DeleteTag(tagId!, booleantag!);
            return Category;
        }
    }
}
