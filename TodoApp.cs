using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Flurl.Http;
using Microsoft.Extensions.Configuration;

namespace TodoCli
{

    public sealed class TodoApp : IDisposable
    {
        private static TodoApp _defaultInstance = null;

        public static TodoApp Default()
        {
            if (_defaultInstance == null)
            {
                ITodoRepository repo = new TodoApiRepository();
                _defaultInstance = new TodoApp(repo);
            }
            return _defaultInstance;
        }

        private readonly ITodoRepository repository;

        public TodoApp(ITodoRepository repository)
        {
            this.repository = repository;
        }

        public IEnumerable<Todo> GetAll() => repository.FetchAll();

        private TodoValidationResult ValidateTodo(Todo todo)
        {
            if (todo == null)
                throw new ArgumentNullException(nameof(todo));

            if (string.IsNullOrWhiteSpace(todo.Text))
                return TodoValidationResult.NullOrEmptyText;

            if (todo.Id == 0)
                return TodoValidationResult.InvalidId;

            return TodoValidationResult.Success;
        }

        public TodoValidationResult Add(params string[] texts)
        {
            if (texts == null)
                return TodoValidationResult.NullOrEmptyText;

            var newTodos = texts.Select(t => new Todo
            {
                Text = t
            }).ToArray();

            repository.Add(newTodos);

            return TodoValidationResult.Success;
        }

        public TodoValidationResult Update(long todoId, Todo todo)
        {
            if (todo == null)
                throw new ArgumentNullException(nameof(todo));

            if (todoId == 0 || todoId != todo.Id)
                return TodoValidationResult.InvalidId;

            var validationResult = ValidateTodo(todo);
            if (validationResult != TodoValidationResult.Success)
                return validationResult;

            repository.Update(todoId, todo);
            return TodoValidationResult.Success;
        }

        public void Remove(long todoId) => repository.Remove(todoId);
        public void Clear() => repository.Clear();

        public void Dispose()
        {
            repository.Dispose();
        }
    }

    public enum TodoValidationResult
    {
        Success,
        InvalidId,
        NullOrEmptyText
    }

}
