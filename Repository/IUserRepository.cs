using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Grpc.Core;
using TaskAPI.models;
using Status = TaskAPI.models.Status;

namespace TaskAPI.Repository
{
    public interface IUserRepository
    {
        public Task<models.User> GetUserById(string id);
        public Task<models.User> Register(string username, string email, string password);
        public Task<models.User> Login(string email, string password);
        public Task<models.Task> AddTask(string id, string title, string idUser, int status, string? description = null, string? endDate = null, models.Task[]? subTasks = null, Tag[]? tags = null, Category[]? category = null);
        public Task<models.Category> AddCategory(string id, string title, string idUser);
        public Task<models.Tag> AddTag(string id, string title, string idUser);
        public Task<List<models.Category>> GetCategory(string idUser);
        public Task<List<models.Tag>> GetTags(string idUser);
        public Task<HttpStatusCode> DeleteCategory(string idCategory,bool isAll);
        public Task<HttpStatusCode> DeleteTag(string idTag);
        public Task<models.Category> UpdateCategory(string id, string title, string idUser);
        public Task<models.Tag> UpdateTag(string id, string title, string idUser);
        public Task<List<models.Task>> GetTasks(string idUser);
        public Task<HttpStatusCode> DeleteTask(string idTask);
        public Task<models.Task> UpdateTask(string id, string title, string idUser, string description, string endDate, int status, models.Task[] subTasks, Tag[] tags, Category[] category);
        public Task<models.UserConfiguration> AddTheme(string theme, string idUser);
        public Task<models.UserConfiguration> GetTheme(string idUser);
    }
}
