using Spectre.Console.Cli;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoCli.Commands
{
    public sealed class ListCommand : Command
    {
        private readonly TodoApp todoApp;
        private readonly TodoRenderer renderer;

        public ListCommand()
        {
            todoApp = TodoApp.Default();
            renderer = new TodoRenderer(todoApp);
        }

        public override int Execute(CommandContext context)
        {
            renderer.RenderTaskList();
            return 0;
        }
    }
}
