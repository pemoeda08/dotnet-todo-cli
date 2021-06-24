using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;

namespace TodoCli
{
    public class TodoApiRepository : ITodoRepository
    {
        private readonly IFlurlClient restClient;

        public TodoApiRepository(IFlurlClient restClient)
        {
            this.restClient = restClient ?? throw new ArgumentNullException(nameof(restClient));
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