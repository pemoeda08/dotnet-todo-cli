using Spectre.Console.Cli;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoCli.Commands
{
    public sealed class RemoveCommand : Command<RemoveCommand.Settings>
    {
        private readonly TodoApp todoApp;
        private readonly TodoRenderer renderer;

        public RemoveCommand()
        {
            todoApp = TodoApp.Default();
            renderer = new TodoRenderer(todoApp);
        }

        public sealed class Settings : CommandSettings
        {
            [Description("1-based index of task to be removed.")]
            [CommandArgument(0, "[index]")]
            public int Index { get; init; }

            [CommandOption("-i|--interactive")]
            [DefaultValue(false)]
            public bool Interactive { get; init; }
        }

        public override int Execute([NotNull] CommandContext context, [NotNull] Settings settings)
        {
            var list = todoApp.GetAll().OrderBy(t => t.DateCreated).ToList();
            int argsIndex = settings.Index;
            if (argsIndex < 1 || argsIndex > list.Count)
            {
                Console.WriteLine("todo remove: index out of bound.");
                return -1;
            }

            int index = argsIndex - 1;

            todoApp.Remove(list[index].Id);
            renderer.RenderTaskList();
            return 0;
        }
    }
}
