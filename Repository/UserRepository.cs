using System.Net;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using TaskAPI.models;
using Azure.Data.Tables;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Azure;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;


namespace TaskAPI.Repository
{
    //This class contains the calls to the  database and the principal  operations 
    //Using Linq Queries  in most of Tasks that are possible 
    //It is important to know that the calls to the functions that add , delete  , get .. to the database are asynchronous
    //because the performance of the application upgrade a lot 
    internal class UserRepository : IUserRepository
    {
        private readonly CosmosClient _cosmosClient;


        public UserRepository(CosmosClient cosmosClient)
        {
            _cosmosClient = cosmosClient;
        }

        public async Task<TableClient> ConnectionTables(string containerName)
        {
            var azuriteConnectionString = Environment.GetEnvironmentVariable("AzuriteStorage");
            var serviceClient = new TableServiceClient(azuriteConnectionString);
            var tables = serviceClient.GetTableClient(containerName);
            await tables.CreateIfNotExistsAsync();
            return tables;
        }


        public Task<models.User> GetUserById(string id)
        {
            var container = _cosmosClient.GetContainer("TaskProject", "Users");
            var user = container.GetItemLinqQueryable<models.User>().Where(c => c.Id == id).FirstOrDefault();
            if(user != null)
            {
                return System.Threading.Tasks.Task.FromResult(user);
            }
            return System.Threading.Tasks.Task.FromResult<models.User>(null!);
        }

        public async Task< models.User> Login(string email, string password)
        {
                var container =  _cosmosClient.GetContainer("TaskProject", "Users");
                var user =  container.GetItemLinqQueryable<models.User>().Where(c => c.Email == email && c.Password == password).FirstOrDefault();
                if (user != null)
                {
                return user;
                }
                throw new Exception("User not found"); 
        }

        public async  Task<models.User> Register(string username, string email, string password)
        {
            var container = _cosmosClient.GetContainer("TaskProject", "Users");
            models.User user = new models.User();
            user.Username = username;
            user.Email = email;
            user.Password = password;
            user.Id = Guid.NewGuid().ToString();
            await container.CreateItemAsync<models.User>(user);
            return user;
       }
        public async  Task<models.Task> AddTask(string id, string title, string idUser, int status, string? description = null, string? endDate = null, models.Task[]? subTasks = null, Tag[]? tags = null, Category[]? category = null)
        {
            var container = _cosmosClient.GetContainer("TaskProject","Tasks");
            models.Task task = new models.Task();
            task.Id = id;
            task.Title = title;
            task.Description = description;
            task.EndDate = endDate;
            task.Status=status; 
            task.SubTasks = subTasks;
            task.Tags = tags;
            task.Categories = category;
            task.IdUser = idUser;
            await container.CreateItemAsync<models.Task>(task);
            return task;
        }

        public async Task<Category> AddCategory(string id, string title, string idUser)
        {
            var container = _cosmosClient.GetContainer("TaskProject", "Category");
            models.Category category = new models.Category();
            category.Title = title;
            category.IdUser = idUser;
            category.Id = id;
            await container.CreateItemAsync<models.Category>(category);
            return category;
        }

        public async  Task<Tag> AddTag(string id, string title, string idUser)
        {
            var container = _cosmosClient.GetContainer("TaskProject", "Tag");
            models.Tag tag = new models.Tag();
            tag.Title = title;
            tag.IdUser = idUser;
            tag.Id = id;
           await container.CreateItemAsync<models.Tag>(tag);
            return tag;
        }

        //Important to filter to have the corrects Categorys of the user  this is common  in get Functions
        public async  Task<List<Category>> GetCategory(string idUser)
        {
            var container = _cosmosClient.GetContainer("TaskProject", "Category");
            var query = container.GetItemLinqQueryable<models.Category>().Where((c) => c.IdUser == idUser).ToFeedIterator();
            var categories = new List<Category>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                categories.AddRange(response);
            }
            return categories;
        }

        public async Task<List<Tag>> GetTags(string idUser)
        {
            var container = _cosmosClient.GetContainer("TaskProject", "Tag");
            var query = container.GetItemLinqQueryable<models.Tag>().Where((c) => c.IdUser == idUser).ToFeedIterator();
            var tags = new List<Tag>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                tags.AddRange(response);
            }
            return tags;
        }
        // Fix for CS0029 and CS8604 in the DeleteCategory method
        public async Task<HttpStatusCode> DeleteCategory(string idCategory, bool isAll)
        {
            var container = _cosmosClient.GetContainer("TaskProject", "Category");
            var taskContainer = _cosmosClient.GetContainer("TaskProject", "Tasks");

            // Here we are finding the id and getting the category of the category
            var query = taskContainer.GetItemLinqQueryable<models.Task>()
                .Where(t => t.Categories != null && t.Categories.Any(c => c.Id == idCategory))
                .ToFeedIterator();
            while (query.HasMoreResults)
            {
                foreach (var task in await query.ReadNextAsync())
                {
                    //Here we reaAsign to the property categories all the categories that are distinct 
                    //Because  in cosmos we can not delete a field from a  document
                    task.Categories = task.Categories?
                        .Where(c => c.Id != idCategory)
                        .ToArray();
                    // Replace the updated task
                    if(isAll == true)
                    {
                        await taskContainer.ReplaceItemAsync(task, task.Id, new PartitionKey(task.Id));
                    }
                    else
                    {
                        await taskContainer.DeleteItemAsync<models.Task>(task.Id, new PartitionKey(task.Id));
                    }
                    
                }
            }   
            // Here we delete the category item in general
            var result = await container.DeleteItemAsync<models.Category>(idCategory, new PartitionKey(idCategory));
            return result.StatusCode;
        }
        public async Task<HttpStatusCode> DeleteTag(string idTag, bool isAll)
        {
            var container = _cosmosClient.GetContainer("TaskProject", "Tag");
            var taskContainer = _cosmosClient.GetContainer("TaskProject", "Tasks");

            var query = taskContainer.GetItemLinqQueryable<models.Task>()
                .Where(t => t.Tags != null && t.Tags.Any(tag => tag.Id ==idTag))
                .ToFeedIterator();

            while(query.HasMoreResults)
            {
                foreach(var task in  await query.ReadNextAsync())
                {
                    task.Tags = task.Tags?.Where(tag => tag.Id != idTag).ToArray();

                    if (isAll == true)
                    {
                        await taskContainer.ReplaceItemAsync(task, task.Id, new PartitionKey(task.Id));
                    }
                    else
                    {
                        await taskContainer.DeleteItemAsync<models.Task>(task.Id, new PartitionKey(task.Id));
                    }
                }
            }
            var result = await container.DeleteItemAsync<models.Tag>(idTag, new PartitionKey(idTag));
            return result.StatusCode;
        }

        public async Task<Category> UpdateCategory(string id, string title, string idUser)
        {
            var container = _cosmosClient.GetContainer("TaskProject", "Category");
            var  category = new Category(id, title, idUser);
            var result = await container.ReplaceItemAsync<models.Category>(category, category.Id, new PartitionKey(category.Id));
            return result;
        }
        public async Task<Tag> UpdateTag(string id, string title, string idUser)
        {
            var container = _cosmosClient.GetContainer("TaskProject", "Tag");
            var tag = new Tag(id, title, idUser);
            var result = await container.ReplaceItemAsync<models.Tag>(tag, tag.Id, new PartitionKey(tag.Id));
            return result;
        }

        public async  Task<List<models.Task>> GetTasks(string idUser)
        {
            var container = _cosmosClient.GetContainer("TaskProject", "Tasks");
            var query = container.GetItemLinqQueryable<models.Task>().Where((c) => c.IdUser == idUser).ToFeedIterator();
            var task = new List<models.Task>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                task.AddRange(response);
            }
            return task;
        }
      public async  Task<HttpStatusCode> DeleteTask(string idTask)
        {
            var container = _cosmosClient.GetContainer("TaskProject", "Tasks");
            var result = await container.DeleteItemAsync<models.Task>(idTask, new PartitionKey(idTask));
            return result.StatusCode;
        }

        public async Task<models.Task> UpdateTask(string id, string title, string idUser, string description, string endDate, int status, models.Task[] subTasks, Tag[] tags, Category[] category)
        {
            var container = _cosmosClient.GetContainer("TaskProject", "Tasks");
            var task = new models.Task(id, title, idUser, description, endDate, status, subTasks, tags, category);
            var result = await container.ReplaceItemAsync<models.Task>(task, task.Id, new PartitionKey(task.Id));
            return result;
        }

        public async  Task<models.UserConfiguration> AddTheme(string theme, string idUser)
        {
            var table = await ConnectionTables("cache");
            var tableEntity = new UserConfiguration
            {
                Theme = theme,
                RowKey = idUser,
                PartitionKey= idUser
            };
            table.UpsertEntity(tableEntity);
            return tableEntity;

        }

        public async Task<UserConfiguration> GetTheme(string idUser)
        {
            var table = await ConnectionTables("cache");
            var response = await table.GetEntityAsync<Azure.Data.Tables.TableEntity>(idUser, idUser);
            var theme = response.Value.GetString("Theme");
            return new UserConfiguration
            {
                Theme = theme,
                PartitionKey = idUser,
                RowKey= idUser,
                IdUser = idUser,
            };
        }
    }

}
