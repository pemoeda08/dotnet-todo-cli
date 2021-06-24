using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace TodoCli
{
    public interface ITodoRepository
    {
        IEnumerable<Todo> FetchAll();
        Todo Get(long todoId);
        void Add(params Todo[] todo);
        void Update(long todoId, Todo todos);
        void Remove(long todoId);
        void Clear();
    }

    public class TodoFileRepository : ITodoRepository
    {
        private readonly string DIRECTORY_PATH = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".todo");
        private readonly string FILENAME = "list.todo";
        private SortedList<long, Todo> _cache;

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
                _cache = new SortedList<long, Todo>(JsonSerializer.Deserialize<List<Todo>>(text).ToDictionary(t => t.Id));
            else
                _cache = new SortedList<long, Todo>();
        }

        public IEnumerable<Todo> FetchAll() => _cache.Any() ? _cache.Values.AsEnumerable() : Enumerable.Empty<Todo>();

        private void Save()
        {
            string fullPath = Path.Combine(DIRECTORY_PATH, FILENAME);
            string text = JsonSerializer.Serialize(_cache.Values.ToList());
            File.WriteAllText(fullPath, text);
        }


        public Todo Get(long todoId) => _cache[todoId];

        public void Add(params Todo[] todos)
        {
            todos.ToList().ForEach(todo =>
            {
                _cache.Add(todo.Id, todo);
            });
            Save();
        }

        public void Update(long todoId, Todo todo)
        {
            var existing = _cache[todoId];
            existing.Text = todo.Text;
            Save();
        }

        public void Remove(long todoId)
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
}