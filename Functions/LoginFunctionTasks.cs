
using System.Net;
using System.Text.Json;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker;
using TaskAPI.Services;

namespace TaskAPI.Functions
{
    public class LoginFunctionTasks
    {
        private readonly CosmosClient _cosmosClient;
        private readonly IUserService _userService;

        public LoginFunctionTasks(CosmosClient cosmosClient, IUserService userService)
        {
            _cosmosClient = cosmosClient;
            _userService = userService;
        }

        //This is the call function that login in the app which have the same code that a normal post
        [Function("login")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req,
            FunctionContext executionContext)
        {
            try
            {
                var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var userData = JsonSerializer.Deserialize<models.User>(requestBody);
                var response = req.CreateResponse(HttpStatusCode.OK);
                response.Headers.Add("Content-Type", "text/json; charset=utf-8");
                var userTask = _userService.Login(userData!.Email, userData.Password);
                var user = await userTask;
                var userJson = JsonSerializer.Serialize(user);
                await response.WriteStringAsync(userJson);
                Console.WriteLine("The response2 is " + userJson);
                return response;
            }
            catch(Exception e)
            {
                var responseNotFound = req.CreateResponse(HttpStatusCode.NotFound);
                return responseNotFound;
            }

        }
    }
}
