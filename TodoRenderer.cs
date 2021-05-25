using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoCli
{
    public class TodoRenderer
    {
        private readonly TodoApp app;

        public TodoRenderer(TodoApp app)
        {
            this.app = app ?? throw new ArgumentNullException(nameof(app));
        }

        public void RenderTaskList()
        {
            var list = app.GetAll().ToList();
            if (!list.Any())
            {
                Console.WriteLine("empty.");
                return;
            }

            Console.WriteLine(@"---- your current list ----");
            for (int i = 1; i <= list.Count; i++)
            {
                var todo = list[i - 1];
                Console.WriteLine($"[{i}] {todo.Text} | {todo.DateCreated:dd MMM yyyy -- hh:mm}");
            }
        }
    }
}
