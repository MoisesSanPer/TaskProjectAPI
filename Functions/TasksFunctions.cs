using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Text.Json;
using Microsoft.Azure.Cosmos;
using TaskAPI.Services;


namespace TaskAPI.Functions
{
    public class TasksFunctionsRegister
    {

        private readonly CosmosClient _cosmosClient;
        private readonly IUserService _userService;

        public TasksFunctionsRegister(CosmosClient cosmosClient, IUserService userService)
        {
            _cosmosClient = cosmosClient;
            _userService = userService;
        }

        [Function("register")]
        public  async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req,
            FunctionContext executionContext)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var userData = JsonSerializer.Deserialize<models.User>(requestBody);
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/json; charset=utf-8");
            var userTask =_userService.Register(userData!.Username, userData.Email, userData.Password);
            var user = await userTask;
            var userJson = JsonSerializer.Serialize(user);
            await response.WriteStringAsync(userJson);
            Console.WriteLine("The response2 is " + userJson);
            return response;
        }
    }
}
