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
    internal class AddTheme
    {
        private readonly CosmosClient _cosmosClient;
        private readonly IUserService _userService;

        public AddTheme(CosmosClient cosmosClient, IUserService userService)
        {
            _cosmosClient = cosmosClient;
            _userService = userService;
        }

        //This functions is similar to all Add Functions  that we found it do the same
        [Function("AddTheme")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req,
            FunctionContext executionContext)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var UserData = JsonSerializer.Deserialize<models.UserConfiguration>(requestBody);
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json; c-harset=utf-8");
            var Theme = _userService.AddTheme(UserData!.Theme,UserData.IdUser);
            var theme = await Theme;
            var themeJson = JsonSerializer.Serialize(theme);
            await response.WriteStringAsync(themeJson);
            Console.WriteLine("The response2 is " + themeJson);
            return response;
        }
    }
}
