using System.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;

namespace TodoCli
{
    public class ApiToken
    {
        public string Token { get; set; }
    }
    public class TodoApiRepository : ITodoRepository
    {
        private static readonly string DIRECTORY_PATH = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".todo");
        private static readonly string USER_CREDENTIAL_FILENAME = "todo-user";
        private static readonly string API_TOKEN_FILENAME = "todo-api-token";
        private readonly IFlurlClient restClient;

        public TodoApiRepository()
        {
            restClient = new FlurlClient(baseUrl: "http://localhost:5000/");
            string apiKeyFilePath = Path.Combine(DIRECTORY_PATH, API_TOKEN_FILENAME);
            if (!File.Exists(apiKeyFilePath))
            {
                string userCredentialFile = Path.Combine(DIRECTORY_PATH, USER_CREDENTIAL_FILENAME);
                if (!File.Exists(userCredentialFile))
                {
                    throw new FileNotFoundException($"File '{Path.GetFileName(userCredentialFile)}' not found in '{DIRECTORY_PATH}'.");
                }

                string[] credentials = File.ReadAllLines(userCredentialFile).Select(str => str.Trim()).ToArray();
                if (credentials == null || !credentials.Any())
                    throw new InvalidOperationException("Invalid credential format.");
                var resp = restClient.Request("auth", "login")
                    .PostJsonAsync(new
                    {
                        username = credentials[0],
                        password = credentials[1]
                    }).GetAwaiter().GetResult();
                var tokenObj = resp.GetJsonAsync<ApiToken>().GetAwaiter().GetResult();
                string tokenJson = JsonSerializer.Serialize(tokenObj);
                File.WriteAllText(apiKeyFilePath, tokenJson, Encoding.UTF8);
            }

            string json = File.ReadAllText(apiKeyFilePath);
            var token = JsonSerializer.Deserialize<ApiToken>(json);

            restClient.WithOAuthBearerToken(token.Token);
        }

        public void Add(params Todo[] todos)
        {
            var submitTodos = todos.Select(t => new { text = t.Text });
            restClient.Request("todo").PostJsonAsync(submitTodos).GetAwaiter().GetResult();
        }

        public void Clear()
        {
            var todoList = FetchAll();
            if (!todoList.Any())
                return;
            restClient.Request("todo")
                .SetQueryParam("ids", todoList.Select(t => t.Id))
                .DeleteAsync()
                    .GetAwaiter().GetResult();
        }

        public void Dispose()
        {
            restClient.Dispose();
            GC.SuppressFinalize(this);
        }

        public IEnumerable<Todo> FetchAll()
        {
            return restClient.Request("todo").GetJsonAsync<IEnumerable<Todo>>().GetAwaiter().GetResult();
        }

        public Todo Get(long todoId)
        {
            throw new NotImplementedException();
        }

        public void Remove(long todoId)
        {
            restClient.Request("todo", todoId).DeleteAsync().GetAwaiter().GetResult();
        }

        public void Update(long todoId, Todo todo)
        {
            throw new NotImplementedException();
        }

    }
}