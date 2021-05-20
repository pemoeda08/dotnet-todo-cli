using Spectre.Console;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Linq;
using System.Collections.Generic;

namespace TodoCli
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("usage: add, list, remove, clear.");
                return;
            }

            var todoRepo = new TodoFileRepository();
            var todoApp = new TodoApp(todoRepo);

            void ListTodo(List<Todo> list)
            {
                Console.WriteLine(@"---- your current list ----");
                for (int i = 1; i <= list.Count; i++)
                {
                    var todo = list[i - 1];
                    Console.WriteLine($"[{i}] {todo.Text} | {todo.DateCreated:dd MMM yyyy -- hh:mm}");
                }
            }

            if (args[0] == "add")
            {
                string text = string.Join(" ", args[1..]).Trim();
                var result = todoApp.Add(text);
                switch (result)
                {
                    case TodoValidationResult.NullOrEmptyText:
                        Console.WriteLine("todo add: please input text.");
                        break;

                    case TodoValidationResult.Success:
                        var list = todoApp.GetAll().ToList();
                        ListTodo(list);
                        break;
                }
            }
            else if (args[0] == "list")
            {
                ListTodo(todoApp.GetAll().ToList());
            }
            else if (args[0] == "remove" || args[0] == "delete" || args[0] == "del")
            {
                var list = todoApp.GetAll().ToList();
                bool success = int.TryParse(args[1], out int argsIndex);
                if (!success)
                {
                    Console.WriteLine("todo remove: index specified must be an integer.");
                    return;
                }
                if (argsIndex < 1 || argsIndex > list.Count)
                {
                    Console.WriteLine("todo remove: index out of bound.");
                    return;
                }

                int index = argsIndex - 1;

                todoApp.Remove(list[index].Id);
                ListTodo(list);
            }
            else if (args[0] == "clear")
            {
                todoApp.Clear();
                Console.WriteLine("todo: list cleared.");
            }
            else
            {
                Console.WriteLine($"todo: unknown command '{args[0]}'");
            }
        }
    }

}
