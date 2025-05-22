using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Azure;
using Grpc.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Cosmos;
using Microsoft.IdentityModel.Tokens;
using TaskAPI.models;
using TaskAPI.Repository;

namespace TaskAPI.Services
{
    //This is the layer that uses the repository
    public  class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public  Task<models.User> GetUsersById(string id)
        {
            return _userRepository.GetUserById(id);
        }

        public async  Task<models.User> Login(string email, string password)
        {
            var user = await  _userRepository.Login(email, password);
            if (user != null)
            {
                user.Token = GenerateToken(email);
                return user;
            }
            return null!;
        }

        public  async Task<models.User> Register(string username,string email ,string password)
        {
            var user = await _userRepository.Register(username, email, password);
            user.Token = GenerateToken(username);
            return user;   
        }
        public async Task<models.Task> AddTask(string id, string title, string idUser, int status, string? description = null, string? endDate = null, models.Task[]? subTasks = null, Tag[]? tags = null, Category[]? category = null)
        {
            return await _userRepository.AddTask(id, title, idUser, status, description, endDate, subTasks, tags, category);
        }

        public async Task<Category> AddCategory(string id, string title, string idUser)
        {
           return await _userRepository.AddCategory(id, title, idUser);
        }
        public async Task<Tag> AddTag(string id, string title, string idUser)
        {
            return await _userRepository.AddTag(id, title, idUser);  
        }
        public async Task<List<Category>> GetCategories(string idUser)
        {
           return await _userRepository.GetCategory(idUser);
        }
        public async Task<List<Tag>> GetTags(string idUser)
        {
            return await _userRepository.GetTags(idUser);
        }
        public async Task<HttpStatusCode> DeleteCategory(string idCategory, bool isAll)
        {
            return  await _userRepository.DeleteCategory(idCategory,isAll);
        }
        public async Task<HttpStatusCode> DeleteTag(string idTag, bool isAll)
        {
            return await _userRepository.DeleteTag(idTag,isAll);
        }
        public async Task<models.Category> UpdateCategory(string id, string title, string idUser)
        {
            return await _userRepository.UpdateCategory(id, title, idUser);
        }
        public async Task<models.Tag> UpdateTag(string id, string title, string idUser)
        {
            return await _userRepository.UpdateTag(id, title, idUser);
        }
        public async Task<List<models.Task>> GetTasks(string idUser)
        {
           return await _userRepository.GetTasks(idUser);
        }
        public async Task<HttpStatusCode> DeleteTask(string idTask)
        {
            return await _userRepository.DeleteTask(idTask);
        }
        public async  Task<models.Task> UpdateTask(string id, string title, string idUser, string description, string endDate, int status, models.Task[] subTasks, Tag[] tags, Category[] category)
        {
           return await _userRepository.UpdateTask(id, title, idUser, description, endDate, status, subTasks, tags, category);
        }
        public async Task<models.UserConfiguration> AddTheme(string theme, string idUser)
        {
           return await _userRepository.AddTheme(theme, idUser);
        }
        public async Task<UserConfiguration> GetTheme(string idUser)
        {
            return await _userRepository.GetTheme(idUser);
        }
        //This method generate a Token using and email and the basics reclamations method to create it 
        public string GenerateToken(string email)
        {
            var claimsIdentity = new ClaimsIdentity(new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Email,email),
            });
            var key = "580913574639645127984624132452156t21516t216t1251ewrf3we1qf21qefvq3w4fg243";
            var encodeKey = Encoding.ASCII.GetBytes(key);

            var tokenDescriptor = new SecurityTokenDescriptor()
            { 
                Subject = claimsIdentity,
                Expires = DateTime.Now.AddHours(2),
                Audience = "Postman",
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(encodeKey),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwt = tokenHandler.WriteToken(token);
            Console.WriteLine($"The token is  {jwt}");
            return jwt;
        }

      
    }
}
