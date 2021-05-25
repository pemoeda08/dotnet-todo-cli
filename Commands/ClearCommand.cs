using Spectre.Console.Cli;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoCli.Commands
{
    public sealed class ClearCommand : Command
    {
        private readonly TodoApp todoApp;
        private readonly TodoRenderer renderer;

        public ClearCommand()
        {
            todoApp = TodoApp.Default();
            renderer = new TodoRenderer(todoApp);
        }

        public override int Execute(CommandContext context)
        {
            todoApp.Clear();
            renderer.RenderTaskList();
            Console.WriteLine("todo: list cleared.");
            return 0;
        }
    }
}
