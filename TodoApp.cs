using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TodoCli
{

    public class TodoApp
    {
        private static TodoApp _defaultInstance = null;

        public static TodoApp Default()
        {
            if (_defaultInstance == null)
            {
                ITodoRepository repo = new TodoFileRepository();
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

            if (todo.Id == Guid.Empty)
                return TodoValidationResult.InvalidId;

            return TodoValidationResult.Success;
        }

        public TodoValidationResult Add(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return TodoValidationResult.NullOrEmptyText;

            var newTodo = new Todo
            {
                Id = Guid.NewGuid(),
                DateCreated = DateTime.Now,
                Text = text
            };

            repository.Add(newTodo);

            return TodoValidationResult.Success;
        }

        public TodoValidationResult Update(Guid todoId, Todo todo)
        {
            if (todo == null)
                throw new ArgumentNullException(nameof(todo));

            if (todoId == Guid.Empty || todoId != todo.Id)
                return TodoValidationResult.InvalidId;

            var validationResult = ValidateTodo(todo);
            if (validationResult != TodoValidationResult.Success)
                return validationResult;

            repository.Update(todoId, todo);
            return TodoValidationResult.Success;
        }

        public void Remove(Guid todoId) => repository.Remove(todoId);
        public void Clear() => repository.Clear();

    }

    public interface ITodoRepository
    {
        IEnumerable<Todo> FetchAll();
        Todo Get(Guid todoId);
        void Add(Todo todo);
        void Update(Guid todoId, Todo todo);
        void Remove(Guid todoId);
        void Clear();
    }

    public class TodoFileRepository : ITodoRepository
    {
        private readonly string DIRECTORY_PATH = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".todo");
        private readonly string FILENAME = "list.todo";
        private SortedList<Guid, Todo> _cache;

        public TodoFileRepository()
        {
            EnsureFileCreated();
            FillCache();
        }

        private void EnsureFileCreated()
        {
            Directory.CreateDirectory(DIRECTORY_PATH);
            string fullPath = Path.Combine(DIRECTORY_PATH, FILENAME);
            if (!File.Exists(fullPath))
                File.CreateText(fullPath).Dispose();
        }

        public void FillCache()
        {
            string fullPath = Path.Combine(DIRECTORY_PATH, FILENAME);
            string text = File.ReadAllText(fullPath);
            if (!string.IsNullOrWhiteSpace(text))
                _cache = new SortedList<Guid, Todo>(JsonSerializer.Deserialize<List<Todo>>(text).ToDictionary(t => t.Id));
            else
                _cache = new SortedList<Guid, Todo>();
        }

        public IEnumerable<Todo> FetchAll() => _cache.Any() ? _cache.Values.AsEnumerable() : Enumerable.Empty<Todo>();

        private void Save()
        {
            string fullPath = Path.Combine(DIRECTORY_PATH, FILENAME);
            string text = JsonSerializer.Serialize(_cache.Values.ToList());
            File.WriteAllText(fullPath, text);
        }


        public Todo Get(Guid todoId) => _cache[todoId];

        public void Add(Todo todo)
        {
            _cache.Add(todo.Id, todo);
            Save();
        }

        public void Update(Guid todoId, Todo todo)
        {
            var existing = _cache[todoId];
            existing.Text = todo.Text;
            Save();
        }

        public void Remove(Guid todoId)
        {
            _cache.Remove(todoId);
            Save();
        }

        public void Clear()
        {
            _cache.Clear();
            Save();
        }
    }

    public enum TodoValidationResult
    {
        Success,
        InvalidId,
        NullOrEmptyText
    }

    public record Todo
    {
        public Guid Id { get; init; }
        public string Text { get; set; }
        public DateTime DateCreated { get; init; }
    }

}
