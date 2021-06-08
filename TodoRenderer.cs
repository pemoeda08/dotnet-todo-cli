using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spectre.Console;

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
            var list = app.GetAll().OrderBy(t => t.DateCreated).ToList();
            if (!list.Any())
            {
                Console.WriteLine("empty.");
                return;
            }

            var table = new Table();
            table.Border = TableBorder.Square;
            table.AddColumns(
                new TableColumn("[green]No.[/]"), 
                new TableColumn("[yellow]Todo[/]"), 
                new TableColumn("[blue]Created At[/]").Centered());
            table.Expand();
            table.Columns[0].Width(1).NoWrap();
            table.Columns[1].Width(10);
            table.Columns[2].Width(3);
            for (int i = 1; i <= list.Count; i++)
            {
                var todo = list[i - 1];
                table.AddRow($"{i}", todo.Text, todo.DateCreated.ToString("dd MMM yyyy, HH:mm"));
            }
            AnsiConsole.Render(table);
        }
    }
}
